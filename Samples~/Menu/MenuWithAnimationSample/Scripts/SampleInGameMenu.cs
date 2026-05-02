using Menutee;
using UnityEngine;

/// <summary>
/// Minimal contrived sample: a main panel with a couple of buttons, plus a
/// secondary panel that shows each of the standard UI element types wired up
/// to log to the console. Not meant to look like a real game menu — it exists
/// to demonstrate how the builder API hooks each element type to its handler.
/// </summary>
public class SampleInGameMenu : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_ELEMENTS = "Elements";

	private MenuManager _manager;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		MenuConfig.Builder builder = new MenuConfig.Builder(true, true, PaletteConfig);
		MenuAttributes attr = MenuAttributes.StandardPauseMenu();
		attr.cursorLockMode = CursorLockMode.None;
		builder.SetMenuAttributes(attr);

		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_MAIN)
			.AddPanelObject(
				new ButtonConfig.Builder("log", ButtonPrefab)
					.SetDisplayText("Log Hello")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						Debug.Log("[SampleInGameMenu] Hello from the main panel.");
					}), true)
			.AddPanelObject(
				new ButtonConfig.Builder("elements", ButtonPrefab)
					.SetDisplayText("Show Element Types")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PushPanel(MENU_KEY_ELEMENTS);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("close", ButtonPrefab)
					.SetDisplayText("Close")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.ExitMenu();
					})), true);

		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_ELEMENTS)
			.AddPanelObject(
				new ButtonConfig.Builder("back", ButtonPrefab)
					.SetDisplayText("Back")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PopPanel();
					}), true)
			.AddPanelObject(
				new SliderConfig.Builder("slider", SliderPrefab, 0f, 1f, 0.5f)
					.SetDisplayText("Slider")
					.SetSliderUpdatedHandler(delegate (SliderManager m, float v) {
						Debug.LogFormat("[SampleInGameMenu] Slider = {0:0.00}", v);
					}))
			.AddPanelObject(
				new ToggleConfig.Builder("toggle", TogglePrefab)
					.SetDisplayText("Toggle")
					.SetIsOn(false)
					.SetTogglePressedHandler(delegate (ToggleManager m, bool v) {
						Debug.LogFormat("[SampleInGameMenu] Toggle = {0}", v);
					}))
			.AddPanelObject(
				new DropdownConfig.Builder("dropdown", DropdownPrefab)
					.SetDisplayText("Dropdown")
					.AddOptionStrings(new[] { "Option A", "Option B", "Option C" })
					.SetDefaultOptionIndex(0)
					.SetDropdownChosenHandler(delegate (DropdownManager m, int idx, string opt) {
						Debug.LogFormat("[SampleInGameMenu] Dropdown = [{0}] {1}", idx, opt);
					})));

		CreateMenu(_manager, builder);
	}
}
