using LLMUnitySamples;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.XR;

public class VoiceInput : MonoBehaviour{

    [SerializeField] private TMP_InputField userText;
    [SerializeField] private TMP_Text aiText;
    [SerializeField] private FunctionCalling LLMFunctionCaller;

    private StringBuilder partialText = new StringBuilder();
    private bool isListening = false;
    private DictationRecognizer dictation;

    private InputDevice leftController;


    void Start(){
        // setup dictation
        dictation = new DictationRecognizer(ConfidenceLevel.Medium);
        //dictation.DictationHypothesis += OnHypothesis;
        dictation.DictationResult += OnResult;
        dictation.DictationComplete += OnComplete;
        dictation.DictationError += OnError;


    }

    void Update() {
        if (!leftController.isValid) {
            // Get the left controller 
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
            if (devices.Count > 0)
                leftController = devices[0];
            return;
        }

        // X button = CommonUsages.primaryButton
        leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool xPressed);

        if (xPressed && !isListening)
            StartListening();

        if (!xPressed && isListening)
            StopListening();
    }

    private void StartListening() {
        Debug.Log("START LISTENING (X pressed)");
        partialText.Clear();
        isListening = true;
        dictation.Start();

        Debug.Log("DictationRecognizer.Status = " + dictation.Status);

    }

    private void StopListening() {
        Debug.Log("STOP LISTENING (X released)");
        isListening = false;
        dictation.Stop();
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
