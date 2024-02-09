using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Menutee {
	[CreateAssetMenu(menuName = "Menutee/Menu Input Mediator - Unity")]
	public class UnityMenuInputMediator : MenuInputMediator {
		public string xAxis = "Horizontal";
		public string yAxis = "Vertical";
		public string uiInputCancel = "Cancel";
		public string menuToggle = "Jump";
		public string submit = "Submit";

		private float GetAxis(string name) {
			if (string.IsNullOrEmpty(name)) return 0;
			return Input.GetAxisRaw(name);
		}

		private bool GetButtonDown(string name) {
			if (string.IsNullOrEmpty(name)) return false;
			return Input.GetButtonDown(name);
		}

		public override float UIX() {
			return GetAxis(xAxis);
		}

		public override float UIY() {
			return GetAxis(yAxis);
		}


		public override bool UICancelDown() {
			return GetButtonDown(uiInputCancel);
		}

		public override bool UISubmitDown() {
			return GetButtonDown(submit);
		}

		public override bool MenuToggleDown() {
			return GetButtonDown(menuToggle);
		}

		public override InputType LastInputType {
			// I don't know how to get the last input type in straight
			// Unity (I use Rewired), so stick with a type that won't
			// have any adverse effect.
			get => InputType.Unknown;
		}
	}
}