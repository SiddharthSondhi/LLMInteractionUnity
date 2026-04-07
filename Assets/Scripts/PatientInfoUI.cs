using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatientInfoUI : MonoBehaviour {
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Transform patientButtonsParent;
    [SerializeField] private GameObject patientRowPrefab;


    public static PatientInfoUI Instance;

    private PatientTab currentTab = PatientTab.General;


    private void Awake() {
        Instance = this;
        PopulateList();
        RefreshUI();
    }

    public string SelectPatient(int id) {
        PatientInfo p = PatientManager.GetPatient(id);

        if (p == null) {
            PatientManager.currentPatient = null;
            return $"No patient found with given id : {id}.";
        }
        
        PatientModelManager.Instance.ShowPatient(id);

        PatientManager.currentPatient = p;
        currentTab = PatientTab.General;
        RefreshUI();
        
        return p.GetBasicPatientInfo();
    }

    private void RefreshUI() {
        if (PatientManager.currentPatient == null) {
            infoText.text = "No patient selected.";
            return;
        }

        infoText.text = PatientManager.currentPatient.GetTabInfo(currentTab);
    }

    public void SelectTab(PatientTab tab) {
        if (PatientManager.currentPatient == null)
            return;

        currentTab = tab;
        RefreshUI();
    }

    // add patients into scrollable list in UI
    public void PopulateList() {
        foreach (Transform child in patientButtonsParent)
            Destroy(child.gameObject);

        foreach (var patient in PatientManager.getPatients()) {
            GameObject row = Instantiate(patientRowPrefab, patientButtonsParent);

            row.GetComponentInChildren<TMP_Text>().text = patient.id.ToString();

            row.GetComponent<Button>().onClick.AddListener(() => {
                SelectPatient(patient.id);
            });

            row.GetComponent<Button>().onClick.AddListener(() => {
                InteractionsManager.Instance.IncrementTimesInteractedWithUI();
            });
        }
    }

    public void SelectGeneral() {
        SelectTab(PatientTab.General);
    }

    public void SelectDiagnosis() {
        SelectTab(PatientTab.Diagnosis);
    }

    public void SelectMedicalHistory() {
        SelectTab(PatientTab.MedicalHistory);
    }

    public void SelectImaging() {
        SelectTab(PatientTab.Imaging);
    }

    public void SelectTreatmentPlan() {
        SelectTab(PatientTab.TreatmentPlan);
    }

    public void SelectFollowUp() {
        SelectTab(PatientTab.FollowUp);
    }



}
