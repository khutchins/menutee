using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class SliderConfig : PanelObjectConfig {
		public string DisplayText;
		public float MinValue;
		public float MaxValue;
		public float DefaultValue;
		public bool UseWholeNumbers;
		public SliderUpdatedHandler Handler;

		public SliderConfig(InitObject configInit, string displayText, bool useWholeNumbers, float minValue, float maxValue, float defaultValue, SliderUpdatedHandler handler) 
				: base(configInit) {
			DisplayText = displayText;
			MinValue = minValue;
			MaxValue = maxValue;
			DefaultValue = defaultValue;
			UseWholeNumbers = useWholeNumbers;
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
				manager.Slider.wholeNumbers = UseWholeNumbers;
				manager.SliderUpdated += Handler;
			}
			return go;
		}

		public class Builder : BaseSliderBuilder<SliderConfig, Builder> {
			private float _defaultValue;

			public Builder(string key, GameObject prefab, float minValue, float maxValue, float defaultValue) : base(key, prefab, minValue, maxValue) {
				_defaultValue = defaultValue;
			}

			public override SliderConfig Build() {
				return new SliderConfig(BuildInitObject(), _displayText, _useWholeNumbers, _minValue, _maxValue, _defaultValue, _handler);
			}
		}
	}

    public abstract class BaseSliderBuilder<TConfig, TBuilder> : PanelObjectConfig.Builder<TConfig, TBuilder>
        where TConfig : PanelObjectConfig
        where TBuilder : BaseSliderBuilder<TConfig, TBuilder> {

        protected string _displayText;
        protected float _minValue;
        protected float _maxValue;
        protected bool _useWholeNumbers;
        protected SliderUpdatedHandler _handler;

        public BaseSliderBuilder(string key, GameObject prefab, float minValue, float maxValue)
            : base(key, prefab) {
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public TBuilder SetDisplayText(string displayText) {
            _displayText = displayText;
            return _builderInstance;
        }

        public TBuilder SetUseWholeNumbers(bool useWholeNumbers) {
            _useWholeNumbers = useWholeNumbers;
            return _builderInstance;
        }

        public TBuilder SetSliderUpdatedHandler(SliderUpdatedHandler handler) {
            _handler = handler;
            return _builderInstance;
        }
    }
}