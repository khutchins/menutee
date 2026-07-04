using System.Collections;
using UnityEngine;

namespace Menutee {
	/// <summary>
	/// Fades the whole menu in and out via a CanvasGroup on the MenuManager's
	/// GameObject (added if absent).
	/// </summary>
	public class FadeMenuTransition : MenuTransitionBase, IMenuVisibilityTransition {

		/// <summary>Duration, in seconds, of the menu fade.</summary>
		public float MenuFadeTime = 0.15f;

		public IEnumerator AnimateMenu(MenuManager menu, bool up, bool instant = false) {
			CanvasGroup group = GetOrAddCanvasGroup(menu.gameObject);
			float to = up ? 1f : 0f;
			if (instant) {
				group.alpha = to;
				yield break;
			}
			float from = up ? 0f : group.alpha;
			if (up) {
				group.alpha = 0f;
			}
			yield return Tween(MenuFadeTime, p => group.alpha = Mathf.Lerp(from, to, p));
			group.alpha = to;
		}
	}
}
