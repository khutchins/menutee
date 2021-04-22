using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
    public delegate void SliderUpdatedHandler(SliderManager manager, float newValue);
    public class SliderManager : UIElementManager {
        public int Id;

        public Slider Slider;
        public TextMeshProUGUI Text;

        public event SliderUpdatedHandler SliderUpdated;

        void Awake() {
            Slider.onValueChanged.AddListener(SliderValueUpdated);
        }

        public void SetText(string newText) {
            if (Text != null) {
                Text.text = newText;
            }
        }

        public void SetRange(float min, float max) {
            Slider.minValue = min;
            Slider.maxValue = max;
        }

        public void SetValue(float value) {
            Slider.SetValueWithoutNotify(value);
        }

        void SliderValueUpdated(float newValue) {
            SliderUpdated?.Invoke(this, newValue);
        }

        public override void SetColors(PaletteConfig config) {
            base.SetColors(config);

        }
    }
}