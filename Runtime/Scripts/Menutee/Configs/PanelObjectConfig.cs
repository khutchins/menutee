using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public abstract class PanelObjectConfig {
		/// <summary>
		/// Optional key that uniquely identifies an object within a panel (used for
		/// PanelObjectDictionary lookups). Should be unique within that scope. Null
		/// when the element was created without a key.
		/// </summary>
		public readonly string Key;
		public readonly Action<GameObject> CreationCallback;
		public readonly Action<GameObject, UIElementManager> OnDisplayCallback;
		/// <summary>
		/// Callbacks invoked when this element becomes, or stops being, the menu's
		/// currently focused element.
		/// </summary>
		public readonly List<Action<ElementFocusChange>> FocusChangedCallbacks;
		/// <summary>
		/// Callbacks invoked whenever this element's interaction state changes.
		/// Unlike <see cref="FocusChangedCallbacks"/>, this reflects only this
		/// element's own selected/highlighted flags and is unaffected by other
		/// elements.
		/// </summary>
		public readonly List<Action<InteractionStateChange>> InteractionStateCallbacks;
		public readonly PaletteConfig PaletteConfig;
		public readonly PaletteConfigReference PaletteReference;
		public readonly GameObject Prefab;
		protected string ObjectName => !string.IsNullOrEmpty(Key) ? Key
			: (Prefab != null ? Prefab.name : GetType().Name);

		public PanelObjectConfig(InitObject initObject) {
			Key = initObject.Key;
			CreationCallback = initObject.CreationCallback;
			OnDisplayCallback = initObject.OnDisplayCallback;
			FocusChangedCallbacks = initObject.FocusChangedCallbacks ?? new List<Action<ElementFocusChange>>();
			InteractionStateCallbacks = initObject.InteractionStateCallbacks ?? new List<Action<InteractionStateChange>>();
			PaletteConfig = initObject.PaletteConfig;
			PaletteReference = initObject.PaletteReference;
			Prefab = initObject.Prefab;
		}

		public abstract GameObject Create(GameObject parent);

		public class InitObject {
			public readonly string Key;
			public readonly Action<GameObject> CreationCallback;
			public readonly Action<GameObject, UIElementManager> OnDisplayCallback;
			public readonly List<Action<ElementFocusChange>> FocusChangedCallbacks;
			public readonly List<Action<InteractionStateChange>> InteractionStateCallbacks;
			public readonly PaletteConfig PaletteConfig;
            public readonly PaletteConfigReference PaletteReference;
            public readonly GameObject Prefab;

			public InitObject(string key, GameObject prefab, Action<GameObject> creationCallback = null,
					Action<GameObject, UIElementManager> onDisplayCallback = null, PaletteConfig paletteConfig = null,
					PaletteConfigReference paletteReference = null,
					List<Action<ElementFocusChange>> focusChangedCallbacks = null,
					List<Action<InteractionStateChange>> interactionStateCallbacks = null) {
				Key = key;
				CreationCallback = creationCallback;
				OnDisplayCallback = onDisplayCallback;
				FocusChangedCallbacks = focusChangedCallbacks;
				InteractionStateCallbacks = interactionStateCallbacks;
				PaletteConfig = paletteConfig;
				PaletteReference = paletteReference;
				Prefab = prefab;
			}
		}

		public abstract class Builder<TObject, TBuilder> where TObject : PanelObjectConfig where TBuilder : Builder<TObject, TBuilder> {
			protected string _key;
			protected Action<GameObject> _creationCallback;
			protected Action<GameObject, UIElementManager> _onDisplayCallback;
			protected List<Action<ElementFocusChange>> _focusChangedCallbacks = new List<Action<ElementFocusChange>>();
			protected List<Action<InteractionStateChange>> _interactionStateCallbacks = new List<Action<InteractionStateChange>>();
			protected GameObject _prefab;
			protected PaletteConfig _paletteConfig;
			protected PaletteConfigReference _paletteReference;
			protected readonly TBuilder _builderInstance;

			public Builder(string key, GameObject prefab) {
				_key = key;
				_prefab = prefab;
				_builderInstance = (TBuilder)this;
			}

			public Builder(GameObject prefab) : this(null, prefab) {
			}

			public TBuilder SetPaletteConfigOverride(PaletteConfig config) {
				_paletteConfig = config;
				return _builderInstance;
			}

			public TBuilder SetPaletteReferenceOverride(PaletteConfigReference reference) {
				_paletteReference = reference;
				return _builderInstance;
			}

			public TBuilder SetCreationCallback(Action<GameObject> creationCallback) {
				_creationCallback = creationCallback;
				return _builderInstance;
			}

			public TBuilder SetOnDisplayCallback(Action<GameObject, UIElementManager> onDisplayCallback) {
				_onDisplayCallback = onDisplayCallback;
				return _builderInstance;
            }

			/// <summary>
			/// Adds a callback invoked when this element becomes, or stops being, the
			/// menu's currently focused element. The bool is true on gaining focus,
			/// false on losing it.
			/// </summary>
			public TBuilder AddFocusChangedCallback(Action<ElementFocusChange> onFocusChanged) {
				_focusChangedCallbacks.Add(onFocusChanged);
				return _builderInstance;
			}

			/// <summary>
			/// Adds a callback invoked whenever this element's raw local interaction
			/// state (selected/highlighted) changes, with the previous and next
			/// <see cref="InteractionState"/>.
			/// </summary>
			public TBuilder AddInteractionStateCallback(Action<InteractionStateChange> onInteractionStateChanged) {
				_interactionStateCallbacks.Add(onInteractionStateChanged);
				return _builderInstance;
			}

			public InitObject BuildInitObject() {
				return new InitObject(_key, _prefab, _creationCallback, _onDisplayCallback, _paletteConfig, _paletteReference,
					_focusChangedCallbacks, _interactionStateCallbacks);
			}

			public abstract TObject Build();
		}
	}
}