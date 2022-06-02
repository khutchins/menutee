using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Menutee {
	public class TextConfig : PanelObjectConfig {
		public readonly string DisplayText;

		public TextConfig(InitObject configInit, string displayText)
				: base(configInit) {
			DisplayText = displayText;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
			if (text == null) {
				Debug.LogWarning("No text on TextConfig object!");
			} else {
				text.text = DisplayText;
			}
			return go;
		}

		public class Builder : Builder<TextConfig, Builder> {
			private string _displayText;

			public Builder(string key, GameObject prefab, string text) : base(key, prefab) {
				_displayText = text;
			}

			public override TextConfig Build() {
				return new TextConfig(BuildInitObject(), _displayText);
			}
		}
	}
}