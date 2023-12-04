using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ratferences;

namespace Menutee {
	public class OptionSelectRefConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly string[] OptionStrings;
		public readonly bool Loops;
		public readonly OptionSelectedHandler Handler;
		public IntReference Ref;

		public OptionSelectRefConfig(InitObject configInit, string displayText, string[] optionStrings, IntReference intRef, OptionSelectedHandler handler, bool loops = true)
				: base(configInit) {
			DisplayText = displayText;
			OptionStrings = optionStrings;
			Ref = intRef;
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
				manager.SetOptions(OptionStrings, Ref.Value);
				manager.OptionSelected += Handler;
			}

			// Add reference hookups.
			var receptor = go.AddComponent<IntReceptor>();
			receptor.Reference = Ref;
			manager.OptionChanged.AddListener(receptor.UpdateValue);

			var @event = go.AddComponent<IntEvent>();
			@event.Reference = Ref;
			@event.AddListener(manager.SelectOptionWithoutNotify);

            return go;
		}

		public class Builder : Builder<OptionSelectRefConfig, Builder> {
			private string _displayText;
			private List<string> _optionStrings = new List<string>();
			private IntReference _ref;
			private bool _loops = true;
			private OptionSelectedHandler _handler;

			public Builder(string key, GameObject prefab, IntReference reference) : base(key, prefab) {
				_ref = reference;
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			public Builder SetOptionSelectedHandler(OptionSelectedHandler handler) {
				_handler = handler;
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

			public Builder AddOptionStrings(IEnumerable<string> options) {
				_optionStrings.AddRange(options);
				return _builderInstance;
			}

			public Builder AddOptionString(string option) {
				_optionStrings.Add(option);
				return _builderInstance;
			}

			public override OptionSelectRefConfig Build() {
				return new OptionSelectRefConfig(BuildInitObject(), _displayText, _optionStrings.ToArray(), _ref, _handler, _loops);
			}
		}
	}

	public class OptionSelectToggleRefConfig : PanelObjectConfig {

		public readonly string DisplayText;
		public readonly string[] OptionStrings;
		public readonly bool Loops;
		public readonly OptionSelectedHandler Handler;
		public BoolReference Ref;

		public OptionSelectToggleRefConfig(InitObject configInit, string displayText, string[] optionStrings, BoolReference reference, OptionSelectedHandler handler, bool loops = true)
				: base(configInit) {
			DisplayText = displayText;
			OptionStrings = optionStrings;
			Ref = reference;
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
				manager.SetOptions(OptionStrings, Ref.Value ? 1 : 0);
				manager.OptionSelected += Handler;
			}

			// Add reference hookups.
			var receptor = go.AddComponent<BoolReceptor>();
			receptor.Reference = Ref;
			manager.OptionChanged.AddListener(receptor.UpdateValue);

			var @event = go.AddComponent<BoolEvent>();
			@event.Reference = Ref;
			@event.AddListener((bool newValue) => { manager.SelectOptionWithoutNotify(newValue ? 1 : 0); });

			return go;
		}


		public class Builder : Builder<OptionSelectToggleRefConfig, Builder> {
			private string _displayText;
			private string _onText;
			private string _offText;
			private BoolReference _ref;
			private bool _loops = true;
			private OptionSelectedHandler _handler;

			public Builder(string key, GameObject prefab, string offText, string onText, BoolReference reference) : base(key, prefab) {
				_onText = onText;
				_offText = offText;
				_ref = reference;
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

			public Builder SetToggleManager(System.Action<OptionSelectManager, bool> handler) {
				_handler = (OptionSelectManager manager, int index, string option) => {
					bool on = index == 1;
					handler?.Invoke(manager, on);
				};
				return _builderInstance;
			}

			public override OptionSelectToggleRefConfig Build() {
				return new OptionSelectToggleRefConfig(BuildInitObject(), _displayText, new string[] { _offText, _onText }, _ref, _handler, _loops);
			}
		}
	}
}