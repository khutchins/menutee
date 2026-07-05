using UnityEngine;

namespace Menutee {
    /// <summary>
    /// Describes the element that is currently focused (or, if
    /// <see cref="HasFocus"/> is false, that nothing is). Passed to the panel-
    /// and menu-level focus callbacks. The <see cref="Object"/> is always
    /// meaningful for a focused element. <see cref="Element"/> is null for custom
    /// selectables registered via <see cref="PanelManager.RegisterFocusSource"/>.
    /// <see cref="Key"/> is an optional identity handle: the object config's key
    /// for generated elements, or the key supplied at registration for custom 
    /// sources.
    /// </summary>
    public readonly struct FocusRef {
        public readonly GameObject Object;
        public readonly UIElementManager Element;
        public readonly string Key;
        public readonly InteractionState State;

        public bool HasFocus => Object != null;

        public FocusRef(GameObject obj, UIElementManager element, string key, InteractionState state) {
            Object = obj;
            Element = element;
            Key = key;
            State = state;
        }
    }
}
