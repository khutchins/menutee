using Menutee;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SampleMenuWithCustomBack : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string KEY_BACK = "back";

	private const string KEY_SHOW_OTHER = "show other";
	private const string KEY_OPTIONS = "options";
	private const string KEY_DO_NOTHING = "do nothing";
	private const string KEY_QUALITY = "quality";
	private const string KEY_DIFFICULTY = "difficulty";
	private const string KEY_VSYNC = "vsync";
	private const string KEY_WINDOW_MODE = "window mode";

	[Header("Sample Prefabs")]
	[Tooltip("Prefab used for option-select elements (the default MenuGenerator prefabs don't include one).")]
	public GameObject OptionSelectPrefab;

	[Header("Focus Sample")]
	[Tooltip("Shared detail pane. Shows a description of whatever element is focused, cleared when nothing is.")]
	public TextMeshProUGUI DetailText;
	[Tooltip("Plays a tick as focus moves between fields (but not on panel changes).")]
	public AudioSource FocusAudioSource;
	public AudioClip FocusMoveClip;
	[Tooltip("Palette applied to option selects, so they stand out from the panel background.")]
	public PaletteConfig OptionSelectPalette;

	private MenuManager _manager;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		MenuConfig.Builder builder = new MenuConfig.Builder(false, false)
			.SetDefaultPaletteConfig(PaletteConfig)
			// Detail pane set by menu-wide listener.
			.AddFocusedElementChangedCallback(change => SetDetail(DetailForFocus(change.Current)))
			// A second, independent listener on the same event plays an audio
			// tick. Fire whenever focus lands on an element as long as user 
			// initiated.
			.AddFocusedElementChangedCallback(change => {
				if (change.Current.HasFocus && change.Source == FocusSource.UserInput) {
					PlayMenuAudio();
				}
			});

		builder.AddPanelConfig(BackablePanel(MENU_KEY_MAIN)
			.AddPanelObject(Button(KEY_SHOW_OTHER, "Show Other Menu", _ => _manager.PushPanel("Other")))
			.AddPanelObject(Button(KEY_OPTIONS, "Options", _ => _manager.PushPanel("Options"))),
			true);

		builder.AddPanelConfig(BackablePanel("Options")
			.AddPanelObject(OptionSelect(KEY_QUALITY, "Quality", "Low", "Medium", "High"))
			.AddPanelObject(OptionSelect(KEY_DIFFICULTY, "Difficulty", "Easy", "Normal", "Hard"))
			.AddPanelObject(OptionSelect(KEY_VSYNC, "VSync", "Off", "On")));

		builder.AddPanelConfig(BackablePanel("Other")
			.AddPanelObject(Button(KEY_DO_NOTHING, "Show Empty Menu", _ => _manager.PushPanel("Third")))
			.AddPanelObject(OptionSelect(KEY_WINDOW_MODE, "Window Mode", "Windowed", "Borderless", "Fullscreen")));

		builder.AddPanelConfig(BackablePanel("Third"));

		CreateMenu(_manager, builder);
	}

	/// <summary>A button element wired to a display label and press handler.</summary>
	private ButtonConfig.Builder Button(string key, string label, ButtonPressedHandler onPressed) {
        void wrappedHandler(ButtonManager buttonManager) {
            PlayMenuAudio();
            onPressed(buttonManager);
        }
        return new ButtonConfig.Builder(key, ButtonPrefab)
			.SetDisplayText(label)
			.SetButtonPressedHandler(wrappedHandler);
	}

	/// <summary>
	/// An option-select element. Overridden with the option-select palette so it
	/// stands out from the shared panel background, and wired to update the detail
	/// pane with the selected option's description whenever the value changes.
	/// </summary>
	private OptionSelectConfig.Builder OptionSelect(string key, string label, params string[] options) {
		return new OptionSelectConfig.Builder(key, OptionSelectPrefab)
			.SetDisplayText(label)
			.AddOptionStrings(options)
			.SetPaletteConfigOverride(OptionSelectPalette)
			// Value changes only happen while the element is focused, so the detail
			// pane is safe to update directly here.
			.SetOptionSelectedHandler((manager, index, option) => {
				PlayMenuAudio();
				SetDetail(OptionDetailFor(key, index));
			});
	}

	/// <summary>
	/// A panel whose (default) prefab includes the back button. The back button is
	/// shown only when the panel can be popped (not at root), and navigation is
	/// rebuilt on each display so the button joins or leaves the nav order as its
	/// visibility changes.
	/// </summary>
	private PanelConfig.Builder BackablePanel(string key) {
		return new PanelConfig.Builder(key)
			// Opt the prefab-baked back button into the focus system. Creation
			// runs after instantiation, so pm.BackSelectable exists.
			.SetCreationCallback((panelObj, manager) => {
				SamplePanelManager pm = manager as SamplePanelManager;
				manager.RegisterFocusSource(pm.BackSelectable, KEY_BACK);
			})
			// If a panel has no generated selectables (e.g. an empty leaf), fall
			// back to the back button as the default selection.
			.SetDefaultSelectableCallback((manager, selectables) => {
				if (selectables.Count > 0) {
					return selectables[0].gameObject;
				}
				SamplePanelManager pm = manager as SamplePanelManager;
				return pm.BackSelectable.gameObject;
			})
			.SetOnDisplayCallback((panelObj, manager) => {
				SamplePanelManager pm = manager as SamplePanelManager;
				bool showBack = !manager.Manager.IsAtRoot();
				pm.BackSelectable.gameObject.SetActive(showBack);

				// Rebuild navigation from the generated selectables, appending the
				// back button only while it's visible (navigating to an inactive
				// Selectable silently fails).
				List<Selectable> nav = new List<Selectable>(manager.Selectables);
				if (showBack) {
					nav.Add(pm.BackSelectable);
				}
				SetVerticalNavigation(nav);
			});
	}

	private void SetDetail(string text) {
		if (DetailText != null) {
			DetailText.text = text ?? "";
		} else {
			Debug.Log("Detail: " + (string.IsNullOrEmpty(text) ? "(cleared)" : text));
		}
	}

	private void PlayMenuAudio() {
		if (FocusAudioSource != null && FocusMoveClip != null) {
			FocusAudioSource.PlayOneShot(FocusMoveClip);
		}
	}

	// Per-option detail text, indexed to match the option order passed to OptionSelect.
	// This probably could be done more elegantly, but it's good enough for a demo.
	private static readonly Dictionary<string, string[]> _optionDetails = new Dictionary<string, string[]> {
		[KEY_QUALITY] = new[] { "Best performance.", "Balanced quality and speed.", "Best visuals." },
		[KEY_DIFFICULTY] = new[] { "Relaxed play.", "The intended experience.", "For veterans." },
		[KEY_VSYNC] = new[] { "May tear, but lowest latency.", "No tearing, but adds latency." },
		[KEY_WINDOW_MODE] = new[] { "Classic resizable window.", "Fills the screen, easy to alt-tab.", "Exclusive fullscreen." },
	};

	/// <summary>
	/// Detail text for whatever is currently focused: an option select shows its
	/// current option's description. Everything else shows static text.
	/// </summary>
	private static string DetailForFocus(FocusRef focus) {
		if (!focus.HasFocus) {
			return null;
		}
		if (focus.Element is OptionSelectManager option && option.OptionSelect != null) {
			return OptionDetailFor(focus.Key, option.OptionSelect.value);
		}
		return DescriptionFor(focus.Key);
	}

	private static string OptionDetailFor(string key, int index) {
		if (_optionDetails.TryGetValue(key, out string[] details) && index >= 0 && index < details.Length) {
			return details[index];
		}
		return "";
	}

	private static string DescriptionFor(string key) {
		if (key == KEY_SHOW_OTHER) return "Opens the Other menu.";
		if (key == KEY_OPTIONS) return "Adjust game settings.";
		if (key == KEY_DO_NOTHING) return "Opens an empty menu.";
		if (key == KEY_BACK) return "Returns to the previous menu.";
		return "";
	}
}
