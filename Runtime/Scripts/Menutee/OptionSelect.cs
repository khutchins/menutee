using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menutee {
	/// <summary>
	/// A Selectable "cycle through discrete options" control.
	/// </summary>
	public class OptionSelect : Selectable, IPointerClickHandler, ISubmitHandler {
		public enum StepAxis { Horizontal, Vertical }

		[System.Serializable] public class OptionChangeEvent : UnityEvent<int> { }

		[Header("Display")]
		public TextMeshProUGUI OptionText;
		public Button PrevArrow;
		public Button NextArrow;

		[Header("Behavior")]
		public bool Loops = true;
		public bool HidesArrowAtEnd = true;
		[Tooltip("Which input axis cycles the value. Horizontal for vertical menus; Vertical for horizontal menus.")]
		public StepAxis Axis = StepAxis.Horizontal;

		public OptionChangeEvent onValueChanged = new OptionChangeEvent();

		[Header("Options")]
		[SerializeField] private List<string> _options = new List<string>();
		[SerializeField] private int _index;

		public int value {
			get => _index;
			set => Set(value, true);
		}

		public IReadOnlyList<string> Options => _options;

		private bool CanGoLeft => _options.Count > 0 && (Loops || _index > 0);
		private bool CanGoRight => _options.Count > 0 && (Loops || _index < _options.Count - 1);

		protected override void Awake() {
			base.Awake();
			if (PrevArrow != null) PrevArrow.onClick.AddListener(StepPreviousKeepingFocus);
			if (NextArrow != null) NextArrow.onClick.AddListener(StepNextKeepingFocus);
			UpdateDisplay();
		}

		private void StepPreviousKeepingFocus() {
			SelectPrevious();
			KeepFocus();
		}

		private void StepNextKeepingFocus() {
			SelectNext();
			KeepFocus();
		}

		private void KeepFocus() {
			if (EventSystem.current != null) {
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}

#if UNITY_EDITOR
		protected override void OnValidate() {
			base.OnValidate();
			_index = _options.Count == 0 ? 0 : Mathf.Clamp(_index, 0, _options.Count - 1);
			RefreshCaption();
		}
#endif

		public void SetOptions(IList<string> options, int index) {
			_options = new List<string>(options);
			Set(index, false);
		}

		public void SetValueWithoutNotify(int input) => Set(input, false);

		public void SelectPrevious() {
			if (!CanGoLeft) return;
			int n = _options.Count;
			Set(((_index - 1) % n + n) % n, true);
		}

		public void SelectNext() {
			if (!CanGoRight) return;
			Set((_index + 1) % _options.Count, true);
		}

		private void Set(int newIndex, bool sendCallback) {
			if (_options.Count == 0) {
				_index = 0;
				UpdateDisplay();
				return;
			}
			newIndex = Mathf.Clamp(newIndex, 0, _options.Count - 1);
			bool changed = newIndex != _index;
			_index = newIndex;
			UpdateDisplay();
			if (sendCallback && changed) {
				onValueChanged.Invoke(_index);
			}
		}

		private void UpdateDisplay() {
			RefreshCaption();
			UpdateArrows();
		}

		private void RefreshCaption() {
			if (OptionText != null && _index >= 0 && _index < _options.Count) {
				OptionText.text = _options[_index];
			}
		}

		private void UpdateArrows() {
			SetArrowVisible(PrevArrow, !HidesArrowAtEnd || CanGoLeft);
			SetArrowVisible(NextArrow, !HidesArrowAtEnd || CanGoRight);
		}

		private static void SetArrowVisible(Button arrow, bool visible) {
			if (arrow == null) return;
			GameObject go = arrow.gameObject;
			// Hides using a CanvasGroup instead of SetActive so that it doesn't reflow.
			if (!go.TryGetComponent(out CanvasGroup cg)) {
				cg = go.AddComponent<CanvasGroup>();
			}
			cg.alpha = visible ? 1f : 0f;
			cg.interactable = visible;
			cg.blocksRaycasts = visible;
		}

		// Clicking the body advances to the next option.
		public void OnPointerClick(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left) return;
			if (!IsActive() || !IsInteractable()) return;
			SelectNext();
		}

		public void OnSubmit(BaseEventData eventData) {
			if (!IsActive() || !IsInteractable()) return;
			SelectNext();
		}
		public override void OnMove(AxisEventData eventData) {
			if (!IsActive() || !IsInteractable()) {
				base.OnMove(eventData);
				return;
			}
			MoveDirection dir = eventData.moveDir;
			if (Axis == StepAxis.Horizontal) {
				if (dir == MoveDirection.Left) { 
					SelectPrevious(); 
					return; 
				}
				if (dir == MoveDirection.Right) {
					SelectNext();
					return; 
				}
			} else {
				if (dir == MoveDirection.Up) {
					SelectPrevious();
					return;
				}
				if (dir == MoveDirection.Down) {
					SelectNext();
					return;
				}
			}
			base.OnMove(eventData);
		}
	}
}
