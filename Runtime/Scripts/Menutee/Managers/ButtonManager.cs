using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
    public delegate void ButtonPressedHandler(ButtonManager button);
    public class ButtonManager : UIElementManager {
        public int Id;
        public TextMeshProUGUI Text;
        [Tooltip("Button this manager manages. If null, will attempt to retrieve it from this object.")]
        public Button Button;

        public event ButtonPressedHandler ButtonPressed;

        void Awake() {
            if (Button == null) {
                Button = GetComponent<Button>();
            }
            Button.onClick.AddListener(ButtonWasPressed);
        }

        public void SetText(string newText) {
            if (Text != null) {
                Text.text = newText;
            }
        }

        void ButtonWasPressed() {
            ButtonPressed?.Invoke(this);
        }
    }
}