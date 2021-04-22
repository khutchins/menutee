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
		public readonly GameObject Prefab;

		public PanelObjectConfig(string key, GameObject prefab, Action<GameObject> creationCallback) {
			Key = key;
			CreationCallback = creationCallback;
			Prefab = prefab;
		}

		public abstract GameObject Create(GameObject parent);

		public abstract class Builder {
			protected string _key;
			protected System.Action<GameObject> _creationCallback;
			protected GameObject _prefab;

			public Builder(string key, GameObject prefab) {
				_key = key;
				_prefab = prefab;
			}

			public Builder SetCreationCallback(System.Action<GameObject> creationCallback) {
				_creationCallback = creationCallback;
				return this;
			}

			public abstract PanelObjectConfig Build();
		}
	}
}