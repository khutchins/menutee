using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class ButtonConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly ButtonPressedHandler Handler;

		public ButtonConfig(string key, GameObject prefab, string displayText, System.Action<GameObject> creationCallback, ButtonPressedHandler handler) 
				: base(key, prefab, creationCallback) {
			DisplayText = displayText;
			Handler = handler;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			ButtonManager manager = go.GetComponent<ButtonManager>();
			if (manager == null) {
				Debug.LogWarning("Button prefab does not contain ButtonManager. Menu generation will not proceed normally!");
			} else {
				manager.SetText(DisplayText);
				manager.ButtonPressed += Handler;
			}
			return go;
		}

		public class Builder : Builder<ButtonConfig, Builder> {
			private string _displayText;
			private ButtonPressedHandler _handler;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			public Builder SetButtonPressedHandler(ButtonPressedHandler handler) {
				_handler = handler;
				return _builderInstance;
			}

			public override ButtonConfig Build() {
				return new ButtonConfig(_key, _prefab, _displayText, _creationCallback, _handler);
			}
		}
	}
}