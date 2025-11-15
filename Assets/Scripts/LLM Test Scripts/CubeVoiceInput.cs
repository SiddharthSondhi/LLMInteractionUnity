using System.Text;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceInput : MonoBehaviour{

    [SerializeField] private TMP_InputField userText;
    [SerializeField] private TMP_Text aiText;
    [SerializeField] private CubeFunctionCalling LLMFunctionCaller;

    private StringBuilder partialText = new StringBuilder();
    private bool isListening = false;
    private DictationRecognizer dictation;



    void Start(){
        dictation = new DictationRecognizer(ConfidenceLevel.Medium);
        //dictation.DictationHypothesis += OnHypothesis;
        dictation.DictationResult += OnResult;
        dictation.DictationComplete += OnComplete;
        dictation.DictationError += OnError;
    }

    public void ButtonListening() {
        Debug.Log("BUTTON LISTENING: " + isListening);
        if (isListening) {
            dictation.Stop();
        }
        else {
            partialText.Clear();
            dictation.Start();
            isListening = true;
        }
    }

    private void OnHypothesis(string text) {
        userText.text = text;
    }

    private void OnResult(string text, ConfidenceLevel confidence) {
        partialText.Append(text).Append(" ");
        userText.text = partialText.ToString();
    }

    private void OnComplete(DictationCompletionCause cause) {
        isListening = false;
        //LLMFunctionCaller.SendMessage("OnInputFieldSubmit", userText.text);
    }

    private void OnError (string error, int hresult) {
        isListening = false;
        Debug.LogError($"ERROR in voice recognition: {error} (0x{hresult:X8})");
    }

    private void OnDestroy() {
        if (dictation != null) {
            //dictation.DictationHypothesis -= OnHypothesis;
            dictation.DictationResult -= OnResult;
            dictation.DictationComplete -= OnComplete;
            dictation.DictationError -= OnError;
            dictation.Dispose();
        }
    }

}
