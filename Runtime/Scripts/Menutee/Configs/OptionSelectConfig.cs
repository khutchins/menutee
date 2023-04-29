using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public class OptionSelectConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly string[] OptionStrings;
		public readonly int DefaultIndex;
		public readonly bool Loops;
		public readonly bool HidesArrowIfLastOption;
		public readonly OptionSelectedHandler Handler;

		public OptionSelectConfig(InitObject configInit, string displayText, string[] optionStrings, int defaultIndex, OptionSelectedHandler handler, bool loops = true, bool hidesArrowIfLastOption = true) 
				: base(configInit) {
			DisplayText = displayText;
			OptionStrings = optionStrings;
			DefaultIndex = defaultIndex;
			Handler = handler;
			Loops = loops;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			OptionSelectManager manager = go.GetComponent<OptionSelectManager>();
			if (manager == null) {
				Debug.LogWarning("Option select prefab does not contain OptionSelectManager. Menu generation will not proceed normally!");
			} else {
				manager.Loops = Loops;
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
			private bool _loops = true;
			private bool _hidesArrowIfLastOption = true;
			private OptionSelectedHandler _handler;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			/// <summary>
			/// Whether or not clicking left from the first option will go to the last option and vice versa.
			/// Default is true.
			/// </summary>
			/// <param name="loops">Whether or not selected object can loop.</param>
			public Builder SetLoops(bool loops) {
				_loops = loops;
				return _builderInstance;
            }

			/// <summary>
			/// Whether or not the arrows will be hidden if the player cannot select an option in that direction.
			/// If Loops is set, the arrows will never disappear, as that cannot happen.
			/// Default is true.
			/// </summary>
			/// <param name="loops">Whether or not selected object can loop.</param>
			public Builder SetHidesArrowIfLastOption(bool hidesArrowIfLastOption) {
				_hidesArrowIfLastOption = hidesArrowIfLastOption;
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
				return new OptionSelectConfig(BuildInitObject(), _displayText, _optionStrings.ToArray(), _defaultIndex, _handler, _loops, _hidesArrowIfLastOption);
			}
		}
	}

	public static class OptionSelectToggleConfig {

		public static void SetOn(OptionSelectManager manager, bool on) {
			manager.SelectOption(on ? 1 : 0);
        }

		public class Builder : PanelObjectConfig.Builder<OptionSelectConfig, Builder> {
			private string _displayText;
			private string _onText;
			private string _offText;
			private bool _isOn;
			private bool _loops = true;
			private bool _hidesArrowIfLastOption = true;
			private OptionSelectedHandler _handler;

			public Builder(string key, GameObject prefab, string offText, string onText, bool isOn) : base(key, prefab) {
				_onText = onText;
				_offText = offText;
				_isOn = isOn;
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			/// <summary>
			/// Whether or not clicking left from the first option will go to the last option and vice versa.
			/// Default is true.
			/// </summary>
			/// <param name="loops">Whether or not selected object can loop.</param>
			public Builder SetLoops(bool loops) {
				_loops = loops;
				return _builderInstance;
			}

			/// <summary>
			/// Whether or not the arrows will be hidden if the player cannot select an option in that direction.
			/// If Loops is set, the arrows will never disappear, as that cannot happen.
			/// Default is true.
			/// </summary>
			/// <param name="loops">Whether or not selected object can loop.</param>
			public Builder SetHidesArrowIfLastOption(bool hidesArrowIfLastOption) {
				_hidesArrowIfLastOption = hidesArrowIfLastOption;
				return _builderInstance;
			}

			public Builder SetToggleManager(System.Action<OptionSelectManager, bool> handler) {
				_handler = (OptionSelectManager manager, int index, string option) => {
					bool on = index == 1;
					handler?.Invoke(manager, on);
				};
				return _builderInstance;
			}

			public override OptionSelectConfig Build() {
				return new OptionSelectConfig(BuildInitObject(), _displayText, new string[] { _offText, _onText }, _isOn ? 1 : 0, _handler, _loops, _hidesArrowIfLastOption);
			}
		}
	}

	
}