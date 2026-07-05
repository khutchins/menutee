namespace Menutee {
    /// <summary>
    /// Passed to a panel- or menu-level focus callback when the resolved focused
    /// element changes. Either side has <see cref="FocusRef.HasFocus"/> false when
    /// focus is arriving from, or leaving to, nothing.
    /// </summary>
    public readonly struct FocusChange {
        public readonly FocusRef Previous;
        public readonly FocusRef Current;
        /// <summary>What caused this focus change. See <see cref="FocusSource"/>.</summary>
        public readonly FocusSource Source;

        public FocusChange(FocusRef previous, FocusRef current, FocusSource source) {
            Previous = previous;
            Current = current;
            Source = source;
        }
    }
}
