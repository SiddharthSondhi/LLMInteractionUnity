using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LiverSegmentHighlighter : MonoBehaviour {
    public static LiverSegmentHighlighter Instance;

    [SerializeField] private Material normalMat;
    [SerializeField] private Material highlightMat;
    [SerializeField] private Material transparentMat;
    [SerializeField] private MeshRenderer wholeLiver;

    private MeshRenderer[] segments;

    void Awake() {
        Instance = this;

        segments = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Where(r => r.gameObject.CompareTag("LiverSegment"))
            .OrderBy(r => r.gameObject.name).ToArray();

        // Initialize all to normal
        for (int i = 0; i < segments.Length; i++)
            segments[i].sharedMaterial = normalMat;

        Debug.Log($"Num Segments Found: {segments.Length}");
    }

    public void HighlightSegment(int index) {
        if (index > 8) return;

        //  highlight the selected segment and make everything else transperent
        for (int i = 0; i < segments.Length; i++) {
            segments[i].sharedMaterial = (i == index) ? highlightMat : transparentMat;
        }

        // disable wholeLiver model
        if (wholeLiver != null) {
            wholeLiver.enabled = false;
        }
        else {
            Debug.LogError("wholeLiver not assigned!");
        }
    }

    public void ClearHighlight() {
        for (int i = 0; i < segments.Length; i++) {
            segments[i].sharedMaterial = normalMat;
        }
        wholeLiver.enabled = true;
    }
}