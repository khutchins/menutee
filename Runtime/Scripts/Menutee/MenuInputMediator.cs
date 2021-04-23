using UnityEngine;

namespace Menutee {
	public abstract class MenuInputMediator : ScriptableObject {
		public abstract float UIX();
		public abstract float UIY();

		public abstract bool UISubmitDown();
		public abstract bool UICancelDown();
		public abstract bool MenuToggleDown();
	}
}