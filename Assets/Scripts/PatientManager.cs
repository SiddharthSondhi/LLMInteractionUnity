using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;


[System.Serializable]
public class PatientInfo {
    public string name;
    public int age;
    public string gender;
    public string diagnosis;
    public string procedure;
    public float liverMass; 
}

[System.Serializable]
public class PatientDatabase {
    public List<PatientInfo> patients;
}

public static class PatientManager{
    private static PatientDatabase patients;

    static PatientManager(){
        TextAsset jsonFile = Resources.Load<TextAsset>("patients");
        if (jsonFile == null) {
            Debug.LogError("Could not find patients.json");
            return;
        }

        patients = JsonUtility.FromJson<PatientDatabase>(jsonFile.text);
    }

    public static PatientInfo GetPatient(string name) {
        foreach (var p in patients.patients) {
            if (p.name.ToLower().Trim() == name.ToLower().Trim()) {
                return p;
            }
        }
        return null;
    }
}