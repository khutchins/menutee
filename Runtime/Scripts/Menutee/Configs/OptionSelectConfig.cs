using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class OptionSelectConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly string[] OptionStrings;
		public readonly int DefaultIndex;
		public readonly OptionSelectedHandler Handler;

		public OptionSelectConfig(InitObject configInit, string displayText, string[] optionStrings, int defaultIndex, OptionSelectedHandler handler) 
				: base(configInit) {
			DisplayText = displayText;
			OptionStrings = optionStrings;
			DefaultIndex = defaultIndex;
			Handler = handler;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			OptionSelectManager manager = go.GetComponent<OptionSelectManager>();
			if (manager == null) {
				Debug.LogWarning("Option select prefab does not contain OptionSelectManager. Menu generation will not proceed normally!");
			} else {
				manager.SetText(DisplayText);
				manager.SetOptions(OptionStrings, DefaultIndex);
				manager.OptionSelected += Handler;
			}
			return go;
		}

		public class Builder : Builder<OptionSelectConfig, Builder> {
			private string _displayText;
			private List<string> _optionStrings = new List<string>();
			private int _defaultIndex;
			private OptionSelectedHandler _handler;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			public Builder SetOptionSelectedHandler(OptionSelectedHandler handler) {
				_handler = handler;
				return _builderInstance;
			}

			public Builder AddOptionStrings(IEnumerable<string> options) {
				_optionStrings.AddRange(options);
				return _builderInstance;
			}

			public Builder AddOptionString(string option, bool defaultOption = false) {
				int newIdx = _optionStrings.Count;
				_optionStrings.Add(option);
				if (defaultOption) {
					_defaultIndex = newIdx;
				}
				return _builderInstance;
			}

			public Builder SetDefaultOptionIndex(int idx) {
				_defaultIndex = idx;
				return _builderInstance;
			}

			public override OptionSelectConfig Build() {
				return new OptionSelectConfig(BuildInitObject(), _displayText, _optionStrings.ToArray(), _defaultIndex, _handler);
			}
		}
	}
}