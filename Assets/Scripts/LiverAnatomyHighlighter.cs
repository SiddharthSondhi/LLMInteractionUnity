using System.Collections.Generic;
using UnityEngine;

public enum Transparency {
    Clear,
    Transparent,
    Opaque
}

public class LiverAnatomyHighlighter : MonoBehaviour {
    public static LiverAnatomyHighlighter Instance;

    [SerializeField] private Material normalMat;
    [SerializeField] private Material transparentMat;
    [SerializeField] private Material highlightMat;
    [SerializeField] private Material clearMat;

    [SerializeField] MeshRenderer commonHepaticPortal;
    [SerializeField] MeshRenderer gallBladder;
    [SerializeField] MeshRenderer hepaticPortalVein;
    [SerializeField] MeshRenderer inferiorVenaCava;
    [SerializeField] MeshRenderer leftLobe;
    [SerializeField] MeshRenderer rightLobe;
    [SerializeField] MeshRenderer ligaments;

    List<MeshRenderer> parts = new List<MeshRenderer>();

    private int currentHighlightedIndex = -1;
    private Transparency currentTransparency = Transparency.Opaque;


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

        ClearHighlight();
        parts[index].sharedMaterial = highlightMat;
        currentHighlightedIndex = index;
    }

    public void ClearHighlight() {
        foreach (var p in parts) {
            if (p.sharedMaterial == highlightMat)
                p.sharedMaterial = normalMat;
        }
        currentHighlightedIndex = -1;
        SetTransparency(currentTransparency);
    }

    //make left and right lobe transparent (unless already highlighted)
    public void SetTransparency(Transparency t) {
        Material mat = normalMat;
        currentTransparency = t;

        switch (t) {
            case Transparency.Clear:
                mat = clearMat;
                break;
            case Transparency.Transparent:
                mat = transparentMat;
                break;
            case Transparency.Opaque:
                mat = normalMat;
                break;
        }

        //left lobe
        if (currentHighlightedIndex != 4) {
            parts[4].sharedMaterial = mat;
        }
        
        //right Lobe 
        if (currentHighlightedIndex != 5) {
            parts[5].sharedMaterial = mat;
        }
    }

    public void SetClear() {
        SetTransparency(Transparency.Clear);
    }

    public void SetTransparent() {
        SetTransparency(Transparency.Transparent);
    }

    public void SetOpaque() {
        SetTransparency(Transparency.Opaque);
    }
}
