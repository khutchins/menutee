using System.Collections;
using UnityEngine;

namespace Menutee {
	/// <summary>
	/// Slides the whole menu on and offscreen from the given edge.
	///
	/// It moves a content RectTransform: an explicitly assigned <see cref="Content"/>
	/// if set, otherwise the MenuGenerator's panel container
	/// (<see cref="MenuGenerator.Parent"/>), otherwise the MenuManager's own rect.
	/// Note the content must be a movable rect (e.g. a Screen Space Overlay canvas root
	/// has a driven position and won't visibly move.)
	///
	/// The onscreen position is captured on first show; the offscreen is that offset
	/// by one canvas dimension along the chosen edge (or <see cref="DistanceOverride"/>
	/// if set), so it fully clears the screen.
	/// </summary>
	public class SlideMenuTransition : MenuTransitionBase, IMenuVisibilityTransition {

		/// <summary>Edge the menu slides in from (and back out to).</summary>
		public TransitionEdge FromEdge = TransitionEdge.Bottom;
		/// <summary>Duration, in seconds, of the slide.</summary>
		public float MenuSlideTime = 0.2f;
		/// <summary>Offscreen travel distance in anchored units. If &lt;= 0, uses the canvas size along the slide axis.</summary>
		public float DistanceOverride = 0f;
		/// <summary>Also fade the menu CanvasGroup while sliding.</summary>
		public bool FadeWhileSliding = false;
		/// <summary>Explicit content to slide. If null, resolves to the panel container then the menu rect.</summary>
		public RectTransform Content;

		private Vector2 _rest;
		private bool _hasRest;

		public IEnumerator AnimateMenu(MenuManager menu, bool up, bool instant = false) {
			RectTransform rect = ResolveContent(menu);
			if (rect == null) {
				yield break;
			}

			// Capture the authored onscreen position the first time, before we've
			// moved it. All motion is then driven to absolute positions from it.
			if (!_hasRest) {
				_rest = rect.anchoredPosition;
				_hasRest = true;
			}

			Vector2 offscreen = _rest + OffscreenOffset(menu, rect);
			Vector2 to = up ? _rest : offscreen;

			CanvasGroup group = FadeWhileSliding ? GetOrAddCanvasGroup(menu.gameObject) : null;

			if (instant) {
				rect.anchoredPosition = to;
				if (group != null) group.alpha = up ? 1f : 0f;
				yield break;
			}

			Vector2 from = up ? offscreen : _rest;
			rect.anchoredPosition = from;
			if (group != null && up) {
				group.alpha = 0f;
			}

			yield return Tween(MenuSlideTime, p => {
				float e = EaseInOut(p);
				rect.anchoredPosition = Vector2.Lerp(from, to, e);
				if (group != null) group.alpha = up ? e : 1f - e;
			});

			rect.anchoredPosition = to;
			if (group != null) group.alpha = up ? 1f : 0f;
		}

		private RectTransform ResolveContent(MenuManager menu) {
			if (Content != null) {
				return Content;
			}
			if (menu.Generator != null && menu.Generator.Parent != null) {
				RectTransform parentRect = menu.Generator.Parent.transform as RectTransform;
				if (parentRect != null) {
					return parentRect;
				}
			}
			return menu.transform as RectTransform;
		}

		private Vector2 OffscreenOffset(MenuManager menu, RectTransform rect) {
			RectTransform canvasRect = menu.Canvas != null ? menu.Canvas.transform as RectTransform : null;
			Vector2 size = canvasRect != null ? canvasRect.rect.size : rect.rect.size;
			Vector2 dir = EdgeDirection(FromEdge);
			// Distance along the slide axis, big enough to fully clear the screen.
			float axisSize = dir.x != 0f ? size.x : size.y;
			float dist = DistanceOverride > 0f ? DistanceOverride : axisSize;
			return dir * dist;
		}
	}
}
