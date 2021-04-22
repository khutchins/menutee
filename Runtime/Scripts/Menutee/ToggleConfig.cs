using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class ToggleConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly bool IsOn;
		public readonly TogglePressedHandler Handler;

		public ToggleConfig(string key, GameObject prefab, string displayText, bool isOn, System.Action<GameObject> creationCallback, TogglePressedHandler handler) 
				: base(key, prefab, creationCallback) {
			DisplayText = displayText;
			IsOn = isOn;
			Handler = handler;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			ToggleManager manager = go.GetComponent<ToggleManager>();
			if (manager == null) {
				Debug.LogWarning("Toggle prefab does not contain ToggleManager. Menu generation will not proceed normally!");
			} else {
				manager.SetToggled(IsOn);
				manager.SetText(DisplayText);
				manager.TogglePressed += Handler;
			}
			return go;
		}

		public new class Builder : PanelObjectConfig.Builder {
			private string _displayText;
			private TogglePressedHandler _handler;
			private bool _isOn;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return this;
			}

			public Builder SetIsOn(bool isOn) {
				_isOn = isOn;
				return this;
			}

			public Builder SetTogglePressedHandler(TogglePressedHandler handler) {
				_handler = handler;
				return this;
			}

			public override PanelObjectConfig Build() {
				return new ToggleConfig(_key, _prefab, _displayText, _isOn, _creationCallback, _handler);
			}
		}
	}
}