using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menutee {
	public class UIElementManager : MonoBehaviour {
		[HideInInspector] public int Id;
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

        public virtual bool ConsumesCancelInput {
            get => false;
        }

        /// <summary>
        /// Returns true while this element is in a state where the menu's
        /// toggle input (e.g. pause/menu key) should be ignored. Useful for
        /// elements like text inputs that may capture the same key as a
        /// character (e.g. Space typed into a focused field).
        /// </summary>
        public virtual bool BlocksMenuToggle {
            get => false;
        }

        /// <summary>
        /// Gives an element the chance to react to UI cancel input before the
        /// containing menu uses it to pop a panel. Return true if the element
        /// handled (or wants to swallow) the input. The default implementation
        /// preserves the legacy <see cref="ConsumesCancelInput"/> behavior:
        /// elements that report consumption simply suppress the panel pop
        /// without taking any action.
        /// </summary>
        public virtual bool TryHandleCancelInput() {
            return ConsumesCancelInput;
        }
    }
}
