using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
    public delegate void TogglePressedHandler(ToggleManager toggle, bool on);
    public class ToggleManager : UIElementManager {
        public int Id;

        public event TogglePressedHandler TogglePressed;

        public TextMeshProUGUI Text;
        public Toggle Toggle;

        void Awake() {
            Toggle.onValueChanged.AddListener(ToggleWasPressed);
        }

        public void SetText(string newText) {
            if (Text != null) {
                Text.text = newText;
            }
        }

        public void SetToggled(bool newValue) {
            Toggle.SetIsOnWithoutNotify(newValue);
        }

        void ToggleWasPressed(bool newValue) {
            TogglePressed?.Invoke(this, newValue);
        }
    }
}