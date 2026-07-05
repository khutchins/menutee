namespace Menutee {
    /// <summary>
    /// Passed to a per-object focus callback when an element becomes, or stops
    /// being, the menu's currently focused element.
    /// </summary>
    public readonly struct ElementFocusChange {
        public readonly UIElementManager Element;
        /// <summary>True when the element gained focus, false when it lost it.</summary>
        public readonly bool IsFocused;

        public ElementFocusChange(UIElementManager element, bool isFocused) {
            Element = element;
            IsFocused = isFocused;
        }
    }
}
