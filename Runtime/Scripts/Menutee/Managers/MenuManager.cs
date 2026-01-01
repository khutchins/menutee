using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using System.Collections;

namespace Menutee {
	public class MenuManager : MonoBehaviour, IMenu {

        public delegate void MenuStateChangedHandler(MenuManager manager);

        /// <summary>
        /// Called when the menu is added to the MenuStack and becomes active.
        /// </summary>
        public event MenuStateChangedHandler MenuOpened;
        /// <summary>
        /// Called when the menu is removed from the MenuStack.
        /// </summary>
        public event MenuStateChangedHandler MenuClosed;
        /// <summary>
        /// Called when this menu becomes the top-most menu in the stack (receives focus).
        /// </summary>
        public event MenuStateChangedHandler MenuEnteredTop;
        /// <summary>
        /// Called when another menu is pushed on top of this one, or this menu is being closed.
        /// </summary>
        public event MenuStateChangedHandler MenuExitedTop;


        private bool _active;

		[Tooltip("The canvas element. If this is not set, this script will try to get the canvas from the game object it is on.")]
		public Canvas Canvas;

		[Tooltip("If true, a CanvasGroup will be added/used to control interactivity only when on top.")]
		public bool ManageInteractivity = true;

        [HideInInspector]
		public PanelManager[] Panels;
		public MenuConfig MenuConfig;

		protected Stack<string> _panelStack = new Stack<string>();
		protected Stack<GameObject> _selectedStack = new Stack<GameObject>();
		private GameObject _activeDefaultInput;

		public MenuInputMediator InputMediator;

		private bool _disabled;
		private string _activeKey;
		private PanelManager _activeManager;
		private GameObject _cachedSelection;
        private GameObject _lastValidSelection;
        private CanvasGroup _canvasGroup;

        private void Awake() {
			if (Canvas == null) {
				Canvas = GetComponent<Canvas>();
			}

			if (ManageInteractivity) {
				_canvasGroup = GetComponent<CanvasGroup>();
				if (_canvasGroup == null) {
					_canvasGroup = gameObject.AddComponent<CanvasGroup>();
				}
			}
        }

		private void Start() {
			SetMenuUp(false);

			// This needs to be checked on Start to guarantee
			// that MenuStack has had time to register its singleton.
			if (MenuStack.Shared == null) {
				Debug.LogError("No MenuStack in scene. Menu disabled.");
				_disabled = true;
				return;
			} else if (InputMediator == null) {
				Debug.LogError("No input mediator set on MenuManager. Menu disabled.");
				_disabled = true;
				return;
			} else if (Canvas == null) {
				Debug.LogError("No canvas element set or found in MenuManager. Menu disabled.");
			}

			if (MenuConfig.StartsOpen) {
				MenuStack.Shared.PushAndShowMenu(this);
			}
		}

		public MenuAttributes GetMenuAttributes() {
			return MenuConfig.MenuAttributes;
		}

		/// <summary>
		/// Delays actions in the helper for a frame, since some things *COUGH* audio mixers *COUGH*
		/// don't handle being set in awake properly. Useful for calling things from creation callbacks
		/// for objects.
		/// </summary>
		/// <param name="doer">Thing to do after a frame.</param>
		public void RunGenericActionAfterFrame(Action doer) {
			StartCoroutine(GenericCoroutine(doer));
        }

		private IEnumerator GenericCoroutine(Action doer) {
			yield return null;
			doer?.Invoke();
		}

		public void SetMenuUp(bool newUp) {
			// Menu is closed.
			if (!newUp) {
				_cachedSelection = null;
				_panelStack.Clear();
			}
			_active = newUp;
			SetMenuIsUp(newUp, _active ? MenuConfig.MainPanelKey : null);

            if (newUp) {
                MenuOpened?.Invoke(this);
            } else {
                MenuClosed?.Invoke(this);
            }
        }

		public void SetMenuOnTop(bool newOnTop) {
			if (!newOnTop) {
				_cachedSelection = EventSystem.current.currentSelectedGameObject;
				EventSystem.current.SetSelectedGameObject(null);
			} else if(_cachedSelection != null) {
				EventSystem.current.SetSelectedGameObject(_cachedSelection);
			}
			SetOnTop(newOnTop);

            if (newOnTop) {
                MenuEnteredTop?.Invoke(this);
            } else {
                MenuExitedTop?.Invoke(this);
            }
        }

		protected virtual void SetMenuIsUp(bool isUp, string newKey) {
			Canvas.enabled = isUp;
            ActivatePanel(newKey, true);
		}

		protected virtual void SetOnTop(bool isOnTop) {
			_activeManager?.SetPanelActive(isOnTop);
			Canvas.enabled = isOnTop;
            if (_canvasGroup != null) {
                _canvasGroup.interactable = isOnTop;
                _canvasGroup.blocksRaycasts = isOnTop;
            }
        }

		void ToggleMenu() {
			if (!MenuConfig.Toggleable || _disabled) {
				return;
			}
			MenuStack.Shared.ToggleMenu(this);
		}

		protected virtual void DoToggle() {
			MenuStack.Shared.ToggleMenu(this);
		}

		protected virtual void ActivatePanel(PanelManager oldPanel, PanelManager newPanel, bool fromPush) {
			EnablePanel(newPanel, fromPush);
			DisableOtherPanels(newPanel);
		}

		private bool ShouldHaveDefaultSelection {
			get {
				return MenuConfig.DefaultSelectMode == MenuConfig.SelectMode.Always ||
					(MenuConfig.DefaultSelectMode == MenuConfig.SelectMode.Contextual
					&& InputMediator.LastInputType != MenuInputMediator.InputType.Mouse);
			}
		}

		protected void EnablePanel(PanelManager panel, bool fromPush) {
			EventSystem.current.SetSelectedGameObject(null);
			string oldKey = _activeKey;
			_activeKey = panel != null ? panel.Key : null;

			PanelManager active = panel;
			active?.SetPanelActive(true);

			if (active != null) {
				// Cache the default so that we have something to default to if
				// nothing is selected.
				_activeDefaultInput = active.DefaultInput;
                _lastValidSelection = _activeDefaultInput;

				if (!ShouldHaveDefaultSelection) {
					// No selection updated on push or pop if disabled.
					// The selected game object is cleared above, so no
					// action is required here.
				}
				// If pushing or something is wrong with the selected stack, use the default.
				else if (fromPush || _selectedStack.Count == 0 || _selectedStack.Peek() == null) {
					if (_activeDefaultInput != null) {
						EventSystem.current.SetSelectedGameObject(_activeDefaultInput);
					}
				} 
				// Otherwise, restore the previous selection.
				else {
					GameObject selected = _selectedStack.Pop();
					EventSystem.current.SetSelectedGameObject(selected);
                }
			} else {
				_activeDefaultInput = null;
			}
			_activeManager = active;

			if (oldKey != _activeKey) {
				foreach (System.Action<string, string> action in MenuConfig.PanelChangeCallbacks) {
					action.Invoke(oldKey, _activeKey);
				}
			}
		}

		protected void DisableOtherPanels(PanelManager active) {
			foreach (PanelManager manager in Panels) {
				if (active != manager) {
					manager.SetPanelActive(false);
                }
			}
		}

		private PanelManager PanelForKey(string key) {
			return Panels.Where(x => x.Key == key).FirstOrDefault();
		}

		protected void ActivatePanel(string key, bool fromPush) {
			ActivatePanel(PanelForKey(_activeKey), PanelForKey(key), fromPush);
		}

		private void Update() {
			if (_disabled) return;

            bool isAtTop = MenuStack.Shared.IsMenuAtTop(this);
            bool isInStack = MenuStack.Shared.IsMenuInStack(this);

			// Toggling can happen if the menu is not up at all, or it's on top.
            if (!isInStack || isAtTop) {
                if (InputMediator.MenuToggleDown()) {
                    ToggleMenu();
                    return;
                }
            }

			if (isAtTop) {
                if (InputMediator.UICancelDown()) {
                    if (ShouldIgnoreCancelInput()) {
                        return;
                    }

                    if (!IsAtRoot()) {
                        PopPanel();
                    }
                }

				HandleSelectionRestoration();
            }
		}

		private void HandleSelectionRestoration() {
            if (EventSystem.current.currentSelectedGameObject != null) {
                _lastValidSelection = EventSystem.current.currentSelectedGameObject;
            } else {
                // Null selected object.
                switch (MenuConfig.SelectionRestorationMode) {
                    case MenuConfig.RestorationMode.Never:
                        break;
                    case MenuConfig.RestorationMode.Always:
                        // Try to restore the last input.
                        if (_lastValidSelection != null && _lastValidSelection.activeInHierarchy) {
                            EventSystem.current.SetSelectedGameObject(_lastValidSelection);
                        }
                        // If that isn't available, return to the default.
                        else if (_activeDefaultInput != null && _activeDefaultInput.activeInHierarchy) {
                            EventSystem.current.SetSelectedGameObject(_activeDefaultInput);
                        }
                        break;
                    case MenuConfig.RestorationMode.OnInput:
                        if (_activeDefaultInput != null && _activeDefaultInput.activeInHierarchy
                                && (Mathf.Abs(InputMediator.UIX()) > 0.1 || Mathf.Abs(InputMediator.UIY()) > 0.1)) {
                            EventSystem.current.SetSelectedGameObject(_activeDefaultInput);
                        }
                        break;
                }
            }
        }

        private bool ShouldIgnoreCancelInput() {
            if (_activeManager == null || _activeManager.ElementManagers == null) {
                return false;
            }

            for (int i = 0; i < _activeManager.ElementManagers.Length; i++) {
                var el = _activeManager.ElementManagers[i];
                if (el != null && el.ConsumesCancelInput) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Goes to a panel, bypassing the stack. Used by push and pop after
        /// modifying the stack. Only use if you know what you're doing.
        /// </summary>
        /// <param name="key">Key of the panel to go to.</param>
        /// <param name="fromPush">Whether the panel is from a push or a pop. Used for animations.</param>
        protected void GoToPanel(string key, bool fromPush) {
			ActivatePanel(key, fromPush);
		}

		public void PushPanel(string key) {
			foreach (PanelManager panel in Panels) {
				if (panel.Key == key) {
					_selectedStack.Push(EventSystem.current.currentSelectedGameObject);
					_panelStack.Push(key);
					GoToPanel(key, true);
					return;
				}
			}
			Debug.LogErrorFormat("Cannot push panel {0}! Not in Panels array.", key);
		}

		private bool IsAtRoot() {
			return _panelStack.Count == 0;
		}

		public void PopPanel() {
			if (_panelStack.Count > 0) {
				_panelStack.Pop();
			}
			if (_panelStack.Count == 0) {
				GoToPanel(MenuConfig.MainPanelKey, false);
			} else {
				GoToPanel(_panelStack.Last(), false);
			}
		}

		/// <summary>
		/// Pops the current panel if not at root, otherwise hides the menu.
		/// </summary>
		public void PopPanelOrExitMenu() {
			if (IsAtRoot()) {
				ExitMenu();
            } else {
				PopPanel();
            }
        }

		public void ShowMenu() {
			if (_active) return;
			ToggleMenu();
        }

		public void ExitMenu() {
			if (!_active) return;
			ToggleMenu();
		}

		public void ExitGame() {
			Application.Quit();
		}
	}
}