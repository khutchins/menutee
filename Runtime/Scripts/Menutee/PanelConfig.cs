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
			Custom
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
		public readonly System.Action<List<Selectable>> NavigationCallback;

		public readonly PanelObjectConfig[] PanelObjects;
		[HideInInspector]
		public readonly GameObject[] SupplementalObjects;

		public PanelConfig(string key, string defaultSelectableKey, PanelObjectConfig[] panelObjects, GameObject[] supplementalObjects = null, NavigationType navigation = NavigationType.Vertical, System.Action<List<Selectable>> navigationCallback = null, GameObject prefabOverride = null) {
			Key = key;
			DefaultSelectableKey = defaultSelectableKey;
			PanelObjects = panelObjects;
			SupplementalObjects = supplementalObjects;
			Navigation = navigation;
			NavigationCallback = navigationCallback;

			PrefabOverride = prefabOverride;
		}

		public class Builder {
			private List<PanelObjectConfig> _panelObjectConfigs = new List<PanelObjectConfig>();
			private List<GameObject> _supplementalObjects = new List<GameObject>();
			private GameObject _prefabOverride;
			private string _key;
			private string _defaultSelectableKey;
			private NavigationType _navigation;
			private System.Action<List<Selectable>> _navigationCallback;

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

			public Builder SetPrefabOverride(GameObject prefabOverride) {
				_prefabOverride = prefabOverride;
				return this;
			}

			/// <summary>
			/// Specifies that navigation between selectable elements will use the left/right inputs.
			/// </summary>
			public Builder SetHorizontalNavigation() {
				_navigation = NavigationType.Horizontal;
				return this;
			}

			/// <summary>
			/// Specifies that navigation between selectable elements will use the up/down inputs.
			/// </summary>
			public Builder SetVerticalNavigation() {
				_navigation = NavigationType.Vertical;
				return this;
			}

			/// <summary>
			/// Allows the selectables to have their navigation specified by the provided callback.
			/// The selectable list is the order in which the panel objects were added, ignoring unselectable elements.
			/// </summary>
			public Builder SetCustomNavigation(System.Action<List<Selectable>> navigationCallback) {
				_navigation = NavigationType.Custom;
				_navigationCallback = navigationCallback;
				return this;
			}

			public PanelConfig Build() {
				if (_defaultSelectableKey == null) {
					_defaultSelectableKey = _panelObjectConfigs[0].Key;
				}
				return new PanelConfig(_key, _defaultSelectableKey, 
					_panelObjectConfigs.ToArray(), _supplementalObjects.ToArray(), 
					_navigation, _navigationCallback, _prefabOverride);
			}
		}
	}
}