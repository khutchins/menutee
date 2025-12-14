using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class ToggleConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly bool IsOn;
		public readonly TogglePressedHandler Handler;

		public ToggleConfig(InitObject configInit, string displayText, bool isOn, TogglePressedHandler handler) 
				: base(configInit) {
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

		public class Builder : BaseToggleBuilder<ToggleConfig, Builder> {
            protected bool _isOn;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public Builder SetIsOn(bool isOn) {
				_isOn = isOn;
				return _builderInstance;
			}

			public override ToggleConfig Build() {
				return new ToggleConfig(BuildInitObject(), _displayText, _isOn, _handler);
			}
		}
	}

    public abstract class BaseToggleBuilder<TConfig, TBuilder> : PanelObjectConfig.Builder<TConfig, TBuilder>
        where TConfig : PanelObjectConfig
        where TBuilder : BaseToggleBuilder<TConfig, TBuilder> {

        protected string _displayText;
        protected TogglePressedHandler _handler;

        public BaseToggleBuilder(string key, GameObject prefab) : base(key, prefab) { }

        public TBuilder SetDisplayText(string displayText) {
            _displayText = displayText;
            return _builderInstance;
        }

        public TBuilder SetTogglePressedHandler(TogglePressedHandler handler) {
            _handler = handler;
            return _builderInstance;
        }
    }
}