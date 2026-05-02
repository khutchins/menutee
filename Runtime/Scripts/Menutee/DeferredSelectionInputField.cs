using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Menutee {
	/// <summary>
	/// Input field variant that defers entering edit mode until the user
	/// explicitly submits.
	/// </summary>
	public class DeferredSelectionInputField : TMP_InputField {
		public bool RequireSubmitToEdit = true;

		public override void OnSelect(BaseEventData eventData) {
			if (!RequireSubmitToEdit) {
				base.OnSelect(eventData);
				return;
			}

			DoStateTransition(SelectionState.Selected, false);
		}
	}
}
