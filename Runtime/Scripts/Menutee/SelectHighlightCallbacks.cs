using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Menutee {
    public class SelectHighlightCallbacks : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {
        private bool _selected;
        private bool _highlighted;

        [Tooltip("Called when the object is highlighted.")]
        public UnityEvent OnHighlightOccurred;
        [Tooltip("Called when the object is unhighlighted.")]
        public UnityEvent OnUnhighlightOccurred;
        [Tooltip("Called when the object is selected.")]
        public UnityEvent OnSelectOccurred;
        [Tooltip("Called when the object is deselected.")]
        public UnityEvent OnDeselectOccurred;
        [Tooltip("Called when the object is higlighted OR selected. (Equivalent to registering highlighted and selected.)")]
        public UnityEvent OnHighlightedOrSelected;
        [Tooltip("Called when the object is higlighted XOR selected.")]
        public UnityEvent OnHighlightedXOrSelected;
        [Tooltip("Called when the object was higlighted OR selected, but is no longer either.")]
        public UnityEvent OnUnhighlightedAndUnselected;
        [Tooltip("Called when the object is both higlighted AND selected.")]
        public UnityEvent OnHighlightedAndSelected;
        [Tooltip("Called when the object was higlighted AND selected, but is no longer.")]
        public UnityEvent OnUnhighlightedOrUnselected;

        public bool Selected {
            get { return _selected; }
        }

        public bool Highlighted {
            get { return _highlighted; }
        }

        public void OnSelect(BaseEventData eventData) {
            _selected = true;
            OnSelectOccurred?.Invoke();
            OnHighlightedOrSelected?.Invoke();

            if (_highlighted) {
                OnHighlightedAndSelected?.Invoke();
            } else {
                OnHighlightedXOrSelected?.Invoke();
            }
        }

        public void OnDeselect(BaseEventData eventData) {
            _selected = false;
            OnDeselectOccurred?.Invoke();

            if (_highlighted) {
                OnUnhighlightedOrUnselected?.Invoke();
            } else {
                OnUnhighlightedAndUnselected?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _highlighted = true;
            OnHighlightOccurred?.Invoke();
            OnHighlightedOrSelected?.Invoke();

            if (_selected) {
                OnHighlightedAndSelected?.Invoke();
            } else {
                OnHighlightedXOrSelected?.Invoke();
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            _highlighted = false;
            OnUnhighlightOccurred?.Invoke();

            if (_selected) {
                OnUnhighlightedOrUnselected?.Invoke();
            } else {
                OnUnhighlightedAndUnselected?.Invoke();
            }
        }
    }
}