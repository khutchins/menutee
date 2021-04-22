using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Menutee {
    public class MenuHook : MonoBehaviour, IMenu {
		[Header("General")]
		public bool ShowOnStart = false;
		[Tooltip("The canvas element. If this is set, this script will automatically hide/show the canvas when appropriate.")]
		public Canvas Canvas;

		[Header("Menu Attributes")]
		public CursorLockMode CursorLockMode = CursorLockMode.None;
		public bool CursorVisible = true;
		public bool PausesGame = true;
		[Tooltip("Time scale to use when in menu. If negative, it will use the existing time scale.")]
		public float TimeScale = 0;

		[Header("Hooks")]
		[Tooltip("Callbacks for when the menu is shown.")]
		public UnityEvent OnMenuOpen;
		[Tooltip("Callbacks for when the menu is hidden.")]
		public UnityEvent OnMenuClose;
		[Tooltip("Callbacks for when the menu has become the top menu on the stack.")]
		public UnityEvent OnMenuTop;
		[Tooltip("Callbacks for when the menu is no longer the top menu on the stack.")]
		public UnityEvent OnMenuNotTop;

		void Awake() {
			if (Canvas != null) Canvas.enabled = false;
		}

		void Start() {
			if (ShowOnStart) {
				PushMenu();
			}
		}

		public MenuAttributes GetMenuAttributes() {
			MenuAttributes attributes = new MenuAttributes {
				cursorLockMode = CursorLockMode,
				cursorVisible = CursorVisible,
				pauseGame = PausesGame,
				timeScale = TimeScale
			};
			return attributes;
		}

		public void SetMenuUp(bool newUp) {
			if (Canvas != null) Canvas.enabled = newUp;
			if (newUp) OnMenuOpen?.Invoke();
			else OnMenuClose?.Invoke();
		}

		public void SetMenuOnTop(bool newOnTop) {
			if (newOnTop) OnMenuTop?.Invoke();
			else OnMenuNotTop?.Invoke();
		}

		public void PushMenu() {
			MenuStack.Shared.PushAndShowMenu(this);
		}

		public void PopMenu() {
			MenuStack.Shared.PopAndCloseMenu(this);
		}

	}
}