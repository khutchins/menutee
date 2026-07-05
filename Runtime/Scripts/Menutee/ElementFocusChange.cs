namespace Menutee {
    /// <summary>
    /// Passed to a per-object focus callback when an element becomes, or stops
    /// being, the menu's currently focused element.
    /// </summary>
    public readonly struct ElementFocusChange {
        public readonly UIElementManager Element;
        /// <summary>True when the element gained focus, false when it lost it.</summary>
        public readonly bool IsFocused;
        /// <summary>What caused this focus change. See <see cref="FocusSource"/>.</summary>
        public readonly FocusSource Source;

        public ElementFocusChange(UIElementManager element, bool isFocused, FocusSource source) {
            Element = element;
            IsFocused = isFocused;
            Source = source;
        }
    }
}
