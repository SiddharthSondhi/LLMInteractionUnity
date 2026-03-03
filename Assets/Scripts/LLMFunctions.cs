using UnityEngine;
using SofaUnity;
using NUnit.Framework.Constraints;
using NUnit.Framework;


public static class LLMFunctions {
    public static SofaContext sofaContext;
    public static SofaDAGNode liverNode;
    public static string functionDescriptions =
        @"
        - TranslateLiver
          description - Translates the liver to the position x, y, z
          arguments - x : float, y : float, z : float

        - SetGravity
          description - Sets the x, y, and z values for the force of gravity in the simulation 
          arguments - x : float, y : float, z : float

        - ResetSim
          description - Resets the simulation to its original state.

        - LoadPatientInfo
          description - Loads and displays patient information based on given id. Additionally loads in a model of the patient's liver.
          arguments - id : id

        - HighlightSegment
          description - Highlights one of the 8 liver segments.
          arguments - index : integer in range(1 - 8)

        - HighlightLiverPart
          description - Highlights one of the following liver parts: Common Hepatic Portal, Gall Bladder, Hepatic Portal Vein, Inferior 
                        Vena Cava, Left Lobe, Right Lobe, and Ligaments.
          arguments - partName : string with options 'CommonHepaticPortal', 'GallBladder', 'HepaticPortalVein', 'InferiorVenaCava', 
                      'LeftLobe', 'RightLobe', 'Ligaments 
        ";

    public static string TranslateLiver(float x, float y, float z) {
        liverNode.Translation = new Vector3(x, y, z);
        return $"Translated Liver to {x}, {y}, {z}";
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

        return response;
    }

    public static string HighlightSegment(int segment) {
        LiverSegmentHighlighter.Instance.HighlightSegment(segment);

        return $"Highlighted segment {segment}.";
    }

    public static string HighlightLiverPart(string partName) {
        int index = 0;

        switch (partName) {
            case "CommonHepaticPortal": index = 0; break;
            case "GallBladder":         index = 1; break;
            case "HepaticPortalVein":   index = 2; break;
            case "InferiorVenaCava":    index = 3; break;
            case "LeftLobe":            index = 4; break;
            case "RightLobe":           index = 5; break;
            case "Ligaments":           index = 6; break;
            default:                    index = -1; break;
        }
        
        if (index >= 0) {
            LiverAnatomyHighlighter.Instance.HighlightPart(index);
            return $"Highlighted {partName}.";
        }

        LiverAnatomyHighlighter.Instance.ClearHighlight();
        return $"No part with name {partName} found!";
    }
}