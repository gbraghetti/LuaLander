using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeMonkey.CSharpCourse.Interactive {

    public class QuizWindow : EditorWindow {


        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private VisualTreeAsset quizSingleVisualTreeAsset;
        [SerializeField] private VisualTreeAsset quizOptionSingleVisualTreeAsset;
        [SerializeField] private LectureSO defaultLectureSO;

        [SerializeField] private VisualTreeAsset textTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset codeTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset videoTemplateVisualTreeAsset;


        private VisualElement quizSingleContainerVisualElement;
        private ScrollView quizListScrollView;
        private Button questionSingleNextButton;
        private Action questionSingleNextButtonAction;
        private bool showDebugDoneButton = false;


        [MenuItem("Code Monkey/Quiz", priority = 102)]
        public static void ShowExample() {
            QuizWindow wnd = GetWindow<QuizWindow>();
            wnd.titleContent = new GUIContent("Quiz");
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

            quizListScrollView = root.Q<ScrollView>();

            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            if (objectField.value == null) {
                if (CodeMonkeyInteractiveSO.GetLastSelectedLectureSO() != null) {
                    objectField.value = CodeMonkeyInteractiveSO.GetLastSelectedLectureSO();
                } else {
                    objectField.value = defaultLectureSO;
                }
            }
            objectField.RegisterValueChangedCallback((ChangeEvent<UnityEngine.Object> evt) => {
                ShowQuizList();
            });


            quizSingleContainerVisualElement = root.Q<VisualElement>("quizSingleContainer");

            Button backButton = quizSingleContainerVisualElement.Q<Button>("backButton");
            backButton.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                ShowQuizList();
            });

            questionSingleNextButton = quizSingleContainerVisualElement.Q<Button>("nextButton");
            questionSingleNextButton.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                questionSingleNextButtonAction();
            });

            ShowQuizList();
        }

        public void SetLectureSO(LectureSO lectureSO) {
            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            objectField.value = lectureSO;
        }

        private void ShowQuizList() {
            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            if (objectField.value != null) {
                LectureSO lectureSO = objectField.value as LectureSO;
                ShowQuizList(lectureSO);
            }
        }

        private void ShowQuizList(LectureSO lectureSO) {
            quizListScrollView.style.display = DisplayStyle.Flex;
            quizSingleContainerVisualElement.style.display = DisplayStyle.None;

            if (showDebugDoneButton) {
                Debug.Log("ShowDebugDoneButton");
            }

            // Remove old elements
            MainWindow.DestroyChildren(quizListScrollView);

            // Spawn questions
            foreach (QuizSO quizSO in lectureSO.quizListSO.quizSOList) {
                VisualElement quizSingle = quizSingleVisualTreeAsset.Instantiate();
                string quizQuestion = quizSO.question;
                if (quizQuestion.Length > 100) {
                    quizQuestion = quizQuestion.Substring(0, 100) + "...";
                }
                quizSingle.Q<Button>("button").text = quizQuestion;
                quizSingle.Q<Button>("button").RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                    ShowQuiz(quizSO, lectureSO.quizListSO);
                });

                if (showDebugDoneButton) {
                    quizSingle.Q<VisualElement>("debugDoneButton").style.display = DisplayStyle.Flex;
                    quizSingle.Q<Button>("debugDoneButton").RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                        CodeMonkeyInteractiveSO.SetState(quizSO, CodeMonkeyInteractiveSO.State.Completed);
                        ShowQuizList();
                    });
                } else {
                    quizSingle.Q("debugDoneButton").style.display = DisplayStyle.None;
                }


                quizSingle.Q<VisualElement>("done").style.display =
                    (CodeMonkeyInteractiveSO.GetState(quizSO) == CodeMonkeyInteractiveSO.State.Completed) ?
                        DisplayStyle.Flex : DisplayStyle.None;

                quizListScrollView.Add(quizSingle);
            }
        }

        private void ShowQuiz(QuizSO quizSO, QuizListSO quizListSO) {
            quizListScrollView.style.display = DisplayStyle.None;
            quizSingleContainerVisualElement.style.display = DisplayStyle.Flex;

            Label questionText = quizSingleContainerVisualElement.Q<Label>("questionText");
            questionText.text = quizSO.question;

            // Remove old options
            VisualElement optionContainer = quizSingleContainerVisualElement.Q<VisualElement>("optionContainer");
            MainWindow.DestroyChildren(optionContainer);

            int optionIndex = 0;
            foreach (string optionString in quizSO.optionList) {
                VisualElement quizOptionSingle = quizOptionSingleVisualTreeAsset.Instantiate();
                optionContainer.Add(quizOptionSingle);
                Button optionButton = quizOptionSingle.Q<Button>("optionButton");
                optionButton.text = optionString;
                int clickOptionIndex = optionIndex;
                optionButton.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                    ShowQuizAnswered(quizSO, clickOptionIndex, quizListSO);
                });
                optionIndex++;
            }

            VisualElement answerContainerVisualElement = quizSingleContainerVisualElement.Q<VisualElement>("answerContainerVisualElement");
            MainWindow.DestroyChildren(answerContainerVisualElement);
            MainWindow.AddComplexText(
                textTemplateVisualTreeAsset,
                codeTemplateVisualTreeAsset,
                videoTemplateVisualTreeAsset,
                answerContainerVisualElement,
                quizSO.answer
            );

            ScrollView answerScrollView = quizSingleContainerVisualElement.Q<ScrollView>("answerScrollView");
            answerScrollView.style.display = DisplayStyle.None;

            // Hide Next button
            questionSingleNextButton.style.display = DisplayStyle.None;
        }

        private void ShowQuizAnswered(QuizSO quizSO, int selectedOptionIndex, QuizListSO quizListSO) {
            StyleColor greenStyleColor = new StyleColor(UtilsClass.GetColorFromString("00ff00"));
            StyleColor redStyleColor = new StyleColor(UtilsClass.GetColorFromString("ff0000"));
            StyleColor whiteStyleColor = new StyleColor(UtilsClass.GetColorFromString("ffffff"));

            VisualElement optionContainer = quizSingleContainerVisualElement.Q<VisualElement>("optionContainer");
            List<VisualElement> optionContainerChildList = optionContainer.Children().ToList();
            foreach (VisualElement optionVisualElement in optionContainerChildList) {
                optionVisualElement.Q<Button>().style.borderTopColor = whiteStyleColor;
                optionVisualElement.Q<Button>().style.borderBottomColor = whiteStyleColor;
                optionVisualElement.Q<Button>().style.borderLeftColor = whiteStyleColor;
                optionVisualElement.Q<Button>().style.borderRightColor = whiteStyleColor;
            }

            if (selectedOptionIndex != quizSO.correctOptionIndex) {
                // Player got wrong
                optionContainerChildList[selectedOptionIndex].Q<Button>().style.borderBottomColor = redStyleColor;
                optionContainerChildList[selectedOptionIndex].Q<Button>().style.borderTopColor = redStyleColor;
                optionContainerChildList[selectedOptionIndex].Q<Button>().style.borderLeftColor = redStyleColor;
                optionContainerChildList[selectedOptionIndex].Q<Button>().style.borderRightColor = redStyleColor;
            }
            optionContainerChildList[quizSO.correctOptionIndex].Q<Button>().style.borderBottomColor = greenStyleColor;
            optionContainerChildList[quizSO.correctOptionIndex].Q<Button>().style.borderTopColor = greenStyleColor;
            optionContainerChildList[quizSO.correctOptionIndex].Q<Button>().style.borderLeftColor = greenStyleColor;
            optionContainerChildList[quizSO.correctOptionIndex].Q<Button>().style.borderRightColor = greenStyleColor;

            ScrollView answerScrollView = quizSingleContainerVisualElement.Q<ScrollView>("answerScrollView");
            answerScrollView.style.display = DisplayStyle.Flex;

            CodeMonkeyInteractiveSO.SetState(quizSO, CodeMonkeyInteractiveSO.State.Completed);

            int index = quizListSO.quizSOList.IndexOf(quizSO);
            if (index < quizListSO.quizSOList.Count - 1) {
                index++;
                QuizSO nextQuizSO = quizListSO.quizSOList[index];
                questionSingleNextButton.style.display = DisplayStyle.Flex;
                questionSingleNextButtonAction = () => ShowQuiz(nextQuizSO, quizListSO);
            } else {
                questionSingleNextButton.style.display = DisplayStyle.None;
            }
        }
    }

}