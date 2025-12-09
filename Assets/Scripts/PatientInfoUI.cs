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
            text = $"Found patient id {p.id}.\n" +
                    $"Sex: {p.sex}\n" +
                    $"Year of Birth: {p.yearOfBirth}\n" +
                    $"Liver Density: {p.liverDensity}\n" +
                    $"Pathologies: {p.pathologies}";
        }

        infoText.text = text ;
        return text;
    }

}
