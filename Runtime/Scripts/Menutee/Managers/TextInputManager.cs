using TMPro;
using UnityEngine;

namespace Menutee {
	public delegate void TextChangedHandler(TextInputManager manager, string newText);
	public delegate void TextSubmittedHandler(TextInputManager manager, string finalText);

	public class TextInputManager : UIElementManager {
		[Tooltip("Optional label rendered next to the input field.")]
		public TextMeshProUGUI Label;
		public TMP_InputField InputField;

		public bool BlocksMenuToggleWhenFocused = true;

		public event TextChangedHandler TextChanged;
		public event TextSubmittedHandler TextSubmitted;

		// True for one frame after TMP_InputField self-deactivates on Esc.
		// Lets us swallow the same cancel press if MenuManager reads it after
		// EventSystem already cleared the focus.
		private bool _swallowNextCancel;

		void Awake() {
			if (InputField != null) {
				InputField.onValueChanged.AddListener(OnInputValueChanged);
				InputField.onEndEdit.AddListener(OnInputEndEdit);
			}
		}

		void LateUpdate() {
			_swallowNextCancel = false;
		}

		public void SetLabel(string newText) {
			if (Label != null) {
				Label.text = newText;
			}
		}

		public void SetPlaceholder(string placeholder) {
			if (InputField != null && InputField.placeholder is TMP_Text placeholderText) {
				placeholderText.text = placeholder ?? string.Empty;
			}
		}

		public void SetValue(string value) {
			if (InputField != null) {
				InputField.SetTextWithoutNotify(value ?? string.Empty);
			}
		}

		public void SetCharacterLimit(int limit) {
			if (InputField != null) {
				InputField.characterLimit = limit;
			}
		}

		public void SetContentType(TMP_InputField.ContentType contentType) {
			if (InputField != null) {
				InputField.contentType = contentType;
			}
		}

		public void SetLineType(TMP_InputField.LineType lineType) {
			if (InputField != null) {
				InputField.lineType = lineType;
			}
		}

		public void SetRequireSubmitToEdit(bool requireSubmit) {
			if (InputField is DeferredSelectionInputField menuteeField) {
				menuteeField.RequireSubmitToEdit = requireSubmit;
			} else {
				Debug.LogWarning($"Cannot require submit to edit on this text field - it has to be a DeferredSelectionInputField. {gameObject.name}");
            }
		}

		public string Value {
			get => InputField != null ? InputField.text : string.Empty;
		}

		void OnInputValueChanged(string newValue) {
			TextChanged?.Invoke(this, newValue);
		}

		void OnInputEndEdit(string finalValue) {
			if (InputField != null && InputField.wasCanceled) {
				_swallowNextCancel = true;
			}
			TextSubmitted?.Invoke(this, finalValue);
		}

		public override bool TryHandleCancelInput() {
			if (InputField != null && InputField.isFocused) {
				// MenuManager ran before EventSystem dispatch this frame: deactivate ourselves.
				InputField.DeactivateInputField();
				_swallowNextCancel = false;
				return true;
			}
			if (_swallowNextCancel) {
				// EventSystem ran first and already deactivated us via Esc; swallow the matching cancel.
				_swallowNextCancel = false;
				return true;
			}
			return false;
		}

		public override bool BlocksMenuToggle {
			get => BlocksMenuToggleWhenFocused && InputField != null && InputField.isFocused;
		}
	}
}
