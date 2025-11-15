using SofaUnity;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

public class SceneController : MonoBehaviour {
    [SerializeField] private SofaContext sofaContext;  
    private SofaDAGNode liverNode;
    private bool IsActive = false;


    void Start() {
        // find the DAG node named "Liver"
        foreach (var node in sofaContext.GetComponentsInChildren<SofaDAGNode>()) {
            if (node.DisplayName == "Liver") {
                liverNode = node;
                break;
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) {
            IsActive = !IsActive;
            //Debug.Log("PRESSED  " + IsActive);
        }

        if (IsActive) {
            // Move liver 
            if (Input.GetKey(KeyCode.A))
                liverNode.Translation += new Vector3(-0.01f, 0, 0);
            if (Input.GetKey(KeyCode.D))
                liverNode.Translation += new Vector3(0.01f, 0, 0);
            if (Input.GetKey(KeyCode.W))
                liverNode.Translation += new Vector3(0, 0.01f, 0);
            if (Input.GetKey(KeyCode.S))
                liverNode.Translation += new Vector3(0, -0.01f, 0);

            // Rotate liver 
            if (Input.GetKey(KeyCode.Q))
                liverNode.Rotation += new Vector3(0, -1, 0);
            if (Input.GetKey(KeyCode.E))
                liverNode.Rotation += new Vector3(0, 1, 0);

            // Scale liver 
            if (Input.GetKeyDown(KeyCode.Z))
                liverNode.Scale *= 0.99999f;
            if (Input.GetKeyDown(KeyCode.X))
                liverNode.Scale *= 1.00001f;

            // Reset 
            if (Input.GetKeyDown(KeyCode.R)) {
                liverNode.Translation = Vector3.zero;
                liverNode.Rotation = Vector3.zero;
                liverNode.Scale = Vector3.one;
            }

            for (int i = 0; i < 9; i++) {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i)) {
                    LiverSegmentHighlighter.Instance.HighlightSegment(i);
                }
            }


        }
    }
}