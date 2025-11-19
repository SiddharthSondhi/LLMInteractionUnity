using LLMUnitySamples;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.XR;
using UnityEngine.UI;

public class VoiceInput : MonoBehaviour{

    [SerializeField] private TMP_InputField userText;
    [SerializeField] private TMP_Text aiText;
    [SerializeField] private Image MicIndicator;
    [SerializeField] private SofaFunctionCalling LLMFunctionCaller;

    private bool isListening = false;
    private DictationRecognizer dictation;
    private string hypothesisText = "";
    private string textBeforeListening = "";


    private InputDevice leftController;


    void Start(){
        // setup dictation
        dictation = new DictationRecognizer(ConfidenceLevel.Medium);
        dictation.DictationHypothesis += OnHypothesis;
        dictation.DictationResult += OnResult;
        dictation.DictationComplete += OnComplete;
        dictation.DictationError += OnError;

        /*
        foreach (var device in Microphone.devices)
            Debug.Log("Mic device: " + device);
        */
        
        MicIndicator.enabled = false;
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

    public void OnSendButton() {
        string message = userText.text.Trim();

        if (string.IsNullOrEmpty(message))
            return;

        LLMFunctionCaller.SendMessage("OnInputFieldSubmit", message);
        userText.text = "";
    }

    private void StartListening() {
        //Debug.Log("START LISTENING (X pressed)");

        MicIndicator.enabled = true;
        isListening = true;
        textBeforeListening = userText.text;
        dictation.Start();

        //Debug.Log("DictationRecognizer.Status = " + dictation.Status);
    }

    private void StopListening() {
        //Debug.Log("STOP LISTENING (X released)");
        MicIndicator.enabled = false;
        isListening = false;
        dictation.Stop();
        hypothesisText = "";
    }


    private void OnHypothesis(string text) {
        hypothesisText = text;
        userText.text = textBeforeListening + hypothesisText;
    }

    private void OnResult(string text, ConfidenceLevel confidence) {
        textBeforeListening += text + " ";
        userText.text = textBeforeListening;
        hypothesisText = "";
    }

    private void OnComplete(DictationCompletionCause cause) {
        isListening = false;
    }

    private void OnError (string error, int hresult) {
        isListening = false;
        Debug.LogError($"ERROR in voice recognition: {error} (0x{hresult:X8})");
    }

    private void OnDestroy() {
        if (dictation != null) {
            dictation.DictationHypothesis -= OnHypothesis;
            dictation.DictationResult -= OnResult;
            dictation.DictationComplete -= OnComplete;
            dictation.DictationError -= OnError;
            dictation.Dispose();
        }
    }

}
