using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	[CreateAssetMenu(menuName = "Menutee/UnityMenuInputMediator")]
	public class UnityMenuInputMediator : MenuInputMediator {
		public string xAxis = "Horizontal";
		public string yAxis = "Vertical";
		public string uiInputCancel = "Cancel";
		public string menuToggle = "Jump";
		public string submit = "Submit";

		public override float UIX() {
			return UnityEngine.Input.GetAxisRaw(xAxis);
		}

		public override float UIY() {
			return UnityEngine.Input.GetAxisRaw(yAxis);
		}

		public override bool UICancelDown() {
			return UnityEngine.Input.GetButtonDown(uiInputCancel);
		}

		public override bool UISubmitDown() {
			return UnityEngine.Input.GetButtonDown(submit);
		}

		public override bool MenuToggleDown() {
			return UnityEngine.Input.GetButtonDown(menuToggle);
		}
	}
}