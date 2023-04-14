using System.Collections;
using System.Collections.Generic;
using Menutee;
using UnityEngine;

public class CloseMenuAfterDelay : MonoBehaviour {
    [SerializeField] private MenuHook Menu;

    private void OnEnable() {
        Menu.OnMenuOpen.AddListener(Opened);
    }

    private void OnDisable() {
        Menu.OnMenuOpen.RemoveListener(Opened);
    }

    void Opened() {
        StopAllCoroutines();
        StartCoroutine(Delay());
    }

    IEnumerator Delay() {
        yield return new WaitForSecondsRealtime(3f);
        Menu.PopMenu();
    }
}
