using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
	public class PanelConfig {

		public enum NavigationType {
			Vertical,
			Horizontal,
			Custom,
			AutomaticUnity
		};

		/// <summary>
		/// Used to reference the panels from other panels or do
		/// lookups in the panel dictionary.
		/// </summary>
		public readonly string Key;
		public readonly string DefaultSelectableKey;
		public readonly bool HorizontalMenu;
		public readonly GameObject PrefabOverride;
		public readonly NavigationType Navigation;
		public readonly Navigation.Mode NavigationMode;
		public readonly Action<List<Selectable>> NavigationCallback;
		public readonly Action<PanelManager, List<Selectable>> PanelNavigationCallback;
		public readonly Func<PanelManager, List<Selectable>, GameObject> DefaultSelectableCallback;

		public readonly PanelObjectConfig[] PanelObjects;
		[HideInInspector]
		public readonly GameObject[] SupplementalObjects;

		public PanelConfig(string key, 
				string defaultSelectableKey, 
				PanelObjectConfig[] panelObjects, 
				GameObject[] supplementalObjects = null, 
				NavigationType navigation = NavigationType.Vertical,
				Action<List<Selectable>> navigationCallback = null, 
				GameObject prefabOverride = null, 
				Navigation.Mode mode = UnityEngine.UI.Navigation.Mode.Explicit,
				Action<PanelManager, List<Selectable>> panelNavigationCallback = null,
				Func<PanelManager, List<Selectable>, GameObject> defaultSelectableCallback = null) {
			Key = key;
			DefaultSelectableKey = defaultSelectableKey;
			PanelObjects = panelObjects;
			SupplementalObjects = supplementalObjects;
			Navigation = navigation;
			NavigationCallback = navigationCallback;
			NavigationMode = mode;
			PanelNavigationCallback = panelNavigationCallback;
			DefaultSelectableCallback = defaultSelectableCallback;

			PrefabOverride = prefabOverride;
		}

		public class Builder {
			private List<PanelObjectConfig> _panelObjectConfigs = new List<PanelObjectConfig>();
			private List<GameObject> _supplementalObjects = new List<GameObject>();
			private GameObject _prefabOverride;
			private string _key;
			private string _defaultSelectableKey;
			private NavigationType _navigation;
			private Navigation.Mode _navigationMode;
			private Action<List<Selectable>> _navigationCallback;
			private Action<PanelManager, List<Selectable>> _panelNavigationCallback;
			private Func<PanelManager, List<Selectable>, GameObject> _defaultSelectableCallback;

			public Builder(string key) {
				_key = key;
			}

			public Builder AddSupplementalObject(GameObject obj) {
				_supplementalObjects.Add(obj);
				return this;
			}

			public Builder AddSupplementalObjects(IEnumerable<GameObject> objs) {
				_supplementalObjects.AddRange(objs);
				return this;
			}

			public Builder AddPanelObject(PanelObjectConfig config, bool defaultObject = false) {
				_panelObjectConfigs.Add(config);
				if (defaultObject) {
					_defaultSelectableKey = config.Key;
				}
				return this;
			}

			public Builder AddPanelObject<T, U>(PanelObjectConfig.Builder<T, U> configBuilder, bool defaultObject = false)
					where T : PanelObjectConfig where U : PanelObjectConfig.Builder<T, U> {
				return AddPanelObject(configBuilder.Build(), defaultObject);
			}

			public Builder InsertPanelObject(PanelObjectConfig config, int index, bool defaultObject = false) {
				_panelObjectConfigs.Insert(index, config);
				if (defaultObject) {
					_defaultSelectableKey = config.Key;
				}
				return this;
			}

			public Builder InsertPanelObject<T, U>(PanelObjectConfig.Builder<T, U> configBuilder, int index, bool defaultObject = false) 
					where T : PanelObjectConfig where U : PanelObjectConfig.Builder<T, U> {
				return InsertPanelObject(configBuilder.Build(), index, defaultObject);
			}

			/// <summary>
			/// Overrides the panel prefab to use the one set here. Otherwise, it will
			/// use the one set in the MenuGenerator instance.
			/// </summary>
			/// <param name="prefabOverride">Prefab to use for the panel.</param>
			public Builder SetPrefabOverride(GameObject prefabOverride) {
				_prefabOverride = prefabOverride;
				return this;
			}

			/// <summary>
			/// Calls to get the default selectable object at creation time. Allows
			/// non-programmatically generated selectables to be the default. If unset,
			/// will use the one specified when adding or inserting a panel object, or the
			/// first element if none was set.
			/// </summary>
			public Builder SetDefaultSelectableCallback(Func<PanelManager, List<Selectable>, GameObject> defaultSelectableCallback) {
				_defaultSelectableCallback = defaultSelectableCallback;
				return this;
			}

			/// <summary>
			/// Specifies that navigation between selectable elements will use the left/right inputs.
			/// This will not alter visual appearance, as that is determined by the panel prefab.
			/// </summary>
			public Builder SetHorizontalNavigation() {
				_navigation = NavigationType.Horizontal;
				return this;
			}

			/// <summary>
			/// Specifies that navigation between selectable elements will use the up/down inputs.
			/// This will not alter visual appearance, as that is determined by the panel prefab.
			/// </summary>
			public Builder SetVerticalNavigation() {
				_navigation = NavigationType.Vertical;
				return this;
			}

			/// <summary>
			/// Allows the selectables to have their navigation specified by the provided callback.
			/// The selectable list is the order in which the panel objects were added, ignoring unselectable elements.
			/// </summary>
			public Builder SetCustomNavigation(Action<List<Selectable>> navigationCallback) {
				_navigation = NavigationType.Custom;
				_navigationCallback = navigationCallback;
				return this;
			}

			/// <summary>
			/// Allows the selectables to have their navigation specified by the provided callback.
			/// PanelManager is a reference to the created panel, and will allow you to read non-dynamic elements
			/// to include in the navigation.
			/// The selectable list is the order in which the panel objects were added, ignoring unselectable elements.
			/// </summary>
			public Builder SetCustomNavigation(Action<PanelManager, List<Selectable>> navigationCallback) {
				_navigation = NavigationType.Custom;
				_panelNavigationCallback = navigationCallback;
				return this;
			}

			/// <summary>
			/// Selectables will navigate using Unity's automatic navigation mode. This is not recommended, as it tends to give unnatural results.
			/// </summary>
			public Builder SetAutomaticUnityNavigation(Navigation.Mode mode) {
				_navigation = NavigationType.AutomaticUnity;
				_navigationMode = mode;
				return this;
			}

			public PanelConfig Build() {
				if (_defaultSelectableKey == null && _panelObjectConfigs.Count > 0) {
					_defaultSelectableKey = _panelObjectConfigs[0].Key;
				}
				return new PanelConfig(_key, _defaultSelectableKey, 
					_panelObjectConfigs.ToArray(), _supplementalObjects.ToArray(), 
					_navigation, _navigationCallback, _prefabOverride, _navigationMode, 
					_panelNavigationCallback, _defaultSelectableCallback);
			}
		}
	}
}