using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Adds whichever input system is set up for the project.
/// </summary>
[RequireComponent(typeof(EventSystem))]
public class CrossInputEventSystemSetup : MonoBehaviour {
    private void Awake() {
#if ENABLE_INPUT_SYSTEM
        if (GetComponent<InputSystemUIInputModule>() == null) {
            gameObject.AddComponent<InputSystemUIInputModule>();
        }
#else
        if (GetComponent<StandaloneInputModule>() == null) {
            gameObject.AddComponent<StandaloneInputModule>();
        }
#endif
    }
}
