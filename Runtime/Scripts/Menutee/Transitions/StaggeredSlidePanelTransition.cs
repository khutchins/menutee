using System.Collections;
using UnityEngine;

namespace Menutee {
	/// <summary>
	/// Animates a panel's elements individually instead of as a rigid block: the
	/// old panel fades out, then each element of the new panel slides and fades in
	/// on its own timeline, offset from the previous one by
	/// <see cref="PerElementDelay"/>.
	///
	/// The coroutine yields until the last (most-delayed) element has finished.
	/// </summary>
	public class StaggeredSlidePanelTransition : MenuTransitionBase, IPanelTransition {

		/// <summary>Edge each element slides in from.</summary>
		public TransitionEdge FromEdge = TransitionEdge.Bottom;
		/// <summary>Distance each element travels as it slides in.</summary>
		public float ElementOffset = 120f;
		/// <summary>Duration, in seconds, of a single element's slide/fade.</summary>
		public float ElementAnimTime = 0.18f;
		/// <summary>Delay, in seconds, between successive elements starting.</summary>
		public float PerElementDelay = 0.05f;
		/// <summary>Duration, in seconds, of the outgoing panel's fade-out.</summary>
		public float ExitFadeTime = 0.12f;

		public IEnumerator AnimatePanel(MenuManager menu, PanelManager oldPanel, PanelManager newPanel, bool fromPush) {
			CanvasGroup oldGroup = oldPanel != null ? GetOrAddCanvasGroup(oldPanel.gameObject) : null;

			UIElementManager[] elements = newPanel != null
				? (newPanel.ElementManagers ?? new UIElementManager[0])
				: new UIElementManager[0];
			int n = elements.Length;
			var rects = new RectTransform[n];
			var groups = new CanvasGroup[n];
			var rest = new Vector2[n];
			var start = new Vector2[n];
			Vector2 enterOffset = EdgeDirection(FromEdge) * ElementOffset;
			for (int i = 0; i < n; i++) {
				if (elements[i] == null) continue;
				rects[i] = AsRect(elements[i]);
				groups[i] = GetOrAddCanvasGroup(elements[i].gameObject);
				groups[i].alpha = 0f;
				if (rects[i] != null) {
					rest[i] = rects[i].anchoredPosition;
					start[i] = rest[i] + enterOffset;
					rects[i].anchoredPosition = start[i];
				}
			}

			// Exit the old panel as a block. Hold it transparent afterward; its alpha
			// is restored at the very end, just before the MenuManager disables it —
			// restoring earlier would leave it visible over the new panel.
			if (oldGroup != null) {
				float fromAlpha = oldGroup.alpha;
				yield return Tween(ExitFadeTime, p => oldGroup.alpha = Mathf.Lerp(fromAlpha, 0f, p));
			}

			// Stagger the incoming panel's elements in.
			if (n > 0) {
				float total = Mathf.Max(0, n - 1) * PerElementDelay + ElementAnimTime;
				float elapsed = 0f;
				while (elapsed < total) {
					elapsed += DeltaTime;
					for (int i = 0; i < n; i++) {
						if (rects[i] == null && groups[i] == null) continue;
						// Each element's local progress, offset by its stagger delay.
						float local = ElementAnimTime <= 0f ? 1f
							: Mathf.Clamp01((elapsed - i * PerElementDelay) / ElementAnimTime);
						float e = EaseInOut(local);
						if (groups[i] != null) groups[i].alpha = local;
						if (rects[i] != null) rects[i].anchoredPosition = Vector2.Lerp(start[i], rest[i], e);
					}
					yield return null;
				}

				// Snap everything to its resting state.
				for (int i = 0; i < n; i++) {
					if (groups[i] != null) groups[i].alpha = 1f;
					if (rects[i] != null) rects[i].anchoredPosition = rest[i];
				}
			}

			if (oldGroup != null) {
                // Restore alpha for the next time it's shown
                oldGroup.alpha = 1f;
			}
		}
	}
}
