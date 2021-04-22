using KH;
using KH.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

/// <summary>
/// Helper class with some common menu setups.
/// </summary>
public static class MenuConfigHelper {
	public readonly static string KEY_RESUME = "resume";
	public readonly static string KEY_OPTIONS = "options";
	public readonly static string KEY_EXIT = "exit";
	public readonly static string KEY_BACK = "back";
	public readonly static string KEY_SLIDER_TEXT = "slidertext";
	public readonly static string KEY_SLIDER_LOOK = "sliderlook";
	public readonly static string KEY_SLIDER_VOLUME = "slidervolume";
	public readonly static string KEY_RESOLUTION = "resolution";
	public readonly static string KEY_QUALITY = "quality";
	public readonly static string KEY_FULLSCREEN = "fullscreen";

	public readonly static string KEY_GASOLINE = "gas";
	public readonly static string KEY_TOWER = "tower";

	/// <summary>
	/// Standard resolution dropdown. Filters out Hz values.
	/// </summary>
	/// <returns>Resolution panel object config</returns>
	public static PanelObjectConfig ResolutionConfig(GameObject dropdownPrefab) {

		Resolution[] filteredResolutions = Screen.resolutions.Where(res => Mathf.Abs(res.refreshRate - Screen.currentResolution.refreshRate) <= 1).ToArray();
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

	public static PanelConfig.Builder StandardOptionsPanel(string key, MenuHelper menuHelper, GameObject buttonPrefab, 
			GameObject sliderPrefab, GameObject dropdownPrefab, GameObject togglePrefab) {

		PanelConfig.Builder builder = new PanelConfig.Builder(key);

		builder.AddPanelObject(new ButtonConfig.Builder("back", buttonPrefab)
			.SetDisplayText("Back")
			.SetButtonPressedHandler(delegate (ButtonManager manager) {
				menuHelper.PopMenu();
			}));
		builder.AddPanelObject(new SliderConfig.Builder("sliderlook", sliderPrefab, 0.1f, 3f, 1f)
			.SetDisplayText("Look Speed")
			.SetSliderUpdatedHandler(delegate (SliderManager manager, float newValue) {
				// Handle sensitivity
			}));
		builder.AddPanelObject(new SliderConfig.Builder("slidervolume", sliderPrefab, 0f, 1f, 1f)
			.SetDisplayText("Volume")
			.SetSliderUpdatedHandler(delegate (SliderManager manager, float newValue) {
				// Handle sensitivity
			}));
		builder.AddPanelObject(new DropdownConfig.Builder("quality", dropdownPrefab)
			.SetDisplayText("Quality")
			.AddOptionStrings(QualitySettings.names)
			.SetDefaultOptionIndex(QualitySettings.GetQualityLevel())
			.SetDropdownChosenHandler(delegate (DropdownManager manager, int newIndex, string optionString) {
				QualitySettings.SetQualityLevel(newIndex);
			}));

		// No point in showing resolution config in WebGL - It does nothing.
		if (Application.platform != RuntimePlatform.WebGLPlayer) {
			builder.AddPanelObject(ResolutionConfig(dropdownPrefab));
		}

		builder.AddPanelObject(new ToggleConfig.Builder("fullscreen", togglePrefab)
			.SetDisplayText("Fullscreen")
			.SetIsOn(Screen.fullScreen)
			.SetTogglePressedHandler(delegate (ToggleManager manager, bool newValue) {
				Screen.fullScreen = newValue;
			}));

		return builder;
	}
}