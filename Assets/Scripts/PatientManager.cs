using UnityEngine;
using System.Collections.Generic;

public enum PatientTab {
    General,
    Diagnosis,
    MedicalHistory,
    Imaging,
    TreatmentPlan,
    FollowUp
}

[System.Serializable]
public class GeneralTab {
    public string displayName;
    public int heightCm;
    public int weightKg;
    public string smokingStatus;
    public string alcoholUse;
    public List<string> allergies;
    public List<string> currentMeds;

    public string GetInfo() {
        string allergiesText = (allergies != null && allergies.Count > 0) ? string.Join(", ", allergies) : "None";
        string medsText = (currentMeds != null && currentMeds.Count > 0) ? string.Join(", ", currentMeds) : "None";

        return
            $"Display Name: {displayName}\n" +
            $"Height: {heightCm} cm\n" +
            $"Weight: {weightKg} kg\n" +
            $"Smoking Status: {smokingStatus}\n" +
            $"Alcohol Use: {alcoholUse}\n" +
            $"Allergies: {allergiesText}\n" +
            $"Current Medications: {medsText}";
    }
}

[System.Serializable]
public class DiagnosisTab {
    public string workingDiagnosis;
    public List<string> differential;
    public string tumourBurden;

    public string GetInfo() {
        string differentialText = (differential != null && differential.Count > 0) ? string.Join(", ", differential) : "None";

        return
            $"Working Diagnosis: {workingDiagnosis}\n" +
            $"Differential: {differentialText}\n" +
            $"Tumour Burden: {tumourBurden}";
    }
}

[System.Serializable]
public class MedicalHistoryTab {
    public List<string> pastMedicalHistory;
    public List<string> pastSurgicalHistory;
    public List<string> familyHistory;
    public string socialHistory;

    public string GetInfo() {
        string pmh = (pastMedicalHistory != null && pastMedicalHistory.Count > 0) ? string.Join(", ", pastMedicalHistory) : "None";
        string psh = (pastSurgicalHistory != null && pastSurgicalHistory.Count > 0) ? string.Join(", ", pastSurgicalHistory) : "None";
        string fh = (familyHistory != null && familyHistory.Count > 0) ? string.Join(", ", familyHistory) : "None";

        return
            $"Past Medical History: {pmh}\n" +
            $"Past Surgical History: {psh}\n" +
            $"Family History: {fh}\n" +
            $"Social History: {socialHistory}";
    }
}

[System.Serializable]
public class ImagingTab {
    public string ctSummary;
    public string mriSummary;
    public string radiologyImpression;

    public string GetInfo() {
        return
            $"CT Summary: {ctSummary}\n" +
            $"MRI Summary: {mriSummary}\n" +
            $"Radiology Impression: {radiologyImpression}";
    }
}

[System.Serializable]
public class TreatmentPlanTab {
    public string recommendation;
    public List<string> plannedProcedures;
    public string notes;

    public string GetInfo() {
        string procedures = (plannedProcedures != null && plannedProcedures.Count > 0) ? string.Join(", ", plannedProcedures) : "None";

        return
            $"Recommendation: {recommendation}\n" +
            $"Planned Procedures: {procedures}\n" +
            $"Notes: {notes}";
    }
}

[System.Serializable]
public class FollowUpTab {
    public string nextVisit;
    public List<string> pending;

    public string GetInfo() {
        string pendingText = (pending != null && pending.Count > 0) ? string.Join(", ", pending) : "None";

        return
            $"Next Visit: {nextVisit}\n" +
            $"Pending: {pendingText}";
    }
}

[System.Serializable]
public class PatientTabs {
    public GeneralTab general;
    public DiagnosisTab diagnosis;
    public MedicalHistoryTab medicalHistory;
    public ImagingTab imaging;
    public TreatmentPlanTab treatmentPlan;
    public FollowUpTab followUp;
}

[System.Serializable]
public class PatientInfo {
    public int id;
    public string sex;
    public int yearOfBirth;
    public int liverDensity;
    public string pathologies;
    public PatientTabs tabs;


    public string GetBasicPatientInfo() {
        return  $"Patient id {id}.\n" +
                $"Sex: {sex}\n" +
                $"Year of Birth: {yearOfBirth}\n" +
                $"Liver Density: {liverDensity}\n" +
                $"Pathologies: {pathologies}";
    }

    public string GetFullPatientInfo()
    {
        return
            GetBasicPatientInfo() + "\n\n" +
            "General:\n" + tabs.general.GetInfo() + "\n\n" +
            "Diagnosis:\n" + tabs.diagnosis.GetInfo() + "\n\n" +
            "Medical History:\n" + tabs.medicalHistory.GetInfo() + "\n\n" +
            "Imaging:\n" + tabs.imaging.GetInfo() + "\n\n" +
            "Treatment Plan:\n" + tabs.treatmentPlan.GetInfo() + "\n\n" +
            "Follow Up:\n" + tabs.followUp.GetInfo();
    }

    public string GetTabInfo(PatientTab tab) {
        switch (tab) {
            case PatientTab.General:
                return GetBasicPatientInfo() + "\n\n" + tabs.general.GetInfo();

            case PatientTab.Diagnosis:
                return tabs.diagnosis.GetInfo();

            case PatientTab.MedicalHistory:
                return tabs.medicalHistory.GetInfo();

            case PatientTab.Imaging:
                return tabs.imaging.GetInfo();

            case PatientTab.TreatmentPlan:
                return tabs.treatmentPlan.GetInfo();

            case PatientTab.FollowUp:
                return tabs.followUp.GetInfo();

            default:
                return "";
        }
    }
}

[System.Serializable]
public class PatientDatabase {
    public List<PatientInfo> patients;
}


public static class PatientManager{
    private static PatientDatabase patients;
    public static PatientInfo currentPatient;



    static PatientManager(){
        TextAsset jsonFile = Resources.Load<TextAsset>("patients_detailed");
        if (jsonFile == null) {
            Debug.LogError("Could not find patients_detailed.json");
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

    public static List<PatientInfo> getPatients() {
        return patients.patients;
    }

    public static string GetAllPatientInfoString() {
        string info = "";
        foreach (var p in patients.patients) {
            info += p.GetBasicPatientInfo() + "\n\n";
        }

        return info;
    }

    public static string GetCurrentPatientInfo() { 
        if (currentPatient == null) 
            return "No patient currently selected.";

        return currentPatient.GetFullPatientInfo();
    }
}