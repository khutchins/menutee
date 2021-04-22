using KH;
using KH.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InGameMenu : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_OPTIONS = "Options";
	public readonly static string MENU_KEY_CREDITS = "Credits";

	public readonly static string KEY_RESUME = "resume";
	public readonly static string KEY_OPTIONS = "options";
	public readonly static string KEY_CREDITS = "credits";
	public readonly static string KEY_EXIT = "exit";
	public readonly static string KEY_BACK = "back";
	public readonly static string KEY_SLIDER_TEXT = "slidertext";
	public readonly static string KEY_SLIDER_LOOK = "sliderlook";
	public readonly static string KEY_RESOLUTION = "resolution";
	public readonly static string KEY_QUALITY = "quality";
	public readonly static string KEY_FULLSCREEN = "fullscreen";

	private MenuHelper _manager;

	void Awake() {
		_manager = GetComponent<MenuHelper>();

		MenuConfig.Builder builder = new MenuConfig.Builder(true, true, PaletteConfig);

		builder.AddPanelConfig(new PanelConfig.Builder("main")
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
						_manager.PushMenu(MENU_KEY_OPTIONS);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("restart", ButtonPrefab)
					.SetDisplayText("Restart")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.NewMap();
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
