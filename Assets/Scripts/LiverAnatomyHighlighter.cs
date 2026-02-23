using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class LiverAnatomyHighlighter : MonoBehaviour {
    public static LiverAnatomyHighlighter Instance;

    [SerializeField] private Material normalMat;
    [SerializeField] private Material transparentMat;
    [SerializeField] private Material highlightMat;

    [SerializeField] MeshRenderer commonHepaticPortal;
    [SerializeField] MeshRenderer gallBladder;
    [SerializeField] MeshRenderer hepaticPortalVein;
    [SerializeField] MeshRenderer inferiorVenaCava;
    [SerializeField] MeshRenderer leftLobe;
    [SerializeField] MeshRenderer rightLobe;
    [SerializeField] MeshRenderer ligaments;

    List<MeshRenderer> parts = new List<MeshRenderer>();


    void Awake() {
        Instance = this;

        parts.Add(commonHepaticPortal);
        parts.Add(gallBladder);
        parts.Add(hepaticPortalVein);
        parts.Add(inferiorVenaCava);
        parts.Add(leftLobe);
        parts.Add(rightLobe);
        parts.Add(ligaments);

        // Initialize all to normal
        foreach( var p in parts) {
            p.sharedMaterial = normalMat;
        }           
    }

    public void HighlightPart(int index) {
        if (index > 6 || index < 0) return;

        //  highlight the selected part and make everything else transparent
        foreach (var p in parts) {
            p.sharedMaterial = transparentMat;
        }

        parts[index].sharedMaterial = highlightMat;
    }

    public void ClearHighlight() {
        foreach (var p in parts) {
            p.sharedMaterial = normalMat;
        }
    }
}
