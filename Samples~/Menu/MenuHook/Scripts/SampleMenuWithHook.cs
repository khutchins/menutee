using Menutee;
using UnityEngine;

public class SampleMenuWithHook : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_OPTIONS = "Options";

	public readonly static string KEY_RESUME = "resume";
	public readonly static string KEY_OPTIONS = "options";
	public readonly static string KEY_EXIT = "exit";
	public readonly static string KEY_BACK = "back";

	public MenuHook HookedMenu;

	private MenuManager _manager;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		MenuConfig.Builder builder = new MenuConfig.Builder(false, false, PaletteConfig);

		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_MAIN)
			.AddPanelObject(
				new ButtonConfig.Builder("do nothing", ButtonPrefab)
					.SetDisplayText("Do Nothing"))
			.AddPanelObject(
				new ButtonConfig.Builder("show other", ButtonPrefab)
					.SetDisplayText("Show Other Menu")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						MenuStack.Shared.PushAndShowMenu(HookedMenu);
					}).Build()), true);

		CreateMenu(_manager, builder);
	}
}
