using Menutee;
using UnityEngine;

/// <summary>
/// Demonstrates Menutee's transition system in a single menu:
///
/// * Menu visibility is asymmetric — it slides up from the bottom on open and
///   fades out on close.
/// * Open sequencing is Ordered and close is None: opening plays the menu slide,
///   then the panel's elements cascade in; closing is instant.
/// * The main panel navigates to three sub-panels, each exercising a different
///   panel transition.
/// </summary>
public class SampleInGameMenu : MenuGenerator {

	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_SLIDE = "Slide";
	public readonly static string MENU_KEY_STAGGER = "Stagger";
	public readonly static string MENU_KEY_CROSSFADE = "Crossfade";

	private MenuManager _manager;

	[Tooltip("Container the menu slides on/offscreen. If unset, falls back to the panel container (MenuGenerator.Parent).")]
	[SerializeField] private RectTransform _menuRect;

	void Awake() {
		_manager = GetComponent<MenuManager>();

		MenuAttributes attr = MenuAttributes.StandardPauseMenu();
		attr.cursorLockMode = CursorLockMode.None;

		MenuConfig.Builder builder = new MenuConfig.Builder(true, true, PaletteConfig)
			.SetMenuAttributes(attr)
			// Slide up to open, fade to close.
			.SetMenuTransition(
				new SlideMenuTransition { FromEdge = TransitionEdge.Bottom, Content = _menuRect },
				new FadeMenuTransition())
			// Cascade the panel in after the menu slide; dismiss instantly.
			.SetMenuSequence(MenuPanelSequence.Ordered, MenuPanelSequence.None)
			// Default transition is a slide.
			.SetDefaultPanelTransition(new SlidePanelTransition { SlideDistance = 600f });

		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_MAIN)
			.SetTransition(new StaggeredSlidePanelTransition())
			.AddPanelObject(
				new ButtonConfig.Builder("slide", ButtonPrefab)
					.SetDisplayText("Slide Panel")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PushPanel(MENU_KEY_SLIDE);
					}), true)
			.AddPanelObject(
				new ButtonConfig.Builder("stagger", ButtonPrefab)
					.SetDisplayText("Stagger Panel")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PushPanel(MENU_KEY_STAGGER);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("crossfade", ButtonPrefab)
					.SetDisplayText("Crossfade Panel")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PushPanel(MENU_KEY_CROSSFADE);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("close", ButtonPrefab)
					.SetDisplayText("Close")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.ExitMenu();
					})), true);

		// Uses default SlidePanelTransition.
		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_SLIDE)
			.AddPanelObject(BackButton(), true)
			.AddPanelObject(new ButtonConfig.Builder("a", ButtonPrefab).SetDisplayText("The whole panel"))
			.AddPanelObject(new ButtonConfig.Builder("b", ButtonPrefab).SetDisplayText("slides as one block")));

		// Per-element stagger.
		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_STAGGER)
			.SetTransition(new StaggeredSlidePanelTransition { FromEdge = TransitionEdge.Right })
			.AddPanelObject(BackButton(), true)
			.AddPanelObject(
				new SliderConfig.Builder("slider", SliderPrefab, 0f, 1f, 0.5f)
					.SetDisplayText("Slider"))
			.AddPanelObject(
				new ToggleConfig.Builder("toggle", TogglePrefab)
					.SetDisplayText("Toggle")
					.SetIsOn(false))
			.AddPanelObject(
				new DropdownConfig.Builder("dropdown", DropdownPrefab)
					.SetDisplayText("Dropdown")
					.AddOptionStrings(new[] { "Option A", "Option B", "Option C" })
					.SetDefaultOptionIndex(0)));

		// Crossfade transition.
		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_CROSSFADE)
			.SetTransition(new FadePanelTransition())
			.AddPanelObject(BackButton(), true)
			.AddPanelObject(new ButtonConfig.Builder("a", ButtonPrefab).SetDisplayText("This panel"))
			.AddPanelObject(new ButtonConfig.Builder("b", ButtonPrefab).SetDisplayText("crossfades in")));

		CreateMenu(_manager, builder);
	}

	private ButtonConfig.Builder BackButton() {
		return new ButtonConfig.Builder("back", ButtonPrefab)
			.SetDisplayText("Back")
			.SetButtonPressedHandler(delegate (ButtonManager m) {
				_manager.PopPanel();
			});
	}
}
