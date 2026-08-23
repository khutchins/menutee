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
			go.name = ObjectName;
			ToggleManager manager = go.GetComponent<ToggleManager>();
			if (manager == null) {
				Debug.LogWarning("Toggle prefab does not contain ToggleManager. Menu generation will not proceed normally!");
				return go;
			}
			manager.SetToggled(Ref.Value);
			manager.SetText(DisplayText);
			manager.TogglePressed += Handler;

			manager.TogglePressed += delegate (ToggleManager t, bool on) { Ref.Value = on; };
			BoolEvent @event = go.AddComponent<BoolEvent>();
			@event.Reference = Ref;
			@event.Changed += manager.SetToggled;

			return go;
		}

		public class Builder : BaseToggleBuilder<ToggleRefConfig, Builder> {
			private BoolReference _ref;

			public Builder(string key, GameObject prefab, BoolReference reference) : base(key, prefab) {
				_ref = reference;
			}

			public Builder(GameObject prefab, BoolReference reference) : base(prefab) {
				_ref = reference;
			}

			public override ToggleRefConfig Build() {
				return new ToggleRefConfig(BuildInitObject(), _displayText, _ref, _handler);
			}
		}
	}
}