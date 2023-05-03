using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
	public class MenuGenerator : MonoBehaviour {

		[Header("Container")]
		[Tooltip("Canvas element to be used as the container for panels.")]
		public GameObject Parent;

		[Header("Prefabs")]
		[Tooltip("Panel prefab that menu prefabs are placed in.")]
		public GameObject PanelPrefab;
		public GameObject ButtonPrefab;
		public GameObject SliderPrefab;
		public GameObject TogglePrefab;
		public GameObject DropdownPrefab;

		[Header("Appearance")]
		[Tooltip("Palette to be used for overriding selected, highlighted, etc. elements states.")]
		public PaletteConfig PaletteConfig;

		public Dictionary<string, GameObject> PanelDictionary = new Dictionary<string, GameObject>();
		public Dictionary<string, Dictionary<string, GameObject>> PanelObjectDictionary = new Dictionary<string, Dictionary<string, GameObject>>();

		public void CreateMenu(MenuManager helper, MenuConfig.Builder menuConfigBuilder) {
			if (menuConfigBuilder == null) {
				Debug.LogError("MenuConfig.Builder passed in CreateMenu is null. Menu generation will not proceed.");
				return;
			}
			CreateMenu(helper, menuConfigBuilder.Build());
		}

		public void CreateMenu(MenuManager helper, MenuConfig menuConfig) {
			if (helper == null) {
				Debug.LogError("MenuManager passed in CreateMenu is null. Menu generation will not proceed.");
				return;
			}
			if (menuConfig == null) {
				Debug.LogError("MenuConfig passed in CreateMenu is null. Menu generation will not proceed.");
				return;
			}
			helper.MenuConfig = menuConfig;
			List<PanelManager> panels = new List<PanelManager>();
			foreach (PanelConfig panel in menuConfig.PanelConfigs) {
				PanelManager manager = CreatePanel(helper, Parent, panel, menuConfig);
				panels.Add(manager);
			}
			helper.Panels = panels.ToArray();
		}

		public PanelManager CreatePanel(MenuManager menuManager, GameObject parent, PanelConfig config, MenuConfig menuConfig) {
			GameObject prefab = config.PrefabOverride == null ? PanelPrefab : config.PrefabOverride;
			GameObject panel = Instantiate(prefab, parent.transform);
			panel.name = config.Key;
			PanelManager manager = panel.GetComponent<PanelManager>();
			if (manager == null) {
				manager = panel.AddComponent<PanelManager>();
			}
			manager.Key = config.Key;
			manager.Manager = menuManager;
			manager.Config = config;
			PanelDictionary.Add(config.Key, panel);

			if (config.SupplementalObjects != null) {
				List<GameObject> supplementalObjects = new List<GameObject>();
				foreach (GameObject supPrefab in config.SupplementalObjects) {
					GameObject supObj = Instantiate(supPrefab, parent.transform);
					supplementalObjects.Add(supObj);
				}
				manager.OtherObjects = supplementalObjects.ToArray();
			}

			Dictionary<string, GameObject> dict = new Dictionary<string, GameObject>();
			List<Selectable> selectableObjects = new List<Selectable>();
			List<UIElementManager> panelElements = new List<UIElementManager>();

			foreach (PanelObjectConfig objConfig in config.PanelObjects) {
				GameObject go = objConfig.Create(manager.Parent == null ? panel : manager.Parent);

				UIElementManager elementManager = go.GetComponentInChildren<UIElementManager>();
				// UIElementManager can be null, for instance for an element with no interaction
				// (e.g. just text).
				if (elementManager != null) {
					panelElements.Add(elementManager);
					elementManager.PanelObjectConfig = objConfig;
					elementManager.SetColors(objConfig.PaletteConfig ? objConfig.PaletteConfig : menuConfig.PaletteConfig);
					if (elementManager.SelectableObject != null && elementManager.SelectableObject.GetComponent<Selectable>() != null) {
						selectableObjects.Add(elementManager.SelectableObject.GetComponent<Selectable>());
					}
				}
				objConfig.CreationCallback?.Invoke(go);
				dict[objConfig.Key] = go;
				if (objConfig.Key == config.DefaultSelectableKey) {
					if (elementManager != null) {
						manager.DefaultInput = elementManager.SelectableObject;
					} else {
						Debug.LogWarningFormat("Attempting to set the default selectable to non-selectable object {0}. Either add a UIElementManager subclass to the object, or change the default object.", go);
					}
				}
			}
			manager.ElementManagers = panelElements.ToArray();

			// Set the default selectable to the callback value, if it exists and
			// returns a valid object.
			GameObject callbackSelectable = config.DefaultSelectableCallback?.Invoke(manager, new List<Selectable>(selectableObjects));
			if (callbackSelectable != null) {
				manager.DefaultInput = callbackSelectable;
			}

			// Hook up navigation with elements with selectable objects.
			if (config.Navigation == PanelConfig.NavigationType.Custom) {
				// Create new lists here just in case I end up wanting to use them later or
				// they declare two navigation callbacks and stomp all over the list with
				// the first.
				config.NavigationCallback?.Invoke(new List<Selectable>(selectableObjects));
				config.PanelNavigationCallback?.Invoke(manager, new List<Selectable>(selectableObjects));
			} else if (config.Navigation == PanelConfig.NavigationType.AutomaticUnity) {
				foreach (Selectable selectable in selectableObjects) {
					Navigation navigation = new Navigation();
					navigation.mode = config.NavigationMode;
					selectable.navigation = navigation;
				}
			} else if (config.Navigation == PanelConfig.NavigationType.Horizontal) {
				MenuGenerator.SetHorizontalNavigation(selectableObjects);
			} else {
				MenuGenerator.SetVerticalNavigation(selectableObjects);
			}

			PanelObjectDictionary[config.Key] = dict;
			config.CreationCallback?.Invoke(panel, manager);
			return manager;
		}

		protected GameObject CreatePanelObject(GameObject panel, PanelObjectConfig config) {
			return config.Create(panel);
		}

		/// <summary>
		/// Helper method for setting horizontal navigation on a list
		/// of selectables. Useful if you have some non-dynamic button
		/// on the panel you want to nav to without writing the for loop
		/// yourself.
		/// </summary>
		public static void SetHorizontalNavigation(List<Selectable> selectableObjects, bool allowLoop = false) {
			for (int i = 0; i < selectableObjects.Count; i++) {
				// Make new one to avoid potential property strangeness.
				Navigation navigation = new Navigation();
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = null;
				navigation.selectOnDown = null;
				navigation.selectOnLeft = i > 0 ? selectableObjects[i - 1] : (allowLoop ? selectableObjects[selectableObjects.Count - 1] : null);
				navigation.selectOnRight = i < selectableObjects.Count - 1 ? selectableObjects[i + 1] : (allowLoop ? selectableObjects[0] : null);
				selectableObjects[i].navigation = navigation;
			}
		}

		/// <summary>
		/// Helper method for setting vertical navigation on a list
		/// of selectables. Useful if you have some non-dynamic button
		/// on the panel you want to nav to without writing the for loop
		/// yourself.
		/// </summary>
		public static void SetVerticalNavigation(List<Selectable> selectableObjects, bool allowLoop = false) {
			for (int i = 0; i < selectableObjects.Count; i++) {
				// Make new one to avoid potential property strangeness.
				Navigation navigation = new Navigation();
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = i > 0 ? selectableObjects[i - 1] : (allowLoop ? selectableObjects[selectableObjects.Count - 1] : null);
				navigation.selectOnDown = i < selectableObjects.Count - 1 ? selectableObjects[i + 1] : (allowLoop ? selectableObjects[0] : null);
				navigation.selectOnLeft = null;
				navigation.selectOnRight = null;
				selectableObjects[i].navigation = navigation;
			}
		}
	}
}