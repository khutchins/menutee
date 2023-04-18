using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Menutee {
	public class MenuHook : MonoBehaviour, IMenu {

		public enum SelectedBehavior {
			None,
			Restore,
			SetDefault
		}

		[Header("General")]
		[Tooltip("The canvas element. If this is set, this script will automatically hide/show the canvas when appropriate.")]
		public Canvas Canvas;
		[Tooltip("Whether or not this canvas should be shown by default.")]
		public bool ShowOnStart = false;
		[Tooltip("If this is true, it will hide the canvas except when it is not on top.")]
		public bool HideIfNotOnTop = false;

		[Header("Menu Attributes")]
		public CursorLockMode CursorLockMode = CursorLockMode.None;
		public bool CursorVisible = true;
		public bool PausesGame = true;
		[Tooltip("Time scale to use when in menu. If negative, it will use the existing time scale.")]
		public float TimeScale = 0;

		[Header("Selected Behavior")]
		[Tooltip("GameObject to default to when using the \"SetDefault\" behavior.")]
		public GameObject DefaultSelectedGameObject;
		[Tooltip("GameObject to use when the menu is pushed.")]
		public bool UseDefaultOnPush;
		[Tooltip("Whether or not the selected game object should be cleared when this menu is not on top or disappears.")]
		public bool ClearSelectedOnNotOnTop = false;
		[Tooltip("Behavior to use when the menu is popped back to.")]
		public SelectedBehavior BehaviorOnPop = SelectedBehavior.Restore;
		[Tooltip("Whether or not the default selected game object should be restored if no objects are selected, a direction is pressed, and this menu is on top.")]
		public bool RestoreDefaultOnInputIfNoneSelected = false;
		[Tooltip("Input mediator to use to detect input if restore default on input is enabled.")]
		public MenuInputMediator InputMediator;

		[Header("Hooks")]
		[Tooltip("If this is true, OnMenuClose callbacks will be called on startup if it doesn't start shown.")]
		public bool CallCloseCallbackOnStart = true;
		[Tooltip("If this is true, OnMenuNotTop callbacks will be called on startup if it doesn't start shown.")]
		public bool CallNotTopCallbackOnStart = true;
		[Tooltip("Callbacks for when the menu is shown.")]
		public UnityEvent OnMenuOpen;
		[Tooltip("Callbacks for when the menu is hidden.")]
		public UnityEvent OnMenuClose;
		[Tooltip("Callbacks for when the menu has become the top menu on the stack.")]
		public UnityEvent OnMenuTop;
		[Tooltip("Callbacks for when the menu is no longer the top menu on the stack.")]
		public UnityEvent OnMenuNotTop;

		private GameObject _cachedSelection;
		private bool _isOnTop = false;

		void Awake() {
			if (Canvas != null) Canvas.enabled = false;
		}

		void Start() {
			if (ShowOnStart) {
				PushMenu();
			} else {
				if (CallNotTopCallbackOnStart) OnMenuNotTop?.Invoke();
				if (CallCloseCallbackOnStart) OnMenuClose?.Invoke();
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
			if (newUp) {
				OnMenuOpen?.Invoke();

				if (UseDefaultOnPush) {
					SetSelectedGameObjectFromBehavior(SelectedBehavior.SetDefault);
				}
			} else {
				OnMenuClose?.Invoke();
				_cachedSelection = null;
			}
		}

		public void SetMenuOnTop(bool newOnTop) {
			if (HideIfNotOnTop) Canvas.enabled = newOnTop;
			if (newOnTop) OnMenuTop?.Invoke();
			else OnMenuNotTop?.Invoke();

			_isOnTop = newOnTop;
			if (!newOnTop) {
				_cachedSelection = EventSystem.current.currentSelectedGameObject;
				if (ClearSelectedOnNotOnTop) {
					EventSystem.current.SetSelectedGameObject(null);
				}
			} else {
				// If cached selection is not null, it means that this is from a pop, not a push.
				if (_cachedSelection != null) {
					SetSelectedGameObjectFromBehavior(BehaviorOnPop);
				}
			}
		}

		private void SetSelectedGameObjectFromBehavior(SelectedBehavior behavior) {
			switch (behavior) {
				case SelectedBehavior.Restore:
					EventSystem.current.SetSelectedGameObject(_cachedSelection);
					break;
				case SelectedBehavior.SetDefault:
					EventSystem.current.SetSelectedGameObject(DefaultSelectedGameObject);
					break;
				case SelectedBehavior.None:
				default:
					break;
			}
		}

		public void PushMenu() {
			MenuStack.Shared.PushAndShowMenu(this);
		}

		public void PushOtherMenu(MenuHook other) {
			MenuStack.Shared.PushAndShowMenu(other);
		}

		public void PushOtherMenu(MenuManager other) {
			MenuStack.Shared.PushAndShowMenu(other);
		}

		public void PopMenu() {
			MenuStack.Shared.PopAndCloseMenu(this);
		}

		void Update() {
			if (_isOnTop && RestoreDefaultOnInputIfNoneSelected 
				&& EventSystem.current.currentSelectedGameObject == null 
				&& (Mathf.Abs(InputMediator.UIX()) > 0.1 || Mathf.Abs(InputMediator.UIY()) > 0.1)) {
				EventSystem.current.SetSelectedGameObject(DefaultSelectedGameObject);
			}
		}
	}
}