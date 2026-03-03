using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatientModelManager : MonoBehaviour {
    public static PatientModelManager Instance;

    private GameObject[] patientModels;
    private Vector3[] startLocalPositions;
    private Quaternion[] startLocalRotations;

    private void Awake() {
        Instance = this;
        patientModels = transform.Cast<Transform>().Select(obj => obj.gameObject).ToArray();

        startLocalPositions = new Vector3[patientModels.Length];
        startLocalRotations = new Quaternion[patientModels.Length];

        for (int i = 0; i < patientModels.Length; i++) {
            startLocalPositions[i] = patientModels[i].transform.localPosition;
            startLocalRotations[i] = patientModels[i].transform.localRotation;
        }
    }

    
    public void ShowPatient(int id) {
        int index = id - 1;

        if (index < 0 || index >= patientModels.Length) {
            Debug.LogError($"ShowPatient: invalid id {id}");
            return;
        }

        ResetAllPatients();
        patientModels[index].SetActive(true);
        patientModels[index].transform.position = new Vector3(2.565f, 1.215f, -1.445f);
        patientModels[index].transform.rotation = Quaternion.Euler(-90, 180, 0);
    }

    public void ResetAllPatients() {
        for (int i = 0; i < patientModels.Length; i++) {
            Transform t = patientModels[i].transform;

            t.localPosition = startLocalPositions[i];
            t.localRotation = startLocalRotations[i];
        }
    }

    private void HideAllPatients() {
        foreach (var model in patientModels) {
            model.SetActive(false);
        }
    }

}