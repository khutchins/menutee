namespace Menutee {
    /// <summary>
    /// The local interaction state of a single selectable.
    /// </summary>
    [System.Flags]
    public enum InteractionState {
        None = 0,
        Selected = 1 << 0,
        Highlighted = 1 << 1,
    }
}
