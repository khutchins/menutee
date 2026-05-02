using TMPro;
using UnityEngine;

namespace Menutee {
	public class TextInputConfig : PanelObjectConfig {
		public readonly string DisplayText;
		public readonly string Placeholder;
		public readonly string DefaultValue;
		public readonly int CharacterLimit;
		public readonly TMP_InputField.ContentType ContentType;
		public readonly TMP_InputField.LineType LineType;
		public readonly bool BlocksMenuToggleWhenFocused;
		public readonly bool RequireSubmitToEdit;
		public readonly TextChangedHandler ChangedHandler;
		public readonly TextSubmittedHandler SubmittedHandler;

		public TextInputConfig(InitObject configInit, string displayText, string placeholder, string defaultValue,
				int characterLimit, TMP_InputField.ContentType contentType, TMP_InputField.LineType lineType,
				bool blocksMenuToggleWhenFocused, bool requireSubmitToEdit,
				TextChangedHandler changedHandler, TextSubmittedHandler submittedHandler)
				: base(configInit) {
			DisplayText = displayText;
			Placeholder = placeholder;
			DefaultValue = defaultValue;
			CharacterLimit = characterLimit;
			ContentType = contentType;
			LineType = lineType;
			BlocksMenuToggleWhenFocused = blocksMenuToggleWhenFocused;
			RequireSubmitToEdit = requireSubmitToEdit;
			ChangedHandler = changedHandler;
			SubmittedHandler = submittedHandler;
		}

		public override GameObject Create(GameObject parent) {
			GameObject go = Object.Instantiate(Prefab, parent.transform);
			go.name = Key;
			TextInputManager manager = go.GetComponent<TextInputManager>();
			if (manager == null) {
				Debug.LogWarning("Text input prefab does not contain TextInputManager. Menu generation will not proceed normally!");
			} else {
				manager.SetLabel(DisplayText);
				manager.SetPlaceholder(Placeholder);
				manager.SetContentType(ContentType);
				manager.SetLineType(LineType);
				manager.SetCharacterLimit(CharacterLimit);
				manager.SetValue(DefaultValue);
				manager.BlocksMenuToggleWhenFocused = BlocksMenuToggleWhenFocused;
				manager.SetRequireSubmitToEdit(RequireSubmitToEdit);
				if (ChangedHandler != null) {
					manager.TextChanged += ChangedHandler;
				}
				if (SubmittedHandler != null) {
					manager.TextSubmitted += SubmittedHandler;
				}
			}
			return go;
		}

		public class Builder : Builder<TextInputConfig, Builder> {
			private string _displayText;
			private string _placeholder;
			private string _defaultValue;
			private int _characterLimit;
			private TMP_InputField.ContentType _contentType = TMP_InputField.ContentType.Standard;
			private TMP_InputField.LineType _lineType = TMP_InputField.LineType.SingleLine;
			private bool _blocksMenuToggleWhenFocused = true;
			private bool _requireSubmitToEdit = true;
			private TextChangedHandler _changedHandler;
			private TextSubmittedHandler _submittedHandler;

			public Builder(string key, GameObject prefab) : base(key, prefab) {
			}

			public Builder SetDisplayText(string displayText) {
				_displayText = displayText;
				return _builderInstance;
			}

			public Builder SetPlaceholder(string placeholder) {
				_placeholder = placeholder;
				return _builderInstance;
			}

			public Builder SetDefaultValue(string defaultValue) {
				_defaultValue = defaultValue;
				return _builderInstance;
			}

			public Builder SetCharacterLimit(int characterLimit) {
				_characterLimit = characterLimit;
				return _builderInstance;
			}

			public Builder SetContentType(TMP_InputField.ContentType contentType) {
				_contentType = contentType;
				return _builderInstance;
			}

			public Builder SetLineType(TMP_InputField.LineType lineType) {
				_lineType = lineType;
				return _builderInstance;
			}

			/// <summary>
			/// When true (the default), the menu's toggle input (e.g. pause/menu key)
			/// is ignored while this input field has focus. Set to false if the menu
			/// toggle key cannot collide with text entry in your input bindings.
			/// </summary>
			public Builder SetBlocksMenuToggleWhenFocused(bool blocks) {
				_blocksMenuToggleWhenFocused = blocks;
				return _builderInstance;
			}

			/// <summary>
			/// When true (the default), navigating to the field only highlights it; the
			/// user must press Submit to begin editing. When false, the field activates
			/// immediately on selection (default input field behavior).
			/// Has effect only if the prefab uses <see cref="MenuteeInputField"/>.
			/// </summary>
			public Builder SetRequireSubmitToEdit(bool requireSubmit) {
				_requireSubmitToEdit = requireSubmit;
				return _builderInstance;
			}

			public Builder SetTextChangedHandler(TextChangedHandler handler) {
				_changedHandler = handler;
				return _builderInstance;
			}

			public Builder SetTextSubmittedHandler(TextSubmittedHandler handler) {
				_submittedHandler = handler;
				return _builderInstance;
			}

			public override TextInputConfig Build() {
				return new TextInputConfig(BuildInitObject(), _displayText, _placeholder, _defaultValue,
					_characterLimit, _contentType, _lineType,
					_blocksMenuToggleWhenFocused, _requireSubmitToEdit,
					_changedHandler, _submittedHandler);
			}
		}
	}
}
