using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class DropdownConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly string[] OptionStrings;
		public readonly int DefaultIndex;
		public readonly DropdownChosenHandler Handler;

		public DropdownConfig(string key, GameObject prefab, string displayText, string[] optionStrings, int defaultIndex, System.Action<GameObject> creationCallback, DropdownChosenHandler handler) 
				: base(key, prefab, creationCallback) {
			DisplayText = displayText;
			OptionStrings = optionStrings;
			DefaultIndex = defaultIndex;
			Handler = handler;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			DropdownManager manager = go.GetComponent<DropdownManager>();
			if (manager == null) {
				Debug.LogWarning("Dropdown prefab does not contain DropdownManager. Menu generation will not proceed normally!");
			} else {
				manager.SetText(DisplayText);
				manager.SetOptions(OptionStrings, DefaultIndex);
				manager.DropdownChosen += Handler;
			}
			return go;
		}

		public new class Builder : PanelObjectConfig.Builder {
			private string _displayText;
			private List<string> _optionStrings = new List<string>();
			private int _defaultIndex;
			private DropdownChosenHandler _handler;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return this;
			}

			public Builder SetDropdownChosenHandler(DropdownChosenHandler handler) {
				_handler = handler;
				return this;
			}

			public Builder AddOptionStrings(IEnumerable<string> options) {
				_optionStrings.AddRange(options);
				return this;
			}

			public Builder AddOptionString(string option, bool defaultOption = false) {
				int newIdx = _optionStrings.Count;
				_optionStrings.Add(option);
				if (defaultOption) {
					_defaultIndex = newIdx;
				}
				return this;
			}

			public Builder SetDefaultOptionIndex(int idx) {
				_defaultIndex = idx;
				return this;
			}

			public override PanelObjectConfig Build() {
				return new DropdownConfig(_key, _prefab, _displayText, _optionStrings.ToArray(), _defaultIndex, _creationCallback, _handler);
			}
		}
	}
}