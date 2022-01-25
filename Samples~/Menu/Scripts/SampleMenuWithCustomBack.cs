using Menutee;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleMenuWithCustomBack : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_OPTIONS = "Options";

	public readonly static string KEY_RESUME = "resume";
	public readonly static string KEY_OPTIONS = "options";
	public readonly static string KEY_EXIT = "exit";
	public readonly static string KEY_BACK = "back";

	public GameObject CustomBackPanel;

	private MenuManager _manager;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		MenuConfig.Builder builder = new MenuConfig.Builder(false, false, PaletteConfig);

		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_MAIN)
			.AddPanelObject(
				new ButtonConfig.Builder("show other", ButtonPrefab)
					.SetDisplayText("Show Other Menu")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.PushPanel("Other");
					}).Build()), 
			true);

		builder.AddPanelConfig(new PanelConfig.Builder("Other")
			.SetPrefabOverride(CustomBackPanel)
			.SetCustomNavigation((PanelManager manager, List<Selectable> selectables) => {
				SamplePanelManager pm = manager as SamplePanelManager;
				selectables.Add(pm.BackSelectable);
				SetVerticalNavigation(selectables);
			})
			.AddPanelObject(
				new ButtonConfig.Builder("do nothing", ButtonPrefab)
					.SetDisplayText("Show Empty Menu")
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.PushPanel("Third");
					})));

		builder.AddPanelConfig(new PanelConfig.Builder("Third")
			.SetPrefabOverride(CustomBackPanel)
			.SetDefaultSelectableCallback((PanelManager manager, List<Selectable> selectables) => {
				SamplePanelManager pm = manager as SamplePanelManager;
				return pm.BackSelectable.gameObject;
			}));

		CreateMenu(_manager, builder);
	}
}
