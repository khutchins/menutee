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
			go.name = Key;
			SliderManager manager = go.GetComponent<SliderManager>();
			if (manager == null) {
				Debug.LogWarning("Slider prefab does not contain SliderManager. Menu generation will not proceed normally!");
			} else {
				manager.SetRange(MinValue, MaxValue);
				manager.SetValue(Ref.Value);
				manager.SetText(DisplayText);
				manager.Slider.wholeNumbers = UseWholeNumbers;
				manager.SliderUpdated += Handler;
			}
			FloatReceptor receptor = go.GetComponent<FloatReceptor>();
			if (receptor != null) {
				receptor.Reference = Ref;
			}
			FloatEvent floatEvent = go.GetComponent<FloatEvent>();
			if (floatEvent != null) {
				floatEvent.Reference = Ref;
			}
			return go;
		}

		public class Builder : Builder<SliderRefConfig, Builder> {
			private string _displayText;
			private SliderUpdatedHandler _handler;
			private bool _useWholeNumbers;
			private float _minValue;
			private float _maxValue;
			private FloatReference _ref;

			public Builder(string key, GameObject prefab, float minValue, float maxValue, FloatReference reference) : base(key, prefab) {
				_minValue = minValue;
				_maxValue = maxValue;
				_ref = reference;
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			public Builder SetSliderUpdatedHandler(SliderUpdatedHandler handler) {
				_handler = handler;
				return _builderInstance;
			}

			public Builder SetUseWholeNumbers(bool useWholeNumbers) {
				_useWholeNumbers = useWholeNumbers;
				return _builderInstance;
			}

			public override SliderRefConfig Build() {
				return new SliderRefConfig(BuildInitObject(), _displayText, _useWholeNumbers, _minValue, _maxValue, _ref, _handler);
			}
		}
	}
}