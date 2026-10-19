using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TextUpdater : MonoBehaviour {

		private TextMeshProUGUI Text;
		public Slider Slider;
		private float _displayedValue = float.NaN;

		private void Awake() {
			Text = GetComponent<TextMeshProUGUI>();
		}

		void Start() {
			Slider.onValueChanged.AddListener(UpdateTextFromNumber);
			UpdateTextFromNumber(Slider.value);
		}

		void LateUpdate() {
			if (Slider.value != _displayedValue) {
				UpdateTextFromNumber(Slider.value);
			}
		}

		public void UpdateTextFromNumber(float num) {
			_displayedValue = num;
			string format = Slider.wholeNumbers ? "{0:0}" : "{0:0.00}";
			Text.text = string.Format(format, num);
		}
	}
}