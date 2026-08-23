using Menutee;
using Ratferences;
using UnityEngine;

/// <summary>
/// Dynamic panel generation example.
/// </summary>
public class CatalogMenu : MenuGenerator {
	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_LIST = "Catalog";
	public readonly static string MENU_KEY_SETTINGS = "Settings";
	public readonly static string ITEM_KEY_PREFIX = "Catalog/";

	[Header("Prefabs")]
	public GameObject ButtonPrefab;
	public GameObject SliderPrefab;
	public GameObject TogglePrefab;
	public GameObject OptionSelectPrefab;
	[SerializeField] private GameObject _textPrefab;

	[Header("References")]
	[Tooltip("Driven by the SliderRefConfig on the settings panel.")]
	public FloatReference Volume;
	[Tooltip("Driven by the ToggleRefConfig on the settings panel. Also gates item descriptions.")]
	public BoolReference ShowDescriptions;
	[Tooltip("Driven by the OptionSelectToggleRefConfig on the settings panel.")]
	public BoolReference CompactMode;
	[Tooltip("Driven by the OptionSelectRefConfig on the settings panel.")]
	public IntReference Quality;

	[Header("Appearance")]
	public PaletteConfig PaletteConfig;

	private MenuManager _manager;

	private struct CatalogEntry {
		public string Name;
		public string Description;
		public CatalogEntry(string name, string description) { Name = name; Description = description; }
	}

	private static readonly CatalogEntry[] ENTRIES = new CatalogEntry[] {
		new CatalogEntry("Iron Sword", "A sword."),
		new CatalogEntry("Healing Potion", "Heals you."),
		new CatalogEntry("Wooden Shield", "Shields you."),
		new CatalogEntry("Mystery Tome", "Entombs you."),
	};

	private void Awake() {
		_manager = GetComponent<MenuManager>();

		MenuConfig.Builder builder = new MenuConfig.Builder(true, true).SetDefaultPaletteConfig(PaletteConfig);
		MenuAttributes attr = MenuAttributes.StandardPauseMenu();
		attr.cursorLockMode = CursorLockMode.None;
		builder.SetMenuAttributes(attr);

		builder.AddPanelConfig(new PanelConfig.Builder(MENU_KEY_MAIN)
			.AddPanelObject(
				new ButtonConfig.Builder("browse", ButtonPrefab)
					.SetDisplayText("Browse Catalog")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PushPanel(MENU_KEY_LIST);
					}), true)
			.AddPanelObject(
				new ButtonConfig.Builder("settings", ButtonPrefab)
					.SetDisplayText("Settings")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PushPanel(MENU_KEY_SETTINGS);
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("close", ButtonPrefab)
					.SetDisplayText("Close")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.ExitMenu();
					})),
			mainPanel: true);

		// Produces the catalog list menu when Catalog is pushed.
		builder.AddPanelGenerator(
			delegate (string key) { return key == MENU_KEY_LIST; },
			delegate (string key, PanelGeneratorContext ctx) {

				foreach (CatalogEntry entry in ENTRIES) {
					CatalogEntry captured = entry;
					string itemKey = ITEM_KEY_PREFIX + captured.Name;
					ctx.AddPanelGenerator(
						delegate (string k) { return k == itemKey; },
						delegate (string k, PanelGeneratorContext _) {
							return BuildItemPanel(k, captured);
						});
				}

				PanelConfig.Builder panel = new PanelConfig.Builder(key);
				bool first = true;
				foreach (CatalogEntry entry in ENTRIES) {
					string itemKey = ITEM_KEY_PREFIX + entry.Name;
					string displayName = entry.Name;
					panel.AddPanelObject(
						new ButtonConfig.Builder(entry.Name, ButtonPrefab)
							.SetDisplayText(displayName)
							.SetButtonPressedHandler(delegate (ButtonManager m) {
								_manager.PushPanel(itemKey);
							}),
						first);
					first = false;
				}
				panel.AddPanelObject(
					new ButtonConfig.Builder("back", ButtonPrefab)
						.SetDisplayText("Back")
						.SetButtonPressedHandler(delegate (ButtonManager m) {
							_manager.PopPanel();
						}));
				panel.SetOnDisposeCallback(delegate (GameObject go, PanelManager pm) {
				});
				return panel.Build();
			});

		// Settings panel, generated on demand like the catalog list. An example for
		// using references in a dynamic panel. It should just work, with v6.
		builder.AddPanelGenerator(
			delegate (string key) { return key == MENU_KEY_SETTINGS; },
			delegate (string key, PanelGeneratorContext ctx) {
				return BuildSettingsPanel(key);
			});

		CreateMenu(_manager, builder);
	}

	private PanelConfig BuildSettingsPanel(string key) {
		return new PanelConfig.Builder(key)
			.AddPanelObject(
				new SliderRefConfig.Builder("volume", SliderPrefab, 0f, 1f, Volume)
					.SetDisplayText("Volume"), true)
			.AddPanelObject(
				new ToggleRefConfig.Builder("descriptions", TogglePrefab, ShowDescriptions)
					.SetDisplayText("Show Descriptions"))
			.AddPanelObject(
				new OptionSelectToggleRefConfig.Builder("compact", OptionSelectPrefab, "Off", "On", CompactMode)
					.SetDisplayText("Compact Mode"))
			.AddPanelObject(
				new OptionSelectRefConfig.Builder("quality", OptionSelectPrefab, Quality)
					.AddOptionString("Low")
					.AddOptionString("Medium")
					.AddOptionString("High")
					.SetDisplayText("Quality"))
			.AddPanelObject(
				new ButtonConfig.Builder("defaults", ButtonPrefab)
					.SetDisplayText("Restore Defaults")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						Volume.Value = 0.75f;
						ShowDescriptions.Value = true;
						CompactMode.Value = false;
						Quality.Value = 1;
					}))
			.AddPanelObject(
				new ButtonConfig.Builder("back", ButtonPrefab)
					.SetDisplayText("Back")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PopPanel();
					}))
			.Build();
	}

	private PanelConfig BuildItemPanel(string key, CatalogEntry entry) {
		PanelConfig.Builder builder = new PanelConfig.Builder(key)
			.AddPanelObject(new TextConfig.Builder("name", _textPrefab, entry.Name));
		if (ShowDescriptions.Value) {
			builder.AddPanelObject(new TextConfig.Builder("description", _textPrefab, entry.Description));
		}
		return builder
			.AddPanelObject(
				new ButtonConfig.Builder("back", ButtonPrefab)
					.SetDisplayText("Back")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.PopPanel();
					}), true)
			.SetOnDisposeCallback(delegate (GameObject go, PanelManager pm) {
			})
			.Build();
	}
}
