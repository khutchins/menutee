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
		public GameObject SliderPefab;
		public GameObject TogglePrefab;
		public GameObject DropdownPrefab;

		[Header("Appearance")]
		[Tooltip("Palette to be used for overriding selected, highlighted, etc. elements states.")]
		public PaletteConfig PaletteConfig;

		public Dictionary<string, GameObject> PanelDictionary = new Dictionary<string, GameObject>();
		public Dictionary<string, Dictionary<string, GameObject>> PanelObjectDictionary = new Dictionary<string, Dictionary<string, GameObject>>();

		public void CreateMenu(MenuManager helper, MenuConfig.Builder menuConfigBuilder) {
			CreateMenu(helper, menuConfigBuilder.Build());
		}

		public void CreateMenu(MenuManager helper, MenuConfig menuConfig) {
			helper.MenuConfig = menuConfig;
			List<PanelManager> panels = new List<PanelManager>();
			foreach (PanelConfig panel in menuConfig.PanelConfigs) {
				PanelManager manager = CreatePanel(Parent, panel, menuConfig);
				panels.Add(manager);
			}
			helper.Panels = panels.ToArray();
		}

		public PanelManager CreatePanel(GameObject parent, PanelConfig config, MenuConfig menuConfig) {
			GameObject prefab = config.PrefabOverride == null ? PanelPrefab : config.PrefabOverride;
			GameObject panel = Instantiate(prefab, parent.transform);
			panel.name = config.Key;
			PanelManager manager = panel.AddComponent<PanelManager>();
			manager.Key = config.Key;
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

			// Use this to set up nav menu. Right now it assumes layout is vertical.
			List<Selectable> selectableObjects = new List<Selectable>();

			foreach (PanelObjectConfig objConfig in config.PanelObjects) {
				GameObject go = objConfig.Create(panel);
				UIElementManager elementManager = go.GetComponentInChildren<UIElementManager>();
				elementManager.SetColors(menuConfig.PaletteConfig);
				if (elementManager.SelectableObject != null && elementManager.SelectableObject.GetComponent<Selectable>() != null) {
					selectableObjects.Add(elementManager.SelectableObject.GetComponent<Selectable>());
				}
				objConfig.CreationCallback?.Invoke(go);
				dict[objConfig.Key] = go;
				if (objConfig.Key == config.DefaultSelectableKey) {
					manager.DefaultInput = elementManager;
				}
			}

			// Hook up navigation with elements with selectable objects.
			if (config.Navigation == PanelConfig.NavigationType.Custom) {
				config.NavigationCallback?.Invoke(selectableObjects);
			} else {
				for (int i = 0; i < selectableObjects.Count; i++) {
					// Make new one to avoid potential property strangeness.
					Navigation navigation = new Navigation();
					navigation.mode = Navigation.Mode.Explicit;
					if (config.Navigation == PanelConfig.NavigationType.Horizontal) {
						navigation.selectOnUp = null;
						navigation.selectOnDown = null;
						navigation.selectOnLeft = i > 0 ? selectableObjects[i - 1] : null;
						navigation.selectOnRight = i < selectableObjects.Count - 1 ? selectableObjects[i + 1] : null;
					} else {
						navigation.selectOnUp = i > 0 ? selectableObjects[i - 1] : null;
						navigation.selectOnDown = i < selectableObjects.Count - 1 ? selectableObjects[i + 1] : null;
						navigation.selectOnLeft = null;
						navigation.selectOnRight = null;
					}
					selectableObjects[i].navigation = navigation;
				}
			}

			PanelObjectDictionary[config.Key] = dict;
			return manager;
		}

		protected GameObject CreatePanelObject(GameObject panel, PanelObjectConfig config) {
			return config.Create(panel);
		}
	}
}