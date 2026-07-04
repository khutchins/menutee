using System.Collections;

namespace Menutee {
	/// <summary>
	/// Animation strategy for the whole menu becoming visible or hidden.
	///
	/// The returned coroutine runs on the MenuManager and should yield until the
	/// animation is visually complete.
	///
	/// When <paramref name="instant"/> is true, apply the end state for the given
	/// direction immediately and return without yielding. This is used beneath
	/// the covers for making sure animations start out in the correct state. 
	/// Implementations must not yield when instant is true (use yield break).
	/// </summary>
	public interface IMenuVisibilityTransition {
		IEnumerator AnimateMenu(MenuManager menu, bool up, bool instant = false);
	}
}
