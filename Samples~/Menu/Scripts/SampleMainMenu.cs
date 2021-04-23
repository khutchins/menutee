using Menutee;
using UnityEngine;

public class SampleMainMenu : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_OPTIONS = "Options";

	public readonly static string KEY_RESUME = "resume";
	public readonly static string KEY_OPTIONS = "options";
	public readonly static string KEY_EXIT = "exit";
	public readonly static string KEY_BACK = "back";

	private MenuManager _manager;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		// This is an example of how you can create a MenuConfig object without a builder.
		// It works fine for when you're using the default options, but gets trickier if you
		// want to conditionally show options or configure some optional parameters in the
		// constructor. You also need to manually specify the default object, whereas with
		// the builder you can specify it while adding the panel config.
		MenuConfig config = new MenuConfig(false, false, MENU_KEY_MAIN, PaletteConfig, new PanelConfig[] {
			new PanelConfig(MENU_KEY_MAIN, KEY_RESUME, new PanelObjectConfig[] {
				new ButtonConfig(KEY_RESUME, ButtonPrefab, "Play Game", null, delegate(ButtonManager manager) {
					Debug.Log("Play game pressed.");
				}),
				new ButtonConfig(KEY_OPTIONS, ButtonPrefab, "Options", null, delegate(ButtonManager manager) {
					_manager.PushPanel(MENU_KEY_OPTIONS);
				}),
				new ButtonConfig(KEY_EXIT, ButtonPrefab, "Exit", null, delegate(ButtonManager manager) {
					_manager.ExitGame();
				})
			}),
			MenuConfigHelper.StandardOptionsPanel(MENU_KEY_OPTIONS, _manager, ButtonPrefab, SliderPefab, DropdownPrefab, TogglePrefab).Build(),
		});

		CreateMenu(_manager, config);
	}
}
