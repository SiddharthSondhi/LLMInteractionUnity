using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;
using Whisper;
using Whisper.Utils;
using UnityEngine.UI;

public class WhisperVoiceInput : MonoBehaviour {
    [SerializeField] private WhisperManager whisper;
    [SerializeField] private MicrophoneRecord microphoneRecord;
    [SerializeField] private TMP_InputField userText;
    [SerializeField] private Image MicIndicator;

    private string recognizedText = "";

    private InputDevice leftController;
    private bool isListening = false;

    void Start() {
        microphoneRecord.vadStop = false; 
        microphoneRecord.OnRecordStop += OnRecordStop;

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

    private void StartListening() {
        if (microphoneRecord.IsRecording)
            return;

        isListening = true;
        recognizedText = "";
        microphoneRecord.StartRecord();

        MicIndicator.enabled = true;

        //Debug.Log("Started recording");
    }

    private void StopListening() {
        if (!microphoneRecord.IsRecording)
            return;

        isListening = false;
        microphoneRecord.StopRecord();

        MicIndicator.enabled = false;

        //Debug.Log("Stopped recording");
    }

    private async void OnRecordStop(AudioChunk recordedAudio) {
        if (whisper == null)
            return;

        var result = await whisper.GetTextAsync(
            recordedAudio.Data,
            recordedAudio.Frequency,
            recordedAudio.Channels
        );

        if (result == null) {
            Debug.LogWarning("Whisper returned no result");
            return;
        }

        recognizedText = result.Result?.Trim() ?? "";
        userText.text += " " + recognizedText;
        //Debug.Log("Recognized text: " + recognizedText);
    }

    private void OnDestroy() {
        if (microphoneRecord != null)
            microphoneRecord.OnRecordStop -= OnRecordStop;
    }
}