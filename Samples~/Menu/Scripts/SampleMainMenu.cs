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

	[Tooltip("Prefab for a menu element that just contains text.")]
	public GameObject TextPrefab;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		// This is an example of how you can create a MenuConfig object without a builder.
		// It works fine for when you're using the default options, but gets trickier if you
		// want to conditionally show options or configure some optional parameters in the
		// constructor. You also need to manually specify the default object, whereas with
		// the builder you can specify it while adding the panel config.
		//
		// It's recommended that use the Builder. See SampleInGameMenu for an example.
		MenuConfig config = new MenuConfig(false, true, false, MENU_KEY_MAIN, PaletteConfig, new PanelConfig[] {
			new PanelConfig(MENU_KEY_MAIN, KEY_RESUME, new PanelObjectConfig[] {
				new ButtonConfig(new PanelObjectConfig.InitObject(KEY_RESUME, ButtonPrefab), "Play Game", delegate(ButtonManager manager) {
					Debug.Log("Play game pressed.");
				}),
				new TextConfig(new PanelObjectConfig.InitObject("other_text", TextPrefab), "Just Some Text"),
				new ButtonConfig(new PanelObjectConfig.InitObject(KEY_OPTIONS, ButtonPrefab), "Options", delegate(ButtonManager manager) {
					_manager.PushPanel(MENU_KEY_OPTIONS);
				}),
				new ButtonConfig(new PanelObjectConfig.InitObject(KEY_EXIT, ButtonPrefab), "Exit",  delegate(ButtonManager manager) {
					_manager.ExitGame();
				})
			}),
			MenuConfigHelper.StandardOptionsPanel(MENU_KEY_OPTIONS, _manager, ButtonPrefab, SliderPefab, DropdownPrefab, TogglePrefab).Build(),
		});

		CreateMenu(_manager, config);
	}
}
