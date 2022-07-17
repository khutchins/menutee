using Menutee;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleOptionSelectMenu : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";

	public GameObject OptionSelectPrefab;

	private MenuManager _manager;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		// This is just the option select prefab in action.
		MenuConfig.Builder builder = new MenuConfig.Builder(false, true, PaletteConfig);

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

		panel.AddPanelObject(new OptionSelectConfig.Builder("something", OptionSelectPrefab)
		.SetDisplayText("Doesn't Loop")
		.SetLoops(false)
		.AddOptionString("1", false)
		.AddOptionString("2", false)
		.AddOptionString("3", true)
		.SetOptionSelectedHandler(delegate (OptionSelectManager manager, int newIndex, string optionString) {
			Debug.Log($"Selected {optionString}");
		}));

		builder.AddPanelConfig(panel);
		CreateMenu(_manager, builder);
	}
}
