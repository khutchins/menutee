using UnityEngine;
using UnityEngine.EventSystems;

namespace Menutee {
    /// <summary>
    /// Internal plumbing that watches a single selectable's select/deselect and
    /// pointer enter/exit events, tracks its <see cref="InteractionState"/>, and
    /// reports transitions to the owning <see cref="MenuManager"/>, which resolves
    /// the menu-wide focus. One is attached automatically to every generated
    /// element and on demand by <see cref="PanelManager.RegisterFocusSource"/>.
    /// </summary>
    public class FocusRelay : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {
        private PanelManager _panel;
        private UIElementManager _element;
        private string _key;
        private InteractionState _state;

        public PanelManager Panel => _panel;
        public UIElementManager Element => _element;
        public string Key => _key;
        public InteractionState State => _state;

        public void Configure(PanelManager panel, UIElementManager element, string key) {
            _panel = panel;
            _element = element;
            _key = key;
        }

        public FocusRef ToFocusRef() {
            return new FocusRef(gameObject, _element, _key, _state);
        }

        public void OnSelect(BaseEventData eventData) {
            SetState(_state | InteractionState.Selected);
        }

        public void OnDeselect(BaseEventData eventData) {
            SetState(_state & ~InteractionState.Selected);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            SetState(_state | InteractionState.Highlighted);
        }

        public void OnPointerExit(PointerEventData eventData) {
            SetState(_state & ~InteractionState.Highlighted);
        }

        // Unity does not reliably send OnPointerExit or OnDeselect when
        // an object is deactivated.
        protected virtual void OnDisable() {
            SetState(InteractionState.None);
        }

        private void SetState(InteractionState next) {
            if (next == _state) return;
            InteractionState prev = _state;
            _state = next;
            if (_panel != null && _panel.Manager != null) {
                _panel.Manager.HandleFocusChange(this, prev, next);
            }
        }
    }
}
