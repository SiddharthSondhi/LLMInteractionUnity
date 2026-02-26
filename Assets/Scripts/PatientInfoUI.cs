using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatientInfoUI : MonoBehaviour {
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject patientRowPrefab;
    public static PatientInfoUI Instance;


    private void Awake() {
        Instance = this;
        PopulateList();
    }

    public string DisplayPatient(int id) {
        PatientInfo p = PatientManager.GetPatient(id);
        string text;

        if (p == null) {
            text = $"No patient found with given id : {id}.";
        }
        else {
            text = p.getPatientInfo();
            PatientModelManager.Instance.ShowPatient(id);
        }

        infoText.text = text ;
        return text;
    }

    // add patients into scrollable list in UI
    public void PopulateList() {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var patient in PatientManager.getPatients()) {
            GameObject row = Instantiate(patientRowPrefab, contentParent);

            row.GetComponentInChildren<TMP_Text>().text = patient.id.ToString();

            row.GetComponent<Button>().onClick.AddListener(() => {
                DisplayPatient(patient.id);
            });
        }
    }



}
