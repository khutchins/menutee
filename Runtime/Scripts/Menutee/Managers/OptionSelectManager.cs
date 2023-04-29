using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menutee {
    public delegate void OptionSelectedHandler(OptionSelectManager dropdown, int index, string option);
    public class OptionSelectManager : UIElementManager {
        public int Id;

        public event OptionSelectedHandler OptionSelected;

        public Button LeftButton;
        public Button RightButton;
        public Button TextButton;

        public TextMeshProUGUI Text;
        public TextMeshProUGUI OptionText;

        public UnityEvent<int> OptionChanged;

        public bool Loops { get; set; }
        private int _index;
        private string[] _options;
        private bool _selected;

        void Awake() {
            if (LeftButton != null) {
                LeftButton.onClick.AddListener(ChooseLeft);
            }
            if (RightButton != null) {
                RightButton.onClick.AddListener(ChooseRight);
            }
            if (TextButton != null) {
                TextButton.onClick.AddListener(ChooseRight);
                MoveCallback callback = TextButton.GetComponent<MoveCallback>();
                if (callback != null) {
                    callback.Move += OnSelected;
                } else {
                    Debug.LogWarning("No SelectedCallback on option select button. Keyboard/controller left/right won't work.");
                }
            }
        }

        void OnSelected(MoveCallback obj, MoveDirection moveDir) {
            if (moveDir == MoveDirection.Left) {
                ChooseLeft();
            } else if (moveDir == MoveDirection.Right) {
                ChooseRight();
            }
        }

        public void SetText(string newText) {
            if (Text != null) {
                Text.text = newText;
            }
        }

        private void ChooseLeft() {
            if (!CanChooseLeft) return;
            OptionUpdateInternal((_index - 1 + _options.Length) % _options.Length);
        }

        private void ChooseRight() {
            if (!CanChooseRight) return;
            OptionUpdateInternal((_index + 1) % _options.Length);
        }

        public void SetOptions(string[] options, int index) {
            _options = options;
            _index = index;
            UpdateDisplay();
        }

        public void SelectOption(int newIndex) {
            OptionUpdateInternal(newIndex);
        }

        public void SelectOptionWithoutNotify(int newIndex) {
            OptionUpdateInternal(newIndex, false);
        }

        void OptionUpdateInternal(int newIndex, bool notify = true) {
            _index = newIndex;
            UpdateDisplay();
            if (notify) {
                OptionSelected?.Invoke(this, _index, _options[_index]);
                OptionChanged?.Invoke(newIndex);
            }
        }

        private bool CanChooseLeft {
            get => Loops || _index > 0;
        }

        private bool CanChooseRight {
            get => Loops || _index < _options.Length - 1;
        }

        private void UpdateDisplay() {
            OptionText.text = _options[_index];
            LeftButton.gameObject.SetActive(CanChooseLeft);
            RightButton.gameObject.SetActive(CanChooseRight);
        }

        public override void SetColors(PaletteConfig config) {
            base.SetColors(config);
            config.ApplyToSelectable(TextButton);
            config.ApplyToSelectable(LeftButton);
            config.ApplyToSelectable(RightButton);
        }
    }
}