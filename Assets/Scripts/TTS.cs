using UnityEngine;
using Piper;

public class TTS : MonoBehaviour {
    public static TTS Instance;

    [SerializeField] private PiperManager piper;
    [SerializeField] private AudioSource audioSource;

    private void Awake() {
        Instance = this;
    }

    public async void Speak(string text) {
        if (string.IsNullOrWhiteSpace(text)) return;

        var clip = await piper.TextToSpeech(text);

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}