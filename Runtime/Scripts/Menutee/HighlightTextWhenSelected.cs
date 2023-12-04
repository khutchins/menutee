using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

namespace Menutee {
    [Obsolete("Use MirrorSelectable instead.", false)]
    public class HighlightTextWhenSelected : MonoBehaviour, ISelectHandler, IDeselectHandler {

        public Color SelectColor = Color.white;
        [FormerlySerializedAs("TMP")]
        public TMP_Text Text;

        private Color _baseColor;
        private bool _cached;

        void MaybeCacheColor() {
            if (_cached || Text == null) return;
            // This caching doesn't happen on Awake because I want other scripts to be
            // able to change the base color at runtime before this script starts up.
            _baseColor = Text.color;
            _cached = true;
        }

        public void OnSelect(BaseEventData eventData) {
            MaybeCacheColor();
            if (Text != null) {
                Text.color = SelectColor;
            }
        }

        public void OnDeselect(BaseEventData data) {
            MaybeCacheColor();
            if (Text != null) {
                Text.color = _baseColor;
            }
        }
    }
}