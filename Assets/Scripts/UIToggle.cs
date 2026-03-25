using System.Collections.Generic;
using UnityEngine;

public class UIToggleGroups : MonoBehaviour {
    [Header("Enable / Disable Groups")]
    [SerializeField] private bool enableAI = true;
    [SerializeField] private bool enableUI = true;

    [Header("UI Objects AI")]
    [SerializeField] private List<GameObject> groupAIObjects = new List<GameObject>();

    [Header("UI Objects UI")]
    [SerializeField] private List<GameObject> groupUIObjects = new List<GameObject>();


    private void Start() {
        ApplyVisibility();
    }

    private void OnValidate() {
        ApplyVisibility();
    }

    private void ApplyVisibility() {
        SetGroupActive(groupAIObjects, enableAI);
        SetGroupActive(groupUIObjects, enableUI);
    }


    private void SetGroupActive(List<GameObject> objects, bool isActive) {
        if (objects == null) return;

        foreach (GameObject obj in objects) {
            if (obj != null)
                obj.SetActive(isActive);
        }
    }

    public void SetA(bool value) {
        enableAI = value;
        SetGroupActive(groupAIObjects, enableAI);
    }

    public void SetB(bool value) {
        enableUI = value;
        SetGroupActive(groupUIObjects, enableUI);
    }
}