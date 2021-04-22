using KH;
using KH.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenu : MenuGenerator {

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

		MenuConfig config = new MenuConfig(false, false, MENU_KEY_MAIN, PaletteConfig, new PanelConfig[] {
			new PanelConfig(MENU_KEY_MAIN, KEY_RESUME, new PanelObjectConfig[] {
				new ButtonConfig(KEY_RESUME, ButtonPrefab, "Play Game", null, delegate(ButtonManager manager) {
					_manager.NewMap();
				}),
				new ButtonConfig(KEY_OPTIONS, ButtonPrefab, "Options", null, delegate(ButtonManager manager) {
					_manager.PushMenu(MENU_KEY_OPTIONS);
				}),
				new ButtonConfig(KEY_CREDITS, ButtonPrefab, "Credits", null, delegate(ButtonManager manager) {
					_manager.PushMenu(MENU_KEY_CREDITS);
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
