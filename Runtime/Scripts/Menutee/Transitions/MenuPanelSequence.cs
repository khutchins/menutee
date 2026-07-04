namespace Menutee {
	/// <summary>
	/// How the menu-visibility animation composes with the active panel's
	/// enter/exit animation when the menu opens or closes.
	/// </summary>
	public enum MenuPanelSequence {
		/// <summary>
		/// Only the menu animates - the panel is placed (open) or cleared (close)
		/// instantly.
		/// </summary>
		None,

		/// <summary>
		/// Sequences the two animations. On open: the menu animates in, then the
		/// root panel plays its enter animation. On close: the active panel plays
		/// its exit animation, then the menu animates out.
		/// </summary>
		Ordered,
	}
}
