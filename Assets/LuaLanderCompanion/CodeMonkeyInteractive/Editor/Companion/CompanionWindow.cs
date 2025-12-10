using CodeMonkey.CSharpCourse.Companion;
using CodeMonkey.CSharpCourse.Interactive;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CompanionWindow : EditorWindow {


    [SerializeField] private VisualTreeAsset visualTreeAsset;

    [SerializeField] private VisualTreeAsset textTemplateVisualTreeAsset;
    [SerializeField] private VisualTreeAsset codeTemplateVisualTreeAsset;
    [SerializeField] private VisualTreeAsset videoTemplateVisualTreeAsset;




    [MenuItem("Code Monkey/Companion", priority = 100)]
    public static void ShowWindow() {
        CompanionWindow window = GetWindow<CompanionWindow>();
        window.titleContent = new GUIContent("Code Monkey Companion");
    }

    public void CreateGUI() {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement rootVisualTreeAsset = visualTreeAsset.Instantiate();
        rootVisualTreeAsset.style.flexGrow = 1f;
        root.Add(rootVisualTreeAsset);

        SetText("...");

        CodeMonkeyCompanion.OnCompanionMessage += CodeMonkeyCompanion_OnCompanionMessage;
        CodeMonkeyCompanion.OnCompilationFinished += CodeMonkeyCompanion_OnCompilationFinished;

        if (CodeMonkeyCompanion.GetLastCompanionMessageEventArgs() != null) {
            CodeMonkeyCompanion_OnCompanionMessage(null, CodeMonkeyCompanion.GetLastCompanionMessageEventArgs());
        }
    }

    private void OnDestroy() {
        CodeMonkeyCompanion.OnCompanionMessage -= CodeMonkeyCompanion_OnCompanionMessage;
        CodeMonkeyCompanion.OnCompilationFinished -= CodeMonkeyCompanion_OnCompilationFinished;
    }

    private void CodeMonkeyCompanion_OnCompilationFinished(object sender, System.EventArgs e) {
        SetText("...");
    }

    private void CodeMonkeyCompanion_OnCompanionMessage(object sender, CodeMonkeyCompanion.OnCompanionMessageEventArgs e) {
        if (string.IsNullOrEmpty(e.message)) {
            // Empty message
            return;
        }

        string textPrefix = "";
        switch (e.messageType) {
            case CodeMonkeyCompanion.MessageType.Info:
                textPrefix = "INFO: ";
                break;
            case CodeMonkeyCompanion.MessageType.Warning:
                textPrefix = "<color=#ffff00>WARNING: </color>";
                break;
            case CodeMonkeyCompanion.MessageType.Error:
                textPrefix = "<color=#ff0000>ERROR: </color>";
                break;
        }

        SetText(textPrefix + e.message);
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