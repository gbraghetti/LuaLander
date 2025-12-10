using CodeMonkey.CSharpCourse.Interactive;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatAIWindow : EditorWindow {


    [SerializeField] private VisualTreeAsset visualTreeAsset;

    [SerializeField] private VisualTreeAsset textTemplateVisualTreeAsset;
    [SerializeField] private VisualTreeAsset codeTemplateVisualTreeAsset;
    [SerializeField] private VisualTreeAsset videoTemplateVisualTreeAsset;


    private TextField inputTextField;
    private Label questionLabel;
    private bool waitingForChatAIResponse;
    private float waitingNextTimeToGetResponse;
    private int tryGetAnswerAttemptCount;


    [MenuItem("Code Monkey/Chat AI", priority = 100)]
    public static void ShowWindow() {
        ChatAIWindow window = GetWindow<ChatAIWindow>();
        window.titleContent = new GUIContent("Code Monkey Chat AI");
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement rootVisualTreeAsset = visualTreeAsset.Instantiate();
        rootVisualTreeAsset.style.flexGrow = 1f;
        root.Add(rootVisualTreeAsset);

        Button sendButton = rootVisualElement.Q<Button>("sendButton");
        sendButton.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
            SendText();
            //waitingForChatAIResponse = true;
        });

        inputTextField = rootVisualElement.Q<TextField>("inputTextField");
        inputTextField.RegisterCallback<KeyDownEvent>((KeyDownEvent keyDownEvent) => {
            if (keyDownEvent.keyCode == KeyCode.Return || keyDownEvent.keyCode == KeyCode.KeypadEnter) {
                SendText();
            }
        });

        questionLabel = rootVisualElement.Q<Label>("questionLabel");

        SetText("...");

        CodeMonkeyInteractiveSO.GetLastChatAIData(out string lastChatAIQuestionAsked, out string lastChatAIQuestionAnswer);
        if (!string.IsNullOrEmpty(lastChatAIQuestionAsked)) {
            questionLabel.text = lastChatAIQuestionAsked;
            SetText(lastChatAIQuestionAnswer);
        }
    }

    private void Update() {
        if (waitingForChatAIResponse) {
            if (Time.realtimeSinceStartup > waitingNextTimeToGetResponse) {
                waitingForChatAIResponse = false;
                TryGetAnswer();
            } else {
                if (Mathf.Round(Time.realtimeSinceStartup * 10f) % 5 == 0) {
                    // Update text every 0.2s
                    SetText(GetWaitingString());
                }
            }
        }
    }

    private void SendText() {
        if (!string.IsNullOrEmpty(inputTextField.value)) {
            Debug.Log("Asking Chat AI: " + inputTextField.value);
            questionLabel.text = inputTextField.value;
            SetText(GetWaitingString());
            tryGetAnswerAttemptCount = 0;

            CodeMonkeyInteractiveSO.ContactWebsiteChatAIAsk(inputTextField.value, 
                (CodeMonkeyInteractiveSO.ChatAIResponseAskSuccess chatAIResponseAskSuccess) => {
                    if (chatAIResponseAskSuccess.code == 1) {
                        // Success!
                        waitingForChatAIResponse = true;
                        waitingNextTimeToGetResponse = Time.realtimeSinceStartup + 1f;
                        SetText(GetWaitingString());
                    }
                },
                (string other) => {
                    Debug.Log("OTHER: " + other);
                    SetText("OTHER: " + other);
                },
                (string error) => {
                    Debug.Log("ERROR: " + error);
                    SetText("ERROR: " + error);
                }
            );
        }
        inputTextField.value = "";
    }

    private void TryGetAnswer() {
        tryGetAnswerAttemptCount++;

        if (tryGetAnswerAttemptCount > 50) {
            // Too many failed attempts!
            SetText("TIMEOUT");
            return;
        }

        SetText(GetWaitingString());
        CodeMonkeyInteractiveSO.ContactWebsiteChatAIGetMessage(inputTextField.value,
            (string answer) => {
                Debug.Log("Chat AI Response: " + answer);
                SetText(answer);
            },
            () => {
                waitingForChatAIResponse = true;
                waitingNextTimeToGetResponse = Time.realtimeSinceStartup + 1f;
                SetText(GetWaitingString());
            },
            (string error) => {
                Debug.Log("ERROR: " + error);
                SetText("ERROR: " + error);
            }
        );
    }

    private string GetWaitingString() {
        string ret = "Waiting..";
        for (int i = 0; i < Random.Range(0, 20); i++) {
            ret += ".";
        }
        return ret;
    }

    private void SetText(string text) {
        VisualElement textContainerVisualElement = rootVisualElement.Q<VisualElement>("textVisualElement");
        MainWindow.DestroyChildren(textContainerVisualElement);
        MainWindow.AddComplexText(
            textTemplateVisualTreeAsset,
            codeTemplateVisualTreeAsset,
            videoTemplateVisualTreeAsset,
            textContainerVisualElement,
            text
        );
    }

}