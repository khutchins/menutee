using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Menutee {
    public struct MenuAttributes {
        public CursorLockMode cursorLockMode;
        public bool cursorVisible;
        /// <summary>
        /// Sets the time scale while in the menu. If the value is negative, will not modify the existing time scale.
        /// </summary>
        public float timeScale;
        public bool pauseGame;

        /// <summary>
        /// Standard pause menu: Confined and visible cursor, timescale of 0, paused game.
        /// </summary>
        public static MenuAttributes StandardPauseMenu() {
            MenuAttributes attributes = new MenuAttributes();
            attributes.cursorLockMode = CursorLockMode.Confined;
            attributes.cursorVisible = true;
            attributes.timeScale = 0f;
            attributes.pauseGame = true;
            return attributes;
		}

        /// <summary>
        /// Standard pause menu: Locked and invisible cursor, uses existing time scale, unpaused game.
        /// </summary>
        public static MenuAttributes StandardInGame() {
            MenuAttributes attributes = new MenuAttributes();
            attributes.cursorLockMode = CursorLockMode.Locked;
            attributes.cursorVisible = false;
            attributes.timeScale = -1f;
            attributes.pauseGame = false;
            return attributes;
        }

        /// <summary>
        /// Standard non-pause menu: Confined and visible cursor, uses existing time scale, unpaused game.
        /// </summary>
        public static MenuAttributes StandardNonPauseMenu() {
            MenuAttributes attributes = new MenuAttributes();
            attributes.cursorLockMode = CursorLockMode.Confined;
            attributes.cursorVisible = true;
            attributes.timeScale = -1f;
            attributes.pauseGame = false;
            return attributes;
        }
    }

    public interface IMenu {
        MenuAttributes GetMenuAttributes();
        void SetMenuUp(bool newUp);
        void SetMenuOnTop(bool isTop);
	}

    public class MenuStack : MonoBehaviour {
        public static MenuStack Shared;

        [Header("Default Settings")]
        public CursorLockMode DefaultLockMode = CursorLockMode.Locked;
        public bool CursorVisible = false;

        [Header("Disable Functionality")]
        [Tooltip("Disables this script from modifying the cursor lock mode and visibility.")]
        public bool DisableCursorManagement = false;
        [Tooltip("Disables this script from modifying the time scale.")]
        public bool DisableTimeManagement = false;

        [Header("Callbacks")]
        [Tooltip("Callback for when a menu causes the game to go from unpaused to paused.")]
        public UnityEvent OnPause;
        [Tooltip("Callback for when a menu causes the game to go from paused to unpaused.")]
        public UnityEvent OnUnpause;

        private Stack<IMenu> _menuStack = new Stack<IMenu>();
        private Stack<MenuAttributes> _cachedMenuAttributes = new Stack<MenuAttributes>();

        private bool _paused;

        void Awake() {
            Shared = this;
            if (!DisableCursorManagement) {
                Cursor.lockState = DefaultLockMode;
                Cursor.visible = CursorVisible;
            }
            if (!DisableTimeManagement) {
                Time.timeScale = 1f;
            }
            _paused = false;
            OnUnpause?.Invoke();
        }

        /// <summary>
		/// Toggles the menu.
		/// 
		/// NOTE: This will only show the menu if it's not currently in the stack, and will not hide the menu if it's on top of the stack.
		/// </summary>
		/// <param name="menu">Menu to toggle.</param>
        public void ToggleMenu(IMenu menu) {
            if (!_menuStack.Contains(menu)) {
                PushAndShowMenu(menu);
			} else if (_menuStack.Count > 0 && _menuStack.Peek() == menu) {
                PopAndCloseMenu(menu);
			}
		}

        public bool PushAndShowMenu(IMenu menu) {
            if (menu == null) {
                Debug.LogWarning("Attempting to push a null menu!");
                return false;
            } else if (_menuStack.Contains(menu)) {
                Debug.LogWarning("Attempting to push menu already in stack.");
                return false;
			}

            CacheCurrentMenuAttributes();
            SetTopStatusOfTopOfStack(false);
            _menuStack.Push(menu);
            ApplyMenuAttributes(menu.GetMenuAttributes());
            menu.SetMenuUp(true);
            menu.SetMenuOnTop(true);
            return true;
        }

        private void SetTopStatusOfTopOfStack(bool newStatus) {
            if (_menuStack.Count == 0) return;
            _menuStack.Peek().SetMenuOnTop(newStatus);
		}

        public void PopAndPushNewMenu(IMenu current, IMenu newMenu) {
            PopAndCloseMenu(current);
            PushAndShowMenu(newMenu);
		}

        public bool PopAndCloseMenu(IMenu menu) {
            if (_menuStack.Count == 0) {
                Debug.LogWarning("Attempting to pop menu but stack is empty!");
                return false;
            } else if (_menuStack.Peek() != menu) {
                Debug.LogWarning("Attempting to pop menu not on top of stack!");
                return false;
            }
            PopAndApplyMenuAttributes();
            if (_menuStack.Count > 0) {
                IMenu top = _menuStack.Pop();
                top.SetMenuOnTop(false);
                top.SetMenuUp(false);
                SetTopStatusOfTopOfStack(true);
            }
            return true;
		}

        public int StackSize() {
            return _menuStack.Count;
		}

        public bool IsMenuInStack(IMenu menu) {
            return _menuStack.Contains(menu);
		}

        public bool IsMenuAtTop(IMenu menu) {
            return _menuStack.Peek() == menu;
		}

        public bool IsMenuUp(IMenu thisMenu) {
            foreach(IMenu menu in _menuStack) {
                if (thisMenu == menu) return true;
			}
            return false;
		}

        void UpdatePaused(bool newState) {
            if (newState != _paused) {
                _paused = newState;
                if (_paused) OnPause?.Invoke();
                else OnUnpause?.Invoke();
			}
		}

        void CacheCurrentMenuAttributes() {
            MenuAttributes attributes = new MenuAttributes();
            attributes.cursorLockMode = Cursor.lockState;
            attributes.cursorVisible = Cursor.visible;
            attributes.timeScale = Time.timeScale;
            attributes.pauseGame = _paused;
            _cachedMenuAttributes.Push(attributes);
        }

        void PopAndApplyMenuAttributes() {
            if (_cachedMenuAttributes.Count == 0) {
                Debug.LogWarning("Attempting to pop menu attributes but stack is empty!");
                return;
            }
            MenuAttributes attributes = _cachedMenuAttributes.Pop();
            ApplyMenuAttributes(attributes);
        }

        void ApplyMenuAttributes(MenuAttributes attributes) {
            if (!DisableCursorManagement) {
                Cursor.lockState = attributes.cursorLockMode;
                Cursor.visible = attributes.cursorVisible;
            }
            if (!DisableTimeManagement && attributes.timeScale >= 0) {
                Time.timeScale = attributes.timeScale;
            }
            UpdatePaused(attributes.pauseGame);
        }
    }
}