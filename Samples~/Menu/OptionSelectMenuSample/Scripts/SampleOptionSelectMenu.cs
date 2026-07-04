using Menutee;
using Ratferences;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleOptionSelectMenu : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";

	public GameObject OptionSelectPrefab;

	private MenuManager _manager;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		// This is just the option select prefab in action.
		MenuConfig.Builder builder = new MenuConfig.Builder(false, true).SetDefaultPaletteConfig(PaletteConfig);

		PanelConfig.Builder panel = new PanelConfig.Builder(MENU_KEY_MAIN);
		panel.AddPanelObject(new OptionSelectConfig.Builder("quality", OptionSelectPrefab)
		.SetDisplayText("Quality")
		.AddOptionStrings(QualitySettings.names)
		.SetDefaultOptionIndex(QualitySettings.GetQualityLevel())
		.SetOptionSelectedHandler(delegate (OptionSelectManager manager, int newIndex, string optionString) {
			QualitySettings.SetQualityLevel(newIndex);
		}));

		panel.AddPanelObject(new OptionSelectConfig.Builder("something", OptionSelectPrefab)
		.SetDisplayText("Something")
		.AddOptionString("cool", false)
		.AddOptionString("off", false)
		.AddOptionString("out of line", false)
		.AddOptionString("right", true)
		.AddOptionString("wrong", false)
		.SetOptionSelectedHandler(delegate (OptionSelectManager manager, int newIndex, string optionString) {
			Debug.Log($"Selected {optionString}");
		}));

		panel.AddPanelObject(new OptionSelectConfig.Builder("something_else", OptionSelectPrefab)
		.SetDisplayText("No Loop")
		.SetLoops(false)
		.AddOptionString("1", false)
		.AddOptionString("2", false)
		.AddOptionString("3", true)
		.SetOptionSelectedHandler(delegate (OptionSelectManager manager, int newIndex, string optionString) {
			Debug.Log($"Selected {optionString}");
		}));

		panel.AddPanelObject(new OptionSelectToggleConfig.Builder("item123", OptionSelectPrefab, "Off", "On", true)
		.SetDisplayText("Toggle")
		.SetToggleManager((OptionSelectManager manager, bool toggle) => {
			Debug.Log($"Is on? {toggle}");
		}));

		// OptionSelectMappedConfig<T> pairs each display string with a typed value,
		// so the handler receives the mapped value directly instead of an index and
		// raw string.
		panel.AddPanelObject(new OptionSelectMappedConfig<FullScreenMode>.Builder("window_mode", OptionSelectPrefab)
		.SetDisplayText("Window Mode")
		.AddOptionString("Fullscreen", FullScreenMode.ExclusiveFullScreen)
		.AddOptionString("Borderless", FullScreenMode.FullScreenWindow, true)
		.AddOptionString("Windowed", FullScreenMode.Windowed)
		.SetMappedHandler((OptionSelectManager manager, int index, string optionString, FullScreenMode mode) => {
			Debug.Log($"Window mode -> {mode}");
			// e.g. Screen.fullScreenMode = mode;
		}));

		// OptionSelectRefConfig binds the selected index to a Ratferences IntReference,
		// keeping the control and the shared value in sync both ways. Here we create one
		// at runtime for the demo. In a real project you'd assign a saved reference asset.
		IntReference difficultyRef = ScriptableObject.CreateInstance<IntReference>();
		difficultyRef.Value = 1;
		difficultyRef.ValueChanged += newIndex => Debug.Log($"Difficulty reference is now {newIndex}");

		panel.AddPanelObject(new OptionSelectRefConfig.Builder("difficulty", OptionSelectPrefab, difficultyRef)
		.SetDisplayText("Difficulty (Ref)")
		.AddOptionString("Easy", false)
		.AddOptionString("Normal", false)
		.AddOptionString("Hard", false)
		.SetOptionSelectedHandler(delegate (OptionSelectManager manager, int newIndex, string optionString) {
			Debug.Log($"Selected {optionString}");
		}));

		builder.AddPanelConfig(panel);
		CreateMenu(_manager, builder);
	}
}
