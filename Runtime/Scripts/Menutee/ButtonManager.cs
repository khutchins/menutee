using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menutee {
    public delegate void ButtonPressedHandler(ButtonManager button);
    [RequireComponent(typeof(Button))]
    public class ButtonManager : UIElementManager {
        public int Id;
        public TextMeshProUGUI Text;

        public event ButtonPressedHandler ButtonPressed;

        void Awake() {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(ButtonWasPressed);
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