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
		public readonly PaletteConfig PaletteConfig;
		public readonly GameObject Prefab;

		public PanelObjectConfig(string key, GameObject prefab, Action<GameObject> creationCallback, PaletteConfig paletteConfig) {
			Key = key;
			CreationCallback = creationCallback;
			PaletteConfig = paletteConfig;
			Prefab = prefab;
		}

		public abstract GameObject Create(GameObject parent);

		public abstract class Builder<TObject, TBuilder> where TObject : PanelObjectConfig where TBuilder : Builder<TObject, TBuilder> {
			protected string _key;
			protected System.Action<GameObject> _creationCallback;
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

			public TBuilder SetCreationCallback(System.Action<GameObject> creationCallback) {
				_creationCallback = creationCallback;
				return _builderInstance;
			}

			public abstract TObject Build();
		}
	}
}