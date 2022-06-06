using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public abstract class PanelObjectConfig {
		/// <summary>
		/// Key that uniquely identifies an object within
		/// a panel. Should be unique within that scope.
		/// </summary>
		public readonly string Key;
		public readonly Action<GameObject> CreationCallback;
		public readonly Action<GameObject, UIElementManager> OnDisplayCallback;
		public readonly PaletteConfig PaletteConfig;
		public readonly GameObject Prefab;

		public PanelObjectConfig(InitObject initObject) {
			Key = initObject.Key;
			CreationCallback = initObject.CreationCallback;
			OnDisplayCallback = initObject.OnDisplayCallback;
			PaletteConfig = initObject.PaletteConfig;
			Prefab = initObject.Prefab;
		}

		public abstract GameObject Create(GameObject parent);

		public class InitObject {
			public readonly string Key;
			public readonly Action<GameObject> CreationCallback;
			public readonly Action<GameObject, UIElementManager> OnDisplayCallback;
			public readonly PaletteConfig PaletteConfig;
			public readonly GameObject Prefab;

			public InitObject(string key, GameObject prefab, Action<GameObject> creationCallback = null, 
					Action<GameObject, UIElementManager> onDisplayCallback = null, PaletteConfig paletteConfig = null) {
				Key = key;
				CreationCallback = creationCallback;
				OnDisplayCallback = onDisplayCallback;
				PaletteConfig = paletteConfig;
				Prefab = prefab;
			}
		}

		public abstract class Builder<TObject, TBuilder> where TObject : PanelObjectConfig where TBuilder : Builder<TObject, TBuilder> {
			protected string _key;
			protected Action<GameObject> _creationCallback;
			protected Action<GameObject, UIElementManager> _onDisplayCallback;
			protected GameObject _prefab;
			protected PaletteConfig _paletteConfig;
			protected readonly TBuilder _builderInstance;

			public Builder(string key, GameObject prefab) {
				_key = key;
				_prefab = prefab;
				_builderInstance = (TBuilder)this;
			}

			public TBuilder SetPaletteConfigOverride(PaletteConfig config) {
				_paletteConfig = config;
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

			public InitObject BuildInitObject() {
				return new InitObject(_key, _prefab, _creationCallback, _onDisplayCallback, _paletteConfig);
			}

			public abstract TObject Build();
		}
	}
}