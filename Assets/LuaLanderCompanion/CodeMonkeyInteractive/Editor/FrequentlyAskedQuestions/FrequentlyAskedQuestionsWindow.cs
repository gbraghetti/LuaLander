using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeMonkey.CSharpCourse.Interactive {

    public class FrequentlyAskedQuestionsWindow : EditorWindow {


        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private VisualTreeAsset frequentlyAskedQuestionSingleVisualTreeAsset;
        [SerializeField] private LectureSO defaultLectureSO;

        [SerializeField] private VisualTreeAsset textTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset codeTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset videoTemplateVisualTreeAsset;



        private VisualElement questionSingleContainerVisualElement;
        private ScrollView questionListScrollView;
        private Button questionSingleNextButton;
        private Action questionSingleNextButtonAction;
        private bool showDebugDoneButton = false;


        [MenuItem("Code Monkey/Frequently Asked Questions", priority = 101)]
        public static void ShowWindow() {
            FrequentlyAskedQuestionsWindow editorWindow = GetWindow<FrequentlyAskedQuestionsWindow>();
            editorWindow.titleContent = new GUIContent("Frequently Asked Questions");
        }

        public void OnDestroy() {
            CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO().OnStateChanged -= CodeMonkeyInteractiveSO_OnStateChanged;
        }

        private void CodeMonkeyInteractiveSO_OnStateChanged(object sender, EventArgs e) {
        }

        public void CreateGUI() {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO().OnStateChanged -= CodeMonkeyInteractiveSO_OnStateChanged;
            CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO().OnStateChanged += CodeMonkeyInteractiveSO_OnStateChanged;

            // Instantiate UXML
            VisualElement rootVisualTreeAsset = visualTreeAsset.Instantiate();
            rootVisualTreeAsset.style.flexGrow = 1f;
            root.Add(rootVisualTreeAsset);

            questionListScrollView = root.Q<ScrollView>();
            questionListScrollView.style.display = DisplayStyle.Flex;

            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            if (objectField.value == null) {
                if (CodeMonkeyInteractiveSO.GetLastSelectedLectureSO() != null) {
                    objectField.value = CodeMonkeyInteractiveSO.GetLastSelectedLectureSO();
                } else {
                    objectField.value = defaultLectureSO;
                }
            }
            objectField.RegisterValueChangedCallback((ChangeEvent<UnityEngine.Object> evt) => {
                ShowQuestionList();
            });

            questionSingleContainerVisualElement = root.Q<VisualElement>("questionSingleContainer");
            questionSingleContainerVisualElement.style.display = DisplayStyle.None;

            Button backButton = questionSingleContainerVisualElement.Q<Button>("backButton");
            backButton.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                ShowQuestionList();
            });

            questionSingleNextButton = questionSingleContainerVisualElement.Q<Button>("nextButton");
            questionSingleNextButton.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                questionSingleNextButtonAction();
            });

            ShowQuestionList();
        }

        public void SetLectureSO(LectureSO lectureSO) {
            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            objectField.value = lectureSO;
        }

        private void ShowQuestionList() {
            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            if (objectField.value != null) {
                LectureSO lectureSO = objectField.value as LectureSO;
                ShowQuestionList(lectureSO);
            }
        }

        private void ShowQuestionList(LectureSO lectureSO) {
            questionListScrollView.style.display = DisplayStyle.Flex;
            questionSingleContainerVisualElement.style.display = DisplayStyle.None;

            if (showDebugDoneButton) {
                Debug.Log("ShowDebugDoneButton");
            }

            // Remove old questions
            MainWindow.DestroyChildren(questionListScrollView);

            // Spawn questions
            foreach (FrequentlyAskedQuestionSO frequentlyAskedQuestionSO in lectureSO.frequentlyAskedQuestionListSO.frequentlyAskedQuestionSOList) {
                VisualElement questionSingle = frequentlyAskedQuestionSingleVisualTreeAsset.Instantiate();
                questionSingle.Q<Button>("button").text = frequentlyAskedQuestionSO.title;
                questionSingle.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                    ShowQuestion(frequentlyAskedQuestionSO, lectureSO.frequentlyAskedQuestionListSO.frequentlyAskedQuestionSOList);
                });

                if (showDebugDoneButton) {
                    questionSingle.Q<VisualElement>("debugDoneButton").style.display = DisplayStyle.Flex;
                    questionSingle.Q<Button>("debugDoneButton").RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                        CodeMonkeyInteractiveSO.SetState(frequentlyAskedQuestionSO, CodeMonkeyInteractiveSO.State.Completed);
                        ShowQuestionList();
                    });
                } else {
                    questionSingle.Q("debugDoneButton").style.display = DisplayStyle.None;
                }

                questionSingle.Q<VisualElement>("done").style.display = 
                    (CodeMonkeyInteractiveSO.GetState(frequentlyAskedQuestionSO) == CodeMonkeyInteractiveSO.State.Completed) ? 
                        DisplayStyle.Flex : DisplayStyle.None;

                questionListScrollView.Add(questionSingle);
            }

        }

        private void ShowQuestion(FrequentlyAskedQuestionSO frequentlyAskedQuestionSO, List<FrequentlyAskedQuestionSO> frequentlyAskedQuestionSOList) {
            questionListScrollView.style.display = DisplayStyle.None;
            questionSingleContainerVisualElement.style.display = DisplayStyle.Flex;

            Label questionTitleLabel = questionSingleContainerVisualElement.Q<Label>("title");
            Label questionTextLabel = questionSingleContainerVisualElement.Q<Label>("text");

            questionTitleLabel.text = frequentlyAskedQuestionSO.title;

            VisualElement textContainerVisualElement = questionSingleContainerVisualElement.Q<VisualElement>("textContainerVisualElement");
            MainWindow.DestroyChildren(textContainerVisualElement);
            MainWindow.AddComplexText(
                textTemplateVisualTreeAsset,
                codeTemplateVisualTreeAsset,
                videoTemplateVisualTreeAsset,
                textContainerVisualElement,
                frequentlyAskedQuestionSO.text
            );

            CodeMonkeyInteractiveSO.SetState(frequentlyAskedQuestionSO, CodeMonkeyInteractiveSO.State.Completed);

            int index = frequentlyAskedQuestionSOList.IndexOf(frequentlyAskedQuestionSO);
            if (index < frequentlyAskedQuestionSOList.Count - 1) {
                index++;
                FrequentlyAskedQuestionSO nextFrequentlyAskedQuestionSO = frequentlyAskedQuestionSOList[index];
                questionSingleNextButton.style.display = DisplayStyle.Flex;
                questionSingleNextButtonAction = () => ShowQuestion(nextFrequentlyAskedQuestionSO, frequentlyAskedQuestionSOList);
            } else {
                questionSingleNextButton.style.display = DisplayStyle.None;
            }
        }

    }

}