using System.Collections;
using UnityEngine;

namespace Menutee {
	/// <summary>
	/// Slides the panel as a whole: the outgoing panel slides off one side 
	/// while the incoming panel slides in from the other. Push and pop move
	/// in opposite directions.
	///
	/// Assumes both panels share the same anchored position, which  is the
	/// case when panels are stacked siblings in the same container.
	/// </summary>
	public class SlidePanelTransition : MenuTransitionBase, IPanelTransition {

		/// <summary>Edge the incoming panel enters from on a push. A pop reverses it (enters from the opposite edge).</summary>
		public TransitionEdge FromEdge = TransitionEdge.Right;
		/// <summary>Slide distance in anchored units. Should exceed the panel size along the slide axis.</summary>
		public float SlideDistance = 800f;
		/// <summary>Duration, in seconds, of the panel slide.</summary>
		public float PanelSlideTime = 0.2f;

		public IEnumerator AnimatePanel(MenuManager menu, PanelManager oldPanel, PanelManager newPanel, bool fromPush) {
			RectTransform newRect = AsRect(newPanel);
			RectTransform oldRect = AsRect(oldPanel);
			if (newRect == null) {
				yield break;
			}

			// Push slides the new panel in from FromEdge; pop reverses (from the
			// opposite edge). The old panel slides out the other way.
			Vector2 offset = (fromPush ? 1f : -1f) * SlideDistance * EdgeDirection(FromEdge);

			Vector2 newRest = newRect.anchoredPosition;
			Vector2 newFrom = newRest + offset;
			newRect.anchoredPosition = newFrom;

			Vector2 oldRest = oldRect != null ? oldRect.anchoredPosition : Vector2.zero;
			Vector2 oldTo = oldRest - offset;

			yield return Tween(PanelSlideTime, p => {
				float e = EaseInOut(p);
				newRect.anchoredPosition = Vector2.Lerp(newFrom, newRest, e);
				if (oldRect != null) {
					oldRect.anchoredPosition = Vector2.Lerp(oldRest, oldTo, e);
				}
			});

			// Snap to final and restore the old panel's rest position so it's
			// correctly placed the next time it is shown.
			newRect.anchoredPosition = newRest;
			if (oldRect != null) {
				oldRect.anchoredPosition = oldRest;
			}
		}
	}
}
