using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ratferences;

namespace Menutee {
	public class ToggleRefConfig : PanelObjectConfig {
		public string DisplayText;
		public readonly bool IsOn;
		public TogglePressedHandler Handler;
		public BoolReference Ref;

		public ToggleRefConfig(InitObject configInit, string displayText, BoolReference reference, TogglePressedHandler handler)
				: base(configInit) {
			DisplayText = displayText;
			Ref = reference;
			Handler = handler;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			ToggleManager manager = go.GetComponent<ToggleManager>();
			if (manager == null) {
				Debug.LogWarning("Toggle prefab does not contain ToggleManager. Menu generation will not proceed normally!");
			} else {
				manager.SetToggled(Ref.Value);
				manager.SetText(DisplayText);
				manager.TogglePressed += Handler;
			}

			// Add reference hookups.
			var receptor = go.AddComponent<BoolReceptor>();
			receptor.Reference = Ref;
			manager.Toggle.onValueChanged.AddListener(receptor.UpdateValue);

			var @event = go.AddComponent<BoolEvent>();
			@event.Reference = Ref;
			@event.AddListener(manager.Toggle.SetIsOnWithoutNotify);

			return go;
		}

		public class Builder : Builder<ToggleRefConfig, Builder> {
			private string _displayText;
			private TogglePressedHandler _handler;
			private BoolReference _ref;

			public Builder(string key, GameObject prefab, BoolReference reference) : base(key, prefab) {
				_ref = reference;
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			public Builder SetTogglePressedHandler(TogglePressedHandler handler) {
				_handler = handler;
				return _builderInstance;
			}

			public override ToggleRefConfig Build() {
				return new ToggleRefConfig(BuildInitObject(), _displayText, _ref, _handler);
			}
		}
	}
}