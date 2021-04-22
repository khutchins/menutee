using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menutee {
    public class HighlightTextWhenSelected : MonoBehaviour, ISelectHandler, IDeselectHandler {

        public Color SelectColor = Color.white;
        public Text Text;
        public TextMeshProUGUI TMP;

        private Color _baseColor;

        // Start is called before the first frame update
        void Start() {
            if (Text != null) {
                _baseColor = Text.color;
            } else if (TMP != null) {
                _baseColor = TMP.color;
            }
        }

        //Do this when the selectable UI object is selected.
        public void OnSelect(BaseEventData eventData) {
            if (Text != null) {
                Text.color = SelectColor;
            }
            if (TMP != null) {
                TMP.color = SelectColor;
            }
        }

        public void OnDeselect(BaseEventData data) {
            if (Text != null) {
                Text.color = _baseColor;
            }
            if (TMP != null) {
                TMP.color = _baseColor;
            }
        }
    }
}