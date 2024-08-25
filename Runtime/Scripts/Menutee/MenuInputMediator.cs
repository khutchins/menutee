using UnityEngine;

namespace Menutee {
	public abstract class MenuInputMediator : ScriptableObject {
		public enum InputType {
			Unknown = 0,
			Mouse = 1,
			Keyboard = 2,
			Controller = 3
		}

		public abstract float UIX();
		public abstract float UIY();

		public abstract bool UISubmitDown();
		public abstract bool UICancelDown();
		public abstract bool MenuToggleDown();
		public virtual InputType LastInputType {
			get => InputType.Unknown;
		}
	}
}