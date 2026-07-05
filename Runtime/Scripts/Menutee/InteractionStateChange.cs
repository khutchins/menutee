namespace Menutee {
    /// <summary>
    /// Passed to a per-object interaction callback when an element's own
    /// selected/highlighted state changes. Reflects only this element's local
    /// state, independent of what else is focused.
    /// </summary>
    public readonly struct InteractionStateChange {
        public readonly UIElementManager Element;
        public readonly InteractionState Previous;
        public readonly InteractionState Current;

        public InteractionStateChange(UIElementManager element, InteractionState previous, InteractionState current) {
            Element = element;
            Previous = previous;
            Current = current;
        }

        /// <summary>True if the given flag was off before and is on now.</summary>
        public bool Gained(InteractionState flag) => (Previous & flag) == 0 && (Current & flag) != 0;

        /// <summary>True if the given flag was on before and is off now.</summary>
        public bool Lost(InteractionState flag) => (Previous & flag) != 0 && (Current & flag) == 0;
    }
}
