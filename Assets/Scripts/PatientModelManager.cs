using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatientModelManager : MonoBehaviour {
    public static PatientModelManager Instance;

    private GameObject[] patientModels;

    private void Awake() {
        Instance = this;
        patientModels = transform.Cast<Transform>().Select(obj => obj.gameObject).ToArray();
        HideAllPatients();
    }

    private void HideAllPatients() {
        foreach (var model in patientModels) {
            model.SetActive(false);
        }
    }

    public void ShowPatient(int id) {
        int index = id - 1;

        if (index < 0 || index >= patientModels.Length) {
            Debug.LogError($"ShowPatient: invalid id {id}");
            return;
        }

        HideAllPatients();
        patientModels[index].SetActive(true);
        patientModels[index].transform.position = new Vector3(2.565f, 1.215f, -1.945f);
        patientModels[index].transform.rotation = Quaternion.Euler(-90, 180, 0);
    }
}