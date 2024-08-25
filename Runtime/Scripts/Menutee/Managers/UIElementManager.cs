using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menutee {
	public class UIElementManager : MonoBehaviour {
		public GameObject SelectableObject;
		public PanelObjectConfig PanelObjectConfig;

		public virtual void SetColors(PaletteConfig config) {
			if (config != null) {
				foreach (Selectable select in GetComponentsInChildren<Selectable>()) {
					config.ApplyToSelectable(select);
				}
				foreach (IPaletteReceptor receptor in GetComponentsInChildren<IPaletteReceptor>()) {
					config.ApplyToReceptor(receptor);
				}
#pragma warning disable CS0618 // Type or member is obsolete
				foreach (HighlightTextWhenSelected highlight in GetComponentsInChildren<HighlightTextWhenSelected>()) {
#pragma warning restore CS0618 // Type or member is obsolete
					highlight.SelectColor = config.SelectedColor;
				}
			}
		}
	}
}
