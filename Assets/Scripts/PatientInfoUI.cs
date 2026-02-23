using TMPro;
using UnityEngine;

public class PatientInfoUI : MonoBehaviour {
    [SerializeField] private TMP_Text infoText;
    public static PatientInfoUI Instance;

    private void Awake() {
        Instance = this;
    }

    public string DisplayPatient(int id) {
        PatientInfo p = PatientManager.GetPatient(id);
        string text;

        if (p == null) {
            text = $"No patient found with given id : {id}.";
        }
        else {
            text = p.getPatientInfo();
        }

        infoText.text = text ;
        return text;
    }

}
