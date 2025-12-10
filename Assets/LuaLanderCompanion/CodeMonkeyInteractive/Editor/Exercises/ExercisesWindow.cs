using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeMonkey.CSharpCourse.Interactive {

    public class ExercisesWindow : EditorWindow {


        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private VisualTreeAsset exerciseSingleVisualTreeAsset;
        [SerializeField] private LectureSO defaultLectureSO;

        [SerializeField] private VisualTreeAsset textTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset codeTemplateVisualTreeAsset;
        [SerializeField] private VisualTreeAsset videoTemplateVisualTreeAsset;


        private ExerciseSO exerciseSO;
        private VisualElement exerciseSingleContainerVisualElement;
        private VisualElement overVisualElement;
        private Label topMessageLabel;
        private Label completedLabel;
        private ScrollView exerciseListScrollView;
        private Button startStopButton;
        private Button openExerciseButton;
        private Button showHintButton;
        private Button showSolutionButton;
        private Button applySolutionButton;
        private Button videoWalkthroughButton;
        private Button backButton;



        //[MenuItem("Code Monkey/Exercises", priority = 103)]
        public static void ShowWindow() {
            ExercisesWindow wnd = GetWindow<ExercisesWindow>();
            wnd.titleContent = new GUIContent("Exercises");
        }

        public void OnDestroy() {
            CodeMonkeyInteractiveSO.GetCodeMonkeyInteractiveSO().OnStateChanged -= CodeMonkeyInteractiveSO_OnStateChanged;
        }

        private void CodeMonkeyInteractiveSO_OnStateChanged(object sender, EventArgs e) {
            ShowExercise();
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

            exerciseListScrollView = root.Q<ScrollView>();

            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            if (objectField.value == null) {
                if (CodeMonkeyInteractiveSO.GetLastSelectedLectureSO() != null) {
                    objectField.value = CodeMonkeyInteractiveSO.GetLastSelectedLectureSO();
                } else {
                    objectField.value = defaultLectureSO;
                }
            }
            objectField.RegisterValueChangedCallback((ChangeEvent<UnityEngine.Object> evt) => {
                ShowExerciseList();
            });


            exerciseSingleContainerVisualElement = root.Q<VisualElement>("exerciseSingleContainer");
            overVisualElement = root.Q<VisualElement>("over");
            topMessageLabel = root.Q<Label>("topMessageLabel");
            completedLabel = root.Q<Label>("completedLabel");

            openExerciseButton = exerciseSingleContainerVisualElement.Q<Button>("openExerciseButton");
            openExerciseButton.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                exerciseSO.OpenExerciseFile();
            });

            startStopButton = exerciseSingleContainerVisualElement.Q<Button>("startStopButton");
            startStopButton.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                EditorApplication.isPlaying = false;
                exerciseSO.StartStopCompleteExercise();
            });

            showHintButton = exerciseSingleContainerVisualElement.Q<Button>("showHintButton");
            showHintButton.RegisterCallback((ClickEvent clickEvent) => {
                ShowHint(exerciseSO);
            });

            showSolutionButton = exerciseSingleContainerVisualElement.Q<Button>("showSolutionButton");
            showSolutionButton.RegisterCallback((ClickEvent clickEvent) => {
                ShowSolution(exerciseSO);
            });

            applySolutionButton = exerciseSingleContainerVisualElement.Q<Button>("applySolutionButton");
            applySolutionButton.RegisterCallback((ClickEvent clickEvent) => {
                // Apply solution
                exerciseSO.ApplySolution();
            });

            videoWalkthroughButton = exerciseSingleContainerVisualElement.Q<Button>("videoWalkthroughButton");
            videoWalkthroughButton.RegisterCallback((ClickEvent clickEvent) => {
                OpenWalkthroughVideo();
            });


            Button overCloseButton = exerciseSingleContainerVisualElement.Q<Button>("overCloseButton");
            overCloseButton.RegisterCallback((ClickEvent clickEvent) => {
                overVisualElement.style.display = DisplayStyle.None;
            });

            backButton = exerciseSingleContainerVisualElement.Q<Button>("backButton");
            backButton.RegisterCallback((ClickEvent clickEvent) => {
                ShowExerciseList();
            });

            ShowExerciseList();
        }

        private void OpenWalkthroughVideo() {
            Application.OpenURL(exerciseSO.videoWalkthroughLink);
        }

        public void SetLectureSO(LectureSO lectureSO) {
            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            objectField.value = lectureSO;
        }

        private void ShowExerciseList() {
            ObjectField objectField = rootVisualElement.Q<ObjectField>("scriptableObjectField");
            if (objectField.value != null) {
                LectureSO lectureSO = objectField.value as LectureSO;
                ShowExerciseList(lectureSO);
            }
        }

        private void ShowExerciseList(LectureSO lectureSO) {
            exerciseListScrollView.style.display = DisplayStyle.Flex;
            exerciseSingleContainerVisualElement.style.display = DisplayStyle.None;
            overVisualElement.style.display = DisplayStyle.None;

            // Remove old elements
            MainWindow.DestroyChildren(exerciseListScrollView);

            // Spawn exercises
            foreach (ExerciseSO exerciseSO in lectureSO.exerciseListSO.exerciseSOList) {
                VisualElement exerciseSingle = exerciseSingleVisualTreeAsset.Instantiate();
                exerciseSingle.Q<Button>("button").text = exerciseSO.exerciseTitle;
                exerciseSingle.RegisterCallback<ClickEvent>((ClickEvent clickEvent) => {
                    ShowExercise(exerciseSO);
                });

                exerciseSingle.Q<VisualElement>("done").style.display =
                    (CodeMonkeyInteractiveSO.GetState(exerciseSO) == CodeMonkeyInteractiveSO.State.Completed) ?
                        DisplayStyle.Flex : DisplayStyle.None;

                exerciseListScrollView.Add(exerciseSingle);
            }

            HideTopMessage();

            ExerciseSO alreadyStartedExerciseSO = CodeMonkeyInteractiveSO.GetActiveExerciseSO();
            if (alreadyStartedExerciseSO != null) {
                // Has an exercise currently active
                ShowExercise(alreadyStartedExerciseSO);
            }
        }

        private void ShowExercise() {
            if (exerciseSO != null) {
                ShowExercise(exerciseSO);
            }
        }

        private void ShowExercise(ExerciseSO exerciseSO) {
            this.exerciseSO = exerciseSO;

            exerciseListScrollView.style.display = DisplayStyle.None;
            exerciseSingleContainerVisualElement.style.display = DisplayStyle.Flex;
            overVisualElement.style.display = DisplayStyle.None;

            Label exerciseNameLabel = exerciseSingleContainerVisualElement.Q<Label>("exerciseNameLabel");
            Label exerciseTextLabel = exerciseSingleContainerVisualElement.Q<Label>("exerciseTextLabel");

            exerciseNameLabel.text = exerciseSO.exerciseTitle;
            exerciseTextLabel.text = exerciseSO.exerciseText;

            if (string.IsNullOrEmpty(exerciseSO.videoWalkthroughLink)) {
                videoWalkthroughButton.style.display = DisplayStyle.None;
            } else {
                videoWalkthroughButton.style.display = DisplayStyle.Flex;
            }

            if (exerciseSO.IsExerciseActive()) {
                // This exercise is already active
                startStopButton.text = "STOP EXERCISE";
                showHintButton.style.display = DisplayStyle.Flex;
                showSolutionButton.style.display = DisplayStyle.Flex;
                applySolutionButton.style.display = DisplayStyle.Flex;
                openExerciseButton.style.display = DisplayStyle.Flex;
                completedLabel.style.display = DisplayStyle.None;

                backButton.text = "Complete or Stop Exercise to go Back";

                if (CodeMonkeyInteractiveSO.GetState(exerciseSO) == CodeMonkeyInteractiveSO.State.Completed) {
                    // Exercise Active but already completed
                    startStopButton.text = "COMPLETE EXERCISE";
                    string[] congratsMessageArray = new string[] { 
                        "CONGRATS!",
                        "GOOD JOB!",
                        "GOOD WORK!",
                        "AWESOME!"
                    };
                    completedLabel.text = 
                        "<color=#00ff00>" + congratsMessageArray[UnityEngine.Random.Range(0, congratsMessageArray.Length)] + "</color> " + 
                        exerciseSO.completedText;
                    completedLabel.style.display = DisplayStyle.Flex;
                    /*
                    showHintButton.style.display = DisplayStyle.None;
                    showSolutionButton.style.display = DisplayStyle.None;
                    applySolutionButton.style.display = DisplayStyle.None;
                    openExerciseButton.style.display = DisplayStyle.None;
                    */
                }
            } else {
                // Exercise not active
                startStopButton.text = "START EXERCISE";
                showHintButton.style.display = DisplayStyle.None;
                showSolutionButton.style.display = DisplayStyle.None;
                applySolutionButton.style.display = DisplayStyle.None;
                openExerciseButton.style.display = DisplayStyle.None;
                completedLabel.style.display = DisplayStyle.None;
                backButton.text = "Back";
            }

            switch (CodeMonkeyInteractiveSO.GetState(exerciseSO)) {
                case CodeMonkeyInteractiveSO.State.None:
                    HideTopMessage();
                    break;
                case CodeMonkeyInteractiveSO.State.Started:
                    ShowTopMessage("Exercise in progress...", Color.yellow);
                    break;
                case CodeMonkeyInteractiveSO.State.Completed:
                    ShowTopMessage("Exercise Completed!", Color.green);
                    break;
            }
        }

        private void ShowHint(ExerciseSO exerciseSO) {
            overVisualElement.style.display = DisplayStyle.Flex;

            Label overTitleLabel = exerciseSingleContainerVisualElement.Q<Label>("overTitleLabel");
            overTitleLabel.text = "HINT";
            Label overTextLabel = exerciseSingleContainerVisualElement.Q<Label>("overTextLabel");
            overTextLabel.text = exerciseSO.hintText;
        }

        private void ShowSolution(ExerciseSO exerciseSO) {
            overVisualElement.style.display = DisplayStyle.Flex;

            Label overTitleLabel = exerciseSingleContainerVisualElement.Q<Label>("overTitleLabel");
            overTitleLabel.text = "SOLUTION";
            Label overTextLabel = exerciseSingleContainerVisualElement.Q<Label>("overTextLabel");
            overTextLabel.text = exerciseSO.solutionText;
        }

        private void HideTopMessage() {
            topMessageLabel.style.display = DisplayStyle.None;
        }

        private void ShowTopMessage(string topMessage, Color color) {
            topMessageLabel.style.display = DisplayStyle.Flex;
            topMessageLabel.text = topMessage;

            topMessageLabel.style.backgroundColor = color;
        }

    }

}