using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class SliderConfig : PanelObjectConfig {
		public string DisplayText;
		public float MinValue;
		public float MaxValue;
		public float DefaultValue;
		public SliderUpdatedHandler Handler;

		public SliderConfig(string key, GameObject prefab, string displayText, float minValue, float maxValue, float defaultValue, System.Action<GameObject> creationCallback, SliderUpdatedHandler handler) 
				: base(key, prefab, creationCallback) {
			DisplayText = displayText;
			MinValue = minValue;
			MaxValue = maxValue;
			DefaultValue = defaultValue;
			Handler = handler;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			SliderManager manager = go.GetComponent<SliderManager>();
			if (manager == null) {
				Debug.LogWarning("Slider prefab does not contain SliderManager. Menu generation will not proceed normally!");
			} else {
				manager.SetRange(MinValue, MaxValue);
				manager.SetValue(DefaultValue);
				manager.SetText(DisplayText);
				manager.SliderUpdated += Handler;
			}
			return go;
		}

		public class Builder : Builder<SliderConfig, Builder> {
			private string _displayText;
			private SliderUpdatedHandler _handler;
			private float _minValue;
			private float _maxValue;
			private float _defaultValue;

			public Builder(string key, GameObject prefab, float minValue, float maxValue, float defaultValue) : base(key, prefab) {
				_minValue = minValue;
				_maxValue = maxValue;
				_defaultValue = defaultValue;
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			public Builder SetSliderUpdatedHandler(SliderUpdatedHandler handler) {
				_handler = handler;
				return _builderInstance;
			}

			public override SliderConfig Build() {
				return new SliderConfig(_key, _prefab, _displayText, _minValue, _maxValue, _defaultValue, _creationCallback, _handler);
			}
		}
	}
}