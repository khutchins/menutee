using System;
using System.Collections.Generic;
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
		public readonly PanelObjectConfig DefaultSelectable;
		public readonly GameObject PrefabOverride;
		public readonly NavigationType Navigation;
		public readonly Navigation.Mode NavigationMode;
		public readonly Action<List<Selectable>> NavigationCallback;
		public readonly Action<PanelManager, List<Selectable>> PanelNavigationCallback;
		public readonly Action<GameObject, PanelManager> CreationCallback;
		public readonly Action<GameObject, PanelManager> OnDisplayCallback;
		public readonly Action<GameObject, PanelManager> OnDisposeCallback;
		public readonly Func<PanelManager, List<Selectable>, GameObject> DefaultSelectableCallback;

		/// <summary>
		/// Optional per-panel animation used when navigating to this panel. When
		/// set, overrides the menu-level <see cref="MenuConfig.DefaultPanelTransition"/>.
		/// See <see cref="IPanelTransition"/>.
		/// </summary>
		public readonly IPanelTransition Transition;

		public readonly PanelObjectConfig[] PanelObjects;
		[HideInInspector]
		public readonly GameObject[] SupplementalObjects;

		private PanelConfig(string key,
				PanelObjectConfig defaultSelectable,
				PanelObjectConfig[] panelObjects,
				GameObject[] supplementalObjects,
				NavigationType navigation,
				Action<List<Selectable>> navigationCallback,
				GameObject prefabOverride,
				Navigation.Mode mode,
				Action<PanelManager, List<Selectable>> panelNavigationCallback,
				Func<PanelManager, List<Selectable>, GameObject> defaultSelectableCallback,
				Action<GameObject, PanelManager> creationCallback,
				Action<GameObject, PanelManager> onDisplayCallback,
				Action<GameObject, PanelManager> onDisposeCallback,
				IPanelTransition transition) {
			Key = key;
			DefaultSelectable = defaultSelectable;
			PanelObjects = panelObjects;
			SupplementalObjects = supplementalObjects;
			Navigation = navigation;
			NavigationCallback = navigationCallback;
			NavigationMode = mode;
			PanelNavigationCallback = panelNavigationCallback;
			DefaultSelectableCallback = defaultSelectableCallback;
			CreationCallback = creationCallback;
			OnDisplayCallback = onDisplayCallback;
			OnDisposeCallback = onDisposeCallback;
			Transition = transition;

			PrefabOverride = prefabOverride;
		}

		public class Builder {
			private List<PanelObjectConfig> _panelObjectConfigs = new List<PanelObjectConfig>();
			private List<GameObject> _supplementalObjects = new List<GameObject>();
			private GameObject _prefabOverride;
			private string _key;
			private PanelObjectConfig _defaultSelectable;
			private NavigationType _navigation;
			private Navigation.Mode _navigationMode;
			private Action<List<Selectable>> _navigationCallback;
			private Action<PanelManager, List<Selectable>> _panelNavigationCallback;
			private Func<PanelManager, List<Selectable>, GameObject> _defaultSelectableCallback;
			private Action<GameObject, PanelManager> _creationCallback;
			private Action<GameObject, PanelManager> _onDisplayCallback;
			private Action<GameObject, PanelManager> _onDisposeCallback;
			private IPanelTransition _transition;

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
					_defaultSelectable = config;
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
					_defaultSelectable = config;
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
			/// A callback that is called after panel creation. Note that this callback
			/// will occur after the objects in the panel have been created. Useful for
			/// hooking up content of the panel outside of the generated content, like
			/// a title.
			/// </summary>
			public Builder SetCreationCallback(Action<GameObject, PanelManager> creationCallback) {
				_creationCallback = creationCallback;
				return this;
			}

			/// <summary>
			/// A callback that is called whenever a panel is active. First argument
			/// is the panel's gameobject, the second is the PanelManager itself.
			/// </summary>
			public Builder SetOnDisplayCallback(Action<GameObject, PanelManager> onDisplayCallback) {
				_onDisplayCallback = onDisplayCallback;
				return this;
            }

			/// <summary>
			/// Called when this panel is being destroyed. For dynamic panels (those
			/// produced by a PanelGenerator), this fires when the panel is popped
			/// from the stack or when the menu closes while the panel is still up.
			/// Use this to unsubscribe from any events the generator wired up during
			/// creation. Static panels do not dispose during normal operation.
			/// </summary>
			public Builder SetOnDisposeCallback(Action<GameObject, PanelManager> onDisposeCallback) {
				_onDisposeCallback = onDisposeCallback;
				return this;
			}

			/// <summary>
			/// Sets a per-panel animation used when navigating to this panel. Overrides
			/// the menu-level default set via MenuConfig.Builder.SetDefaultPanelTransition.
			/// If neither is set, the panel change is instant.
			/// </summary>
			public Builder SetTransition(IPanelTransition transition) {
				_transition = transition;
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
			[System.Obsolete("Use SetCustomNavigation(Action<PanelManager, List<Selectable>>). That overload also exposes the PanelManager for reading non-generated selectables; this one just ignores it.", false)]
			public Builder SetCustomNavigation(Action<List<Selectable>> navigationCallback) {
				return SetCustomNavigation((PanelManager _, List<Selectable> selectables) => navigationCallback?.Invoke(selectables));
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
				if (_defaultSelectable == null && _panelObjectConfigs.Count > 0) {
					_defaultSelectable = _panelObjectConfigs[0];
				}
				return new PanelConfig(_key, _defaultSelectable,
					_panelObjectConfigs.ToArray(), _supplementalObjects.ToArray(),
					_navigation, _navigationCallback, _prefabOverride, _navigationMode,
					_panelNavigationCallback, _defaultSelectableCallback, _creationCallback,
					_onDisplayCallback, _onDisposeCallback, _transition);
			}
		}
	}
}