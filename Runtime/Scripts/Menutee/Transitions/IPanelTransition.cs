using System.Collections;

namespace Menutee {
	/// <summary>
	/// Animation strategy for a change from one panel to another within a menu.
	///
	/// The returned coroutine is run on the MenuManager and should yield until the
	/// animation is visually complete. The new panel is already enabled and
	/// selectable when called; the old panel is disabled once the coroutine
	/// finishes. Panel transitions only run for push/pop while the menu is open —
	/// opening and closing the menu place panels instantly and are handled by the
	/// menu visibility transition.
	/// </summary>
	public interface IPanelTransition {
		IEnumerator AnimatePanel(MenuManager menu, PanelManager oldPanel, PanelManager newPanel, bool fromPush);
	}
}
