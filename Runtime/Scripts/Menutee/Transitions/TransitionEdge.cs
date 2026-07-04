namespace Menutee {
	/// <summary>
	/// A screen edge, used by directional transitions to say which side an element
	/// or panel enters from (and exits to). Shared by the menu and panel slide
	/// transitions. See <see cref="MenuTransitionBase.EdgeDirection"/> for the
	/// corresponding unit direction.
	/// </summary>
	public enum TransitionEdge {
		Top,
		Bottom,
		Left,
		Right,
	}
}
