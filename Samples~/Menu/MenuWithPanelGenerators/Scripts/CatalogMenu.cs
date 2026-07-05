using Menutee;
using UnityEngine;

/// <summary>
/// Dynamic panel generation example.
/// </summary>
public class CatalogMenu : MenuGenerator {
	public readonly static string MENU_KEY_MAIN = "Main";
	public readonly static string MENU_KEY_LIST = "Catalog";
	public readonly static string ITEM_KEY_PREFIX = "Catalog/";

	[Header("Prefabs")]
	public GameObject ButtonPrefab;
	[SerializeField] private GameObject _textPrefab;

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
				new ButtonConfig.Builder("close", ButtonPrefab)
					.SetDisplayText("Close")
					.SetButtonPressedHandler(delegate (ButtonManager m) {
						_manager.ExitMenu();
					})),
			mainPanel: true);

		// Permanent generator: produces the catalog list panel when "Catalog" is
		// pushed. While building the list, it also registers scoped generators
		// for each of its items, so item detail panels are only generatable
		// while the list panel is on the stack.
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

		CreateMenu(_manager, builder);
	}

	private PanelConfig BuildItemPanel(string key, CatalogEntry entry) {
		return new PanelConfig.Builder(key)
			.AddPanelObject(new TextConfig.Builder("name", _textPrefab, entry.Name))
			.AddPanelObject(new TextConfig.Builder("description", _textPrefab, entry.Description))
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
