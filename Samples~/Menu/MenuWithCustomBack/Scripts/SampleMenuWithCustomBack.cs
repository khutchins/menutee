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

		MenuConfig.Builder builder = new MenuConfig.Builder(false, false).SetDefaultPaletteConfig(PaletteConfig);

		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_MAIN)
			.AddFocusedElementChangedCallback(change => {
				Debug.LogFormat("Main panel focus: {0} -> {1}",
					change.Previous.HasFocus ? change.Previous.Key : "(none)",
					change.Current.HasFocus ? change.Current.Key : "(none)");
			})
			.AddPanelObject(
				new ButtonConfig.Builder("show other", ButtonPrefab)
					.SetDisplayText("Show Other Menu")
					.AddFocusChangedCallback(change => {
						Debug.LogFormat("'Show Other Menu' focused: {0}", change.IsFocused);
					})
					.SetButtonPressedHandler(delegate (ButtonManager manager) {
						_manager.PushPanel("Other");
					}).Build()),
			true);

		builder.AddPanelConfig(new PanelConfig.Builder("Other")
			.SetPrefabOverride(CustomBackPanel)
			// Opt the prefab-baked back button into the focus system. Creation
			// callback runs after instantiation, so pm.BackSelectable exists.
			.SetCreationCallback((panelObj, manager) => {
				SamplePanelManager pm = manager as SamplePanelManager;
				manager.RegisterFocusSource(pm.BackSelectable, KEY_BACK);
			})
			.AddFocusedElementChangedCallback(change => {
				Debug.LogFormat("Other panel focus: {0} -> {1}",
					change.Previous.HasFocus ? change.Previous.Key : "(none)",
					change.Current.HasFocus ? change.Current.Key : "(none)");
			})
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
