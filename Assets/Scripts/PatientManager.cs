using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;


[System.Serializable]
public class PatientInfo {
    public int id;
    public string sex;
    public int yearOfBirth;
    public int liverDensity;
    public string pathologies;
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

    public static PatientInfo GetPatient(int id) {
        foreach (var p in patients.patients) {
            if (p.id == id) {
                return p;
            }
        }
        return null;
    }
}