using TMPro;
using UnityEngine.Events;

namespace Menutee {
	public delegate void OptionSelectedHandler(OptionSelectManager manager, int index, string option);
	public class OptionSelectManager : UIElementManager {
		public event OptionSelectedHandler OptionSelected;
		public UnityEvent<int> OptionChanged;

		public TextMeshProUGUI Text;
		public OptionSelect OptionSelect;

		private string[] _options;

		public bool Loops {
			get => OptionSelect != null && OptionSelect.Loops;
			set {
				if (OptionSelect != null) {
					OptionSelect.Loops = value;
				}
			}
		}

		public bool HidesArrowAtEnd {
			get => OptionSelect != null && OptionSelect.HidesArrowAtEnd;
			set {
				if (OptionSelect != null) {
					OptionSelect.HidesArrowAtEnd = value;
				}
			}
		}

		void Awake() {
			if (OptionSelect != null) {
				OptionSelect.onValueChanged.AddListener(OptionUpdatedInternal);
				if (SelectableObject == null) {
					SelectableObject = OptionSelect.gameObject;
				}
			}
		}

		public void SetText(string newText) {
			if (Text != null) {
				Text.text = newText;
			}
		}

		public void SetOptions(string[] options, int index) {
			_options = options;
			if (OptionSelect != null) {
				OptionSelect.SetOptions(options, index);
			}
		}

		public void SelectOption(int newIndex) {
			if (OptionSelect != null) {
				OptionSelect.value = newIndex;
			}
		}

		public void SelectOptionWithoutNotify(int newIndex) {
			if (OptionSelect != null) {
				OptionSelect.SetValueWithoutNotify(newIndex);
			}
		}

		void OptionUpdatedInternal(int newIndex) {
			OptionSelected?.Invoke(this, newIndex, _options[newIndex]);
			OptionChanged?.Invoke(newIndex);
		}
	}
}
