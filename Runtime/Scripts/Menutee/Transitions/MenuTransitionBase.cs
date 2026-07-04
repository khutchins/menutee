using System;
using System.Collections;
using UnityEngine;

namespace Menutee {
	/// <summary>
	/// Shared helpers for transition implementations. Runs in unscaled time by
	/// default (see <see cref="UseUnscaledTime"/>).
	/// </summary>
	public abstract class MenuTransitionBase {

		/// <summary>
		/// Use unscaled time (the default). Pause menus set timeScale to 0, so scaled
		/// time would freeze the animation - leave this true for anything that pauses.
		/// Set false for diegetic UIs that should slow with game time dilation.
		/// </summary>
		public bool UseUnscaledTime = true;

		/// <summary>Frame delta respecting <see cref="UseUnscaledTime"/>.</summary>
		protected float DeltaTime => UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

		/// <summary>
		/// Simplified version of EZTween with just smoothstep.
		/// </summary>
		protected IEnumerator Tween(float duration, Action<float> step) {
			if (duration <= 0f) {
				step(1f);
				yield break;
			}
			float t = 0f;
			while (t < 1f) {
				t += DeltaTime / duration;
				step(Mathf.Clamp01(t));
				yield return null;
			}
			step(1f);
		}

		protected static float EaseInOut(float t) {
			return t * t * (3f - 2f * t);
		}

		/// <summary>
		/// Unit vector pointing outward toward the given edge - the direction an
		/// element travels from when it enters from that edge.
		/// </summary>
		protected static Vector2 EdgeDirection(TransitionEdge edge) {
			switch (edge) {
				case TransitionEdge.Top: return new Vector2(0f, 1f);
				case TransitionEdge.Left: return new Vector2(-1f, 0f);
				case TransitionEdge.Right: return new Vector2(1f, 0f);
				case TransitionEdge.Bottom:
				default: return new Vector2(0f, -1f);
			}
		}

		protected static CanvasGroup GetOrAddCanvasGroup(GameObject go) {
			if (!go.TryGetComponent<CanvasGroup>(out var cg)) {
				cg = go.AddComponent<CanvasGroup>();
			}
			return cg;
		}

		protected static RectTransform AsRect(Component c) {
			return c != null ? c.transform as RectTransform : null;
		}
	}
}
