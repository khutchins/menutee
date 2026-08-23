using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ratferences;

namespace Menutee {
	public class SliderRefConfig : PanelObjectConfig {
		public string DisplayText;
		public float MinValue;
		public float MaxValue;
		public bool UseWholeNumbers;
		public SliderUpdatedHandler Handler;
		public FloatReference Ref;

		public SliderRefConfig(InitObject configInit, string displayText, bool useWholeNumbers, float minValue, float maxValue, FloatReference reference, SliderUpdatedHandler handler)
				: base(configInit) {
			DisplayText = displayText;
			MinValue = minValue;
			MaxValue = maxValue;
			UseWholeNumbers = useWholeNumbers;
			Handler = handler;
			Ref = reference;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = ObjectName;
			SliderManager manager = go.GetComponent<SliderManager>();
			if (manager == null) {
				Debug.LogWarning("Slider prefab does not contain SliderManager. Menu generation will not proceed normally!");
				return go;
			}
			manager.SetRange(MinValue, MaxValue);
			manager.SetValue(Ref.Value);
			manager.SetText(DisplayText);
			manager.Slider.wholeNumbers = UseWholeNumbers;
			manager.SliderUpdated += Handler;

			manager.SliderUpdated += delegate (SliderManager m, float newValue) { Ref.Value = newValue; };
			FloatEvent @event = go.AddComponent<FloatEvent>();
			@event.Reference = Ref;
			@event.Changed += manager.SetValue;

			return go;
		}

        public class Builder : BaseSliderBuilder<SliderRefConfig, Builder> {
            private FloatReference _ref;

            public Builder(string key, GameObject prefab, float minValue, float maxValue, FloatReference reference)
                : base(key, prefab, minValue, maxValue) {
                _ref = reference;
            }

            public Builder(GameObject prefab, float minValue, float maxValue, FloatReference reference)
                : base(prefab, minValue, maxValue) {
                _ref = reference;
            }

            public override SliderRefConfig Build() {
                return new SliderRefConfig(BuildInitObject(), _displayText, _useWholeNumbers, _minValue, _maxValue, _ref, _handler);
            }
        }
    }
}