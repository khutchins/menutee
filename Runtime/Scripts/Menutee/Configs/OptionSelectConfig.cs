using System;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	public abstract class BaseOptionSelectConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly string[] OptionStrings;
		public int DefaultIndex { get => DefaultIndexGetter(); }
		public readonly Func<int> DefaultIndexGetter;
		public readonly bool Loops;
		public readonly bool HidesArrowIfLastOption;
		public readonly OptionSelectedHandler Handler;

		public BaseOptionSelectConfig(OSInitObject objectSelectInitObject) : base(objectSelectInitObject.InitObject) {
			DisplayText = objectSelectInitObject.DisplayText;
			OptionStrings = objectSelectInitObject.OptionStrings;
			DefaultIndexGetter = objectSelectInitObject.DefaultIndexGetter;
			Loops = objectSelectInitObject.Loops;
			HidesArrowIfLastOption = objectSelectInitObject.HidesArrowIfLastOption;
			Handler = objectSelectInitObject.Handler;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = UnityEngine.Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			OptionSelectManager manager = go.GetComponent<OptionSelectManager>();
			if (manager == null) {
				Debug.LogWarning("Option select prefab does not contain OptionSelectManager. Menu generation will not proceed normally!");
			} else {
				manager.Loops = Loops;
				manager.SetText(DisplayText);
				manager.SetOptions(OptionStrings, DefaultIndex);
				manager.OptionSelected += Handler;
				OnCreate(go, manager);
			}
			return go;
		}

		protected virtual void OnCreate(GameObject go, OptionSelectManager manager) {
			// Override this.
		}

		public class OSInitObject {
			public readonly InitObject InitObject;
			public readonly string DisplayText;
			public readonly string[] OptionStrings;
			public readonly Func<int> DefaultIndexGetter;
			public readonly bool Loops;
			public readonly bool HidesArrowIfLastOption;
			public readonly OptionSelectedHandler Handler;

            public OSInitObject(InitObject initObject, string displayText, string[] optionStrings, Func<int> defaultIndex, bool loops, bool hidesArrowIfLastOption, OptionSelectedHandler handler) {
				InitObject = initObject;
                DisplayText = displayText;
                OptionStrings = optionStrings;
				DefaultIndexGetter = defaultIndex;
                Loops = loops;
                HidesArrowIfLastOption = hidesArrowIfLastOption;
                Handler = handler;
            }
        }

		public abstract new class Builder<TObject, TBuilder> : PanelObjectConfig.Builder<TObject, TBuilder> where TObject : BaseOptionSelectConfig where TBuilder : Builder<TObject, TBuilder> {
			protected string _displayText;
			protected List<string> _optionStrings = new List<string>();
			protected Func<int> _defaultIndexGetter;
			protected bool _loops = true;
			protected bool _hidesArrowIfLastOption = true;
			protected OptionSelectedHandler _handler;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public TBuilder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			/// <summary>
			/// Whether or not clicking left from the first option will go to the last option and vice versa.
			/// Default is true.
			/// </summary>
			/// <param name="loops">Whether or not selected object can loop.</param>
			public TBuilder SetLoops(bool loops) {
				_loops = loops;
				return _builderInstance;
			}

			/// <summary>
			/// Whether or not the arrows will be hidden if the player cannot select an option in that direction.
			/// If Loops is set, the arrows will never disappear, as that cannot happen.
			/// Default is true.
			/// </summary>
			/// <param name="loops">Whether or not selected object can loop.</param>
			public TBuilder SetHidesArrowIfLastOption(bool hidesArrowIfLastOption) {
				_hidesArrowIfLastOption = hidesArrowIfLastOption;
				return _builderInstance;
			}

			public TBuilder SetOptionSelectedHandler(OptionSelectedHandler handler) {
				_handler = handler;
				return _builderInstance;
			}

			public TBuilder AddOptionStrings(IEnumerable<string> options) {
				_optionStrings.AddRange(options);
				return _builderInstance;
			}

			public TBuilder AddOptionString(string option, bool defaultOption = false) {
				int newIdx = _optionStrings.Count;
				_optionStrings.Add(option);
				if (defaultOption) {
					SetDefaultOptionIndex(newIdx);
				}
				return _builderInstance;
			}

			public TBuilder SetDefaultOptionIndex(int idx) {
				_defaultIndexGetter = () => idx;
				return _builderInstance;
			}

			public TBuilder SetDefaultIndexGetter(Func<int> idxGetter) {
				_defaultIndexGetter = idxGetter;
				return _builderInstance;
			}

			public OSInitObject BuildOSInitObject() {
				return new OSInitObject(BuildInitObject(), _displayText, _optionStrings.ToArray(), _defaultIndexGetter, _loops, _hidesArrowIfLastOption, _handler);
			}
		}
	}

	public class OptionSelectConfig : BaseOptionSelectConfig {
		public OptionSelectConfig(OSInitObject objectSelectInitObject) : base(objectSelectInitObject) {
		}

		public class Builder : Builder<OptionSelectConfig, Builder> {

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public override OptionSelectConfig Build() {
				return new OptionSelectConfig(BuildOSInitObject());
			}
		}
	}

	public class OptionSelectMappedConfig<T> : BaseOptionSelectConfig {
		public readonly T[] Options;

		public OptionSelectMappedConfig(OSInitObject objectSelectInitObject, T[] options) : base(objectSelectInitObject) {
			Options = options;
		}

		public delegate void OptionSelectedMappedHandler(OptionSelectManager manager, int index, string optionString, T option);

		public class Builder : Builder<OptionSelectConfig, Builder> {
			private List<T> _options = new List<T>();
			private OptionSelectedMappedHandler _mappedHandler;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public Builder AddOptionString(string optionString, T option, bool defaultOption = false) {
				AddOptionString(optionString, defaultOption);
				_options.Add(option);
				return _builderInstance;
			}

			public Builder SetDefaultOption(T option) {
				int idx = _options.IndexOf(option);
				if (idx < 0) {
					Debug.LogWarning($"Option {option} does not exist in options list.");
				} else {
					SetDefaultOptionIndex(idx);
				}
				return _builderInstance;
			}

			public Builder SetMappedHandler(OptionSelectedMappedHandler handler) {
				_mappedHandler = handler;
				return _builderInstance;
			}

			public override OptionSelectConfig Build() {
				if (_handler != null && _mappedHandler != null) {
					OptionSelectedHandler oldHandler = _handler;
					_handler = (OptionSelectManager dropdown, int index, string optionString) => {
						oldHandler(dropdown, index, optionString);
						_mappedHandler(dropdown, index, optionString, _options[index]);
					};
				} else if (_mappedHandler != null) {
					_handler = (OptionSelectManager dropdown, int index, string optionString) => {
						_mappedHandler(dropdown, index, optionString, _options[index]);
					};
				}
				if (_options.Count != _optionStrings.Count) {
					Debug.LogError("Invalid configuration. Options does not match Option strings. Things will be broken.");
				}
				return new OptionSelectConfig(BuildOSInitObject());
			}
		}
	}

	public class OptionSelectToggleConfig : BaseOptionSelectConfig {
        public OptionSelectToggleConfig(OSInitObject objectSelectInitObject) : base(objectSelectInitObject) {
        }

        public static void SetOn(OptionSelectManager manager, bool on) {
			manager.SelectOption(on ? 1 : 0);
        }

		public class Builder : Builder<OptionSelectToggleConfig, Builder> {
			private string _onText;
			private string _offText;
			private bool _isOn;

			public Builder(string key, GameObject prefab, string offText, string onText, bool isOn) : base(key, prefab) {
				_onText = onText;
				_offText = offText;
				_isOn = isOn;
			}

			public Builder SetToggleManager(System.Action<OptionSelectManager, bool> handler) {
				_handler = (OptionSelectManager manager, int index, string option) => {
					bool on = index == 1;
					handler?.Invoke(manager, on);
				};
				return _builderInstance;
			}

			public override OptionSelectToggleConfig Build() {
				_optionStrings = new List<string>() { _offText, _onText };
				SetDefaultOptionIndex(_isOn ? 1 : 0);
				return new OptionSelectToggleConfig(BuildOSInitObject());
			}
		}
	}

	
}