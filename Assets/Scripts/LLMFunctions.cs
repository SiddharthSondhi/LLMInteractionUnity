using UnityEngine;
using SofaUnity;


public static class LLMFunctions {
    public static SofaContext sofaContext;
    public static SofaDAGNode liverNode;
    public static string functionDescriptions =
        @"
        - TranslateLiver
          description - Translates the liver to the position x, y, z
          arguments - x : float, y : float, z : float

        - RotateLiver        
          description - Rotates the liver given number of degrees around the x, y, or z axis.
          arguments - degrees : float, axis: string with options 'x'|'y'|'z'

        - SetGravity
          description - Sets the x, y, and z values for the force of gravity in the simulation 
          arguments - x : float, y : float, z : float

        - ResetSim
          description - Resets the simulation to its original state.

        - LoadPatientInfo
          description - Loads and displays patient information based on given id. Additionally loads in a model of the patient's liver.
          arguments - id : id

        - HighlightSegment
          description - Highlights one of the 9 liver segments.
          arguments - index : integer in range(1 - 9)
        ";

    public static string TranslateLiver(float x, float y, float z) {
        liverNode.Translation = new Vector3(x, y, z);
        return $"Translated Liver to {x}, {y}, {z}";
    }

    public static string RotateLiver(float degrees, string axis) {
        Vector3 axisVec = Vector3.zero;
        switch (axis) {
            case "x": axisVec = new Vector3(degrees, 0, 0); break;
            case "y": axisVec = new Vector3(0, degrees, 0); break;
            case "z": axisVec = new Vector3(0, 0, degrees); break;
        };

        liverNode.Rotation += axisVec;

        return $"Rotated liver {degrees} degrees around the {axis} axis";
    }

    public static string SetGravity(float gravX, float gravY, float gravZ) { 
        sofaContext.Gravity = new Vector3 (gravX, gravY, gravZ);
        return $"Set gravity to x: {gravX}, y: {gravY} z: {gravZ}";
    }

    public static string ResetSim() {
        liverNode.Translation = Vector3.zero;
        liverNode.Rotation = Vector3.zero;
        liverNode.Scale = Vector3.one;

        sofaContext.Gravity = new Vector3(0.0f, -9.8f, 0.0f);

        return "Reset the simulation";
    }

    public static string LoadPatientInfo(int id) {
        string response = PatientInfoUI.Instance.DisplayPatient(id);
        PatientModelManager.Instance.ShowPatient(id);

        return response;
    }

    public static string HighlightSegment(int segment) {
        LiverSegmentHighlighter.Instance.HighlightSegment(segment - 1);

        return $"Highlighted segment {segment}.";
    }
}