using LLMUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBot : MonoBehaviour{

    [SerializeField]
    private TMP_InputField userText;
    
    [SerializeField] 
    private TMP_Text aiText;      
    
    [SerializeField] 
    private ScrollRect scrollRect;

    [SerializeField]
    private LLMCharacter llmCharacter;

    [SerializeField]
    private Button stopButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        userText.onSubmit.AddListener(handleSubmit);
        stopButton.onClick.AddListener(onButtonClick);
    }

    private void handleSubmit(string text) {

        if (!string.IsNullOrWhiteSpace(text)) {

            userText.interactable = false;
            _ = llmCharacter.Chat(text, AIHandleReply, AIReplyComplete);


            userText.text = "";
        }
        
    }

    private void AIHandleReply(string text) {
        aiText.text = text;

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void AIReplyComplete() {
        userText.interactable = true;
        userText.ActivateInputField();

        Debug.Log("AI reply complete");
    }

    private void onButtonClick() {
        userText.interactable = true;
        userText.ActivateInputField();

        llmCharacter.CancelRequests();
    }

}
