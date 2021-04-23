using Menutee;
using UnityEngine.SceneManagement;

public class SampleInGameMenu : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_OPTIONS = "Options";
	public readonly static string MENU_KEY_CREDITS = "Credits";

	private MenuManager _manager;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		// This is an example of using a builder to make the menu config.
		// It's more verbose, but it lets you easily change the different
		// values in explicit ways.
		MenuConfig.Builder builder = new MenuConfig.Builder(true, true, PaletteConfig);

		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_MAIN)
			.AddPanelObject(
				new ButtonConfig.Builder("resume", ButtonPrefab)
					.SetDisplayText("Resume")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.ExitMenu();
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("options", ButtonPrefab)
					.SetDisplayText("Options")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.PushPanel(MENU_KEY_OPTIONS);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("restart", ButtonPrefab)
					.SetDisplayText("Restart")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("exit", ButtonPrefab)
					.SetDisplayText("Exit")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.ExitGame();
					})), true);

		builder.AddPanelConfig(MenuConfigHelper.StandardOptionsPanel(MENU_KEY_OPTIONS, _manager, ButtonPrefab, SliderPefab, DropdownPrefab, TogglePrefab));

		CreateMenu(_manager, builder);
	}
}
