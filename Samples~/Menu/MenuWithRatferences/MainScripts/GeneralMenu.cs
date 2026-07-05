using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ratferences;
using Menutee;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GeneralMenu : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_OPTIONS = "Options";
	public readonly static string MENU_KEY_VERIFY = "Verify";

	[Header("Prefabs")]
	[Tooltip("Shared prefab set.")]
	[SerializeField] private SampleMenuPrefabSet _prefabs;
	public PaletteConfig PaletteConfig;

	[Header("Menu Signals")]
	[SerializeField] private Signal ControlMenuSignal;
	[SerializeField] private Signal CreditsMenuSignal;

	[Header("Other Bindings")]
	[SerializeField] private bool _mainMenu;
	[SerializeField] private SampleOptionsSO Options;
	[SerializeField] private AudioMixerGroup _musicGroup;
	[SerializeField] private AudioMixerGroup _sfxGroup;

	private MenuManager _manager;
	private System.Action _onVerify;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		MenuConfig.Builder builder = new MenuConfig.Builder(!_mainMenu, !_mainMenu).SetDefaultPaletteConfig(PaletteConfig);
		MenuAttributes noLock = _mainMenu ? MenuAttributes.StandardNonPauseMenu() : MenuAttributes.StandardPauseMenu();
		noLock.cursorLockMode = UnityEngine.CursorLockMode.None;
		builder.SetMenuAttributes(noLock);

		if (_mainMenu) {
			AddMainMenu(builder);
        } else {
			AddInGameMenu(builder);
        }

		AddOptionsPanels(builder);
		AddVerifier(builder);
		CreateMenu(_manager, builder);
	}

	private void AddInGameMenu(MenuConfig.Builder builder) {
		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_MAIN)
			.AddPanelObject(
				new ButtonConfig.Builder("resume", _prefabs.ButtonPrefab)
					.SetDisplayText("Resume")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.ExitMenu();
					}), true)
			.AddPanelObject(
				new ButtonConfig.Builder("options", _prefabs.ButtonPrefab)
					.SetDisplayText("Options")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.PushPanel(MENU_KEY_OPTIONS);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("restart", _prefabs.ButtonPrefab)
					.SetDisplayText("Restart")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("exit", _prefabs.ButtonPrefab)
					.SetDisplayText("Exit")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						VerifyAction(() => SceneManager.LoadScene("MenuScene"));
					})));
	}

	private void AddVerifier(MenuConfig.Builder builder) {
		PanelConfig.Builder verifier = new PanelConfig.Builder(MENU_KEY_VERIFY);

		verifier.AddPanelObject(new TextConfig.Builder("text", _prefabs.TextPrefab, "Are you sure?"));
		verifier.AddPanelObject(
			new ButtonConfig.Builder("yes", _prefabs.ButtonPrefab)
				.SetDisplayText("Yes")
				.SetButtonPressedHandler(delegate (ButtonManager manager) {
					_manager.PopPanel();
					_onVerify.Invoke();
				}));
		verifier.AddPanelObject(
			new ButtonConfig.Builder("no", _prefabs.ButtonPrefab)
				.SetDisplayText("No")
				.SetButtonPressedHandler(delegate (ButtonManager manager) {
					_manager.PopPanel();
				}), true);

		builder.AddPanelConfig(verifier);
	}

	private void AddMainMenu(MenuConfig.Builder builder) {
		PanelConfig.Builder main = new PanelConfig.Builder(MENU_KEY_MAIN);
		main.AddPanelObject(
			new ButtonConfig.Builder("play", _prefabs.ButtonPrefab)
				.SetDisplayText("Play")
				.SetButtonPressedHandler(delegate (ButtonManager manager) {
					Debug.Log("Pressed play!");
				}));
		main.AddPanelObject(
			new ButtonConfig.Builder("options", _prefabs.ButtonPrefab)
				.SetDisplayText("Options")
				.SetButtonPressedHandler(delegate (ButtonManager manager) {
					_manager.PushPanel(MENU_KEY_OPTIONS);
				}));
		main.AddPanelObject(
			new ButtonConfig.Builder("credits", _prefabs.ButtonPrefab)
				.SetDisplayText("Credits")
				.SetButtonPressedHandler(delegate (ButtonManager manager) {
					CreditsMenuSignal.Raise();
				}));
		main.AddPanelObject(
			new ButtonConfig.Builder("exit", _prefabs.ButtonPrefab)
				.SetDisplayText("Exit")
				.SetButtonPressedHandler(delegate (ButtonManager manager) {
					VerifyAction(() => Application.Quit());
				}));

		builder.AddPanelConfig(main, true);
	}

	private void VerifyAction(System.Action onDo) {
		_onVerify = onDo;
		_manager.PushPanel(MENU_KEY_VERIFY);
	}

	/// <summary>
	/// Standard resolution dropdown. Filters out Hz values.
	/// </summary>
	/// <returns>Resolution panel object config</returns>
	PanelObjectConfig ResolutionConfig(GameObject dropdownPrefab) {

#if UNITY_2022_2_OR_NEWER
		Resolution[] filteredResolutions = Screen.resolutions.Where(res => System.Math.Abs(res.refreshRateRatio.value - Screen.currentResolution.refreshRateRatio.value) <= 1).ToArray();
#else
		Resolution[] filteredResolutions = Screen.resolutions.Where(res => Mathf.Abs(res.refreshRate - Screen.currentResolution.refreshRate) <= 1).ToArray();
#endif
		Resolution playerResolution = new Resolution();
		playerResolution.width = Screen.width;
		playerResolution.height = Screen.height;
		int idx = filteredResolutions.Length - 1;
		for (int i = 0; i < filteredResolutions.Length; i++) {
			if (filteredResolutions[i].width == playerResolution.width && filteredResolutions[i].height == playerResolution.height) {
				idx = i;
				break;
			}
		}
		IEnumerable<string> resolutionStrings = filteredResolutions.Select(x => x.width + " x " + x.height);

		return new DropdownConfig.Builder("resolution", dropdownPrefab)
			.SetDisplayText("Resolution")
			.AddOptionStrings(resolutionStrings)
			.SetDefaultOptionIndex(idx)
			.SetDropdownChosenHandler(delegate (DropdownManager manager, int newIndex, string optionString) {
				Resolution res = filteredResolutions[newIndex];
				Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
				Debug.Log("Setting resolution to " + res);
			}).Build();
	}

	void AddOptionsPanels(MenuConfig.Builder menuBuilder) {

		PanelConfig AddControlsPanel(string key) {
			PanelConfig.Builder builder = new PanelConfig.Builder(key);

			List<string> intOptions = new List<string>();
			for (int i = 0; i < 20; i++) {
				intOptions.Add(((i + 1) * 16).ToString());
			}

			builder.AddPanelObject(new SliderRefConfig.Builder("option1", _prefabs.SliderPrefab, 0, 5, Options.Option1).SetDisplayText("Option 1"));
			builder.AddPanelObject(new ToggleRefConfig.Builder("option2", _prefabs.TogglePrefab, Options.Option2).SetDisplayText("Option 2"));
			builder.AddPanelObject(new OptionSelectRefConfig.Builder("option3", _prefabs.OptionSelectPrefab, Options.Option3).SetDisplayText("Option 3").AddOptionStrings(intOptions).SetLoops(false));
			builder.AddPanelObject(new ButtonConfig.Builder("controls", _prefabs.ButtonPrefab).SetDisplayText("Rebind Controls")
				.SetButtonPressedHandler(delegate (ButtonManager manager) {
					ControlMenuSignal.Raise();
				}));
			AddBackOption(builder, _manager, _prefabs.ButtonPrefab);

			PanelConfig config = builder.Build();
			menuBuilder.AddPanelConfig(config);
			return config;
		}


		PanelConfig AddVideoPanel(string key) {
			PanelConfig.Builder builder = new PanelConfig.Builder(key);

			builder.AddPanelObject(new DropdownConfig.Builder("quality", _prefabs.DropdownPrefab)
				.SetDisplayText("Quality")
				.AddOptionStrings(QualitySettings.names)
				.SetDefaultOptionIndex(QualitySettings.GetQualityLevel())
				.SetDropdownChosenHandler(delegate (DropdownManager manager, int newIndex, string optionString) {
					QualitySettings.SetQualityLevel(newIndex);
				}));

			// No point in showing resolution config in WebGL - It does nothing.
			if (Application.platform != RuntimePlatform.WebGLPlayer) {
				builder.AddPanelObject(ResolutionConfig(_prefabs.DropdownPrefab));
			}

			builder.AddPanelObject(new ToggleConfig.Builder("fullscreen", _prefabs.TogglePrefab)
				.SetDisplayText("Fullscreen")
				.SetIsOn(Screen.fullScreen)
				.SetTogglePressedHandler(delegate (ToggleManager manager, bool newValue) {
					Screen.fullScreen = newValue;
				}));
			AddBackOption(builder, _manager, _prefabs.ButtonPrefab);

			PanelConfig config = builder.Build();
			menuBuilder.AddPanelConfig(config);
			return config;
		}

		void SetMixerVolume(AudioMixerGroup group, string paramName, float volumePercent) {
			float decibels = ComputeDecibelVolume(volumePercent);
			group.audioMixer.SetFloat(paramName, decibels);
		}

		float ComputeDecibelVolume(float volumePercent) {
			// Volume must be [0.0001, 1].
			volumePercent = Mathf.Max(0.0001f, Mathf.Min(1, volumePercent));
			return Mathf.Log10(volumePercent) * 20;
		}

		PanelConfig AddAudioPanel(string key) {
			PanelConfig.Builder builder = new PanelConfig.Builder(key);

			builder.AddPanelObject(new SliderRefConfig.Builder("sfxvolume", _prefabs.SliderPrefab, 0f, 1f, Options.SFXVolume).SetDisplayText("SFX Volume")
			.SetCreationCallback((GameObject go) => {
				// Have to run this after a frame, as this API doesn't work in awake (at least when I wrote the code).
				_manager.RunGenericActionAfterFrame(() => {
					SetMixerVolume(_sfxGroup, "SFXVolume", Options.SFXVolume.Value);
				});
			})
			.SetSliderUpdatedHandler((SliderManager manager, float newValue) => {
				SetMixerVolume(_sfxGroup, "SFXVolume", newValue);
			}));
			builder.AddPanelObject(new SliderRefConfig.Builder("musicvolume", _prefabs.SliderPrefab, 0f, 1f, Options.MusicVolume).SetDisplayText("Music Volume")
			.SetCreationCallback((GameObject go) => {
				// Have to run this after a frame, as this API doesn't work in awake (at least when I wrote the code).
				_manager.RunGenericActionAfterFrame(() => {
					SetMixerVolume(_musicGroup, "MusicVolume", Options.MusicVolume.Value);
				});
			})
			.SetSliderUpdatedHandler((SliderManager manager, float newValue) => {
				SetMixerVolume(_musicGroup, "MusicVolume", newValue);
			}));
			AddBackOption(builder, _manager, _prefabs.ButtonPrefab);

			PanelConfig config = builder.Build();
			menuBuilder.AddPanelConfig(config);
			return config;
		}

		PanelConfig.Builder builder = new PanelConfig.Builder(MENU_KEY_OPTIONS);
		AddPanelHookup(builder, _manager, _prefabs.ButtonPrefab, "Controls", AddControlsPanel("Controls"));
		AddPanelHookup(builder, _manager, _prefabs.ButtonPrefab, "Video", AddVideoPanel("Video"));
		AddPanelHookup(builder, _manager, _prefabs.ButtonPrefab, "Audio", AddAudioPanel("Audio"));
		AddBackOption(builder, _manager, _prefabs.ButtonPrefab);

		menuBuilder.AddPanelConfig(builder);
	}

	public static void AddBackOption(PanelConfig.Builder builder, MenuManager menuManager, GameObject buttonPrefab) {
		builder.AddPanelObject(new ButtonConfig.Builder("back", buttonPrefab)
			.SetDisplayText("Back")
			.SetButtonPressedHandler(delegate (ButtonManager manager) {
				menuManager.PopPanel();
			}));
	}

	public static void AddPanelHookup(PanelConfig.Builder builder, MenuManager menuManager, GameObject buttonPrefab, string optionName, PanelConfig targetPanel, bool defaultObject = false) {
		builder.AddPanelObject(new ButtonConfig.Builder(optionName, buttonPrefab)
			.SetDisplayText(optionName)
			.SetButtonPressedHandler(delegate (ButtonManager manager) {
				menuManager.PushPanel(targetPanel.Key);
			}), defaultObject);
	}
}
