namespace Menutee {
    /// <summary>
    /// What caused a focus change. Lets callbacks tell an intentional user move
    /// (keyboard/gamepad navigation, mouse hover, or a restore in direct response
    /// to directional input) apart from a focus change the menu made on its own
    /// (panel changes, menu open/close, or an automatic restore when the selection
    /// was lost).
    /// </summary>
    public enum FocusSource {
        UserInput,
        Programmatic,
    }
}
