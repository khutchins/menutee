using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ratferences;

namespace Menutee {
	public class OptionSelectRefConfig : BaseOptionSelectConfig {
		public IntReference Ref;

		public OptionSelectRefConfig(OSInitObject objectSelectInitObject, IntReference reference) : base(objectSelectInitObject) {
			Ref = reference;
		}

        protected override void OnCreate(GameObject go, OptionSelectManager manager) {
			base.OnCreate(go, manager);

			// Add reference hookups.
			var receptor = go.AddComponent<IntReceptor>();
			receptor.Reference = Ref;
			manager.OptionChanged.AddListener(receptor.UpdateValue);

			var @event = go.AddComponent<IntEvent>();
			@event.Reference = Ref;
			@event.AddListener(manager.SelectOptionWithoutNotify);
		}

		public class Builder : Builder<OptionSelectRefConfig, Builder> {
			private IntReference _ref;

			public Builder(string key, GameObject prefab, IntReference reference) : base(key, prefab) {
				_ref = reference;
			}

			public Builder SetToggleManager(System.Action<OptionSelectManager, bool> handler) {
				_handler = (OptionSelectManager manager, int index, string option) => {
					bool on = index == 1;
					handler?.Invoke(manager, on);
				};
				return _builderInstance;
			}

			public override OptionSelectRefConfig Build() {
				_defaultIndexGetter = () => _ref.Value;
				return new OptionSelectRefConfig(BuildOSInitObject(), _ref);
			}
		}
	}

	public class OptionSelectToggleRefConfig : BaseOptionSelectConfig {

		public BoolReference Ref;

		public OptionSelectToggleRefConfig(OSInitObject objectSelectInitObject, BoolReference reference) : base(objectSelectInitObject) {
			Ref = reference;
		}

		protected override void OnCreate(GameObject go, OptionSelectManager manager) {
			base.OnCreate(go, manager);

			// Add reference hookups.
			var receptor = go.AddComponent<BoolReceptor>();
			receptor.Reference = Ref;
			manager.OptionChanged.AddListener(receptor.UpdateValue);

			var @event = go.AddComponent<BoolEvent>();
			@event.Reference = Ref;
			@event.AddListener((bool newValue) => { manager.SelectOptionWithoutNotify(newValue ? 1 : 0); });
		}

		public class Builder : Builder<OptionSelectToggleRefConfig, Builder> {
			private string _onText;
			private string _offText;
			private BoolReference _ref;

			public Builder(string key, GameObject prefab, string offText, string onText, BoolReference reference) : base(key, prefab) {
				_onText = onText;
				_offText = offText;
				_ref = reference;
			}

			public Builder SetToggleManager(System.Action<OptionSelectManager, bool> handler) {
				_handler = (OptionSelectManager manager, int index, string option) => {
					bool on = index == 1;
					handler?.Invoke(manager, on);
				};
				return _builderInstance;
			}

			public override OptionSelectToggleRefConfig Build() {
				_optionStrings = new List<string>() { _offText, _onText };
				_defaultIndexGetter = () => _ref.Value ? 1 : 0;
				return new OptionSelectToggleRefConfig(BuildOSInitObject(), _ref);
			}
		}
	}
}