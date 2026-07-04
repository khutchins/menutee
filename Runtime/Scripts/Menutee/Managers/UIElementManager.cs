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

		private PaletteConfigReference _paletteReference;

		/// <summary>
		/// Binds this element to the given palette reference (or none). While bound,
		/// the element re-applies its colors whenever the referenced palette changes.
		/// The generator resolves palette precedence and calls this after assigning
		/// PanelObjectConfig, since the reference isn't known yet when the element's
		/// OnEnable first runs during instantiation.
		/// </summary>
		public void BindPaletteReference(PaletteConfigReference reference) {
			_paletteReference = reference;
			ResubscribeToPaletteReference();
			// SetColors no-ops on null, so a null-valued reference leaves the palette
			// applied at generation time untouched.
			if (_paletteReference != null) {
				SetColors(_paletteReference.Value);
			}
		}

		protected virtual void OnEnable() {
			ResubscribeToPaletteReference();
			if (_paletteReference != null) {
				SetColors(_paletteReference.Value);
			}
		}

		protected virtual void OnDisable() {
			UnsubscribeFromPaletteReference();
		}

		private void ResubscribeToPaletteReference() {
			UnsubscribeFromPaletteReference();
			if (_paletteReference != null) {
				_paletteReference.ValueChanged += OnPaletteReferenceChanged;
			}
		}

		private void UnsubscribeFromPaletteReference() {
			if (_paletteReference != null) {
				_paletteReference.ValueChanged -= OnPaletteReferenceChanged;
			}
		}

		private void OnPaletteReferenceChanged(PaletteConfig palette) {
			SetColors(palette);
		}

		public virtual void SetColors(PaletteConfig config) {
			if (config != null) {
				foreach (Selectable select in GetComponentsInChildren<Selectable>()) {
					config.ApplyToSelectable(select);
				}
				foreach (IPaletteReceptor receptor in GetComponentsInChildren<IPaletteReceptor>()) {
					config.ApplyToReceptor(receptor);
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
