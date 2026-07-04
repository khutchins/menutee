using System.Collections;
using UnityEngine;

namespace Menutee {
	/// <summary>
	/// Crossfades between panels: the incoming panel fades in while the outgoing
	/// panel fades out, both visible during the dissolve. Fades a CanvasGroup
	/// on each panel GameObject, adding one if absent.
	/// </summary>
	public class FadePanelTransition : MenuTransitionBase, IPanelTransition {

		/// <summary>Duration, in seconds, of the crossfade.</summary>
		public float FadeTime = 0.15f;

		public IEnumerator AnimatePanel(MenuManager menu, PanelManager oldPanel, PanelManager newPanel, bool fromPush) {
			CanvasGroup oldGroup = oldPanel != null ? GetOrAddCanvasGroup(oldPanel.gameObject) : null;
			CanvasGroup newGroup = newPanel != null ? GetOrAddCanvasGroup(newPanel.gameObject) : null;

			float oldFrom = oldGroup != null ? oldGroup.alpha : 0f;
			if (newGroup != null) {
				newGroup.alpha = 0f;
			}

			yield return Tween(FadeTime, p => {
				if (newGroup != null) newGroup.alpha = p;
				if (oldGroup != null) oldGroup.alpha = Mathf.Lerp(oldFrom, 0f, p);
			});

			if (newGroup != null) newGroup.alpha = 1f;
			// Restore the outgoing panel to full alpha - it gets disabled by the
			// MenuManager, so this just keeps it correct for its next appearance.
			if (oldGroup != null) oldGroup.alpha = 1f;
		}
	}
}
