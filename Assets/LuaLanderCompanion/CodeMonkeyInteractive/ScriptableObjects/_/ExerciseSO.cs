using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CodeMonkey.CSharpCourse.Interactive {

    [CreateAssetMenu()]
    public class ExerciseSO : ScriptableObject {


        public string exerciseName;
        public string exerciseTitle;

        [TextArea(5, 10)]
        public string exerciseText;

        [TextArea(5, 10)]
        public string hintText;

        [TextArea(5, 10)]
        public string solutionText;

        [TextArea(5, 10)]
        public string completedText;

        public string videoWalkthroughLink;


        public void LoadExerciseScene() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
            string exercisesFolderPath = Application.dataPath +
                $"/Lectures/{lectureFolder}/Exercises/";

            string filePath = exercisesFolderPath + "ExerciseScene.unity";
            if (File.Exists(filePath)) {
                filePath = filePath.Substring(Application.dataPath.Length);
                filePath = "Assets" + filePath;
                EditorSceneManager.OpenScene(filePath);
                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(filePath));
            }
        }

        public void TryPingExerciseFile() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
            string exercisesFolderPath = Application.dataPath +
                $"/Lectures/{lectureFolder}/Exercises/";

            string filePath = exercisesFolderPath + "Exercise.cs";
            if (File.Exists(filePath)) {
                filePath = filePath.Substring(Application.dataPath.Length);
                filePath = "Assets" + filePath;
                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(filePath));
            }
        }

        public void OpenExerciseFile() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
            string exercisesFolderPath = Application.dataPath +
                $"/Lectures/{lectureFolder}/Exercises/";

            string filePath = exercisesFolderPath + "ExerciseScene.unity";
            if (File.Exists(filePath)) {
                filePath = filePath.Substring(Application.dataPath.Length);
                filePath = "Assets" + filePath;
                EditorSceneManager.OpenScene(filePath);
            }

            filePath = exercisesFolderPath + "Exercise.cs";
            if (File.Exists(filePath)) {
                filePath = filePath.Substring(Application.dataPath.Length);
                filePath = "Assets" + filePath;
                EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(filePath));
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(filePath));
            }
        }

        public void StartStopCompleteExercise() {
            if (IsExerciseActive()) {
                if (CodeMonkeyInteractiveSO.GetState(this) == CodeMonkeyInteractiveSO.State.Completed) {
                    // Completed
                    CompleteExercise();
                } else {
                    StopExercise();
                }
            } else {
                StartExercise();
                OpenExerciseFile();
            }
        }

        private void StartExercise() {
            Debug.Log("Starting Exercise " + this.exerciseName);
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
            string exercisesFolderPath = Application.dataPath +
                $"/Lectures/{lectureFolder}/Exercises/";
            string zipPath = exercisesFolderPath + $"{lectureFolder}_Exercise_{exerciseName}.zip";

            List<string> unpackedFilePathList = new List<string>();

            using (ZipArchive zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Read)) {
                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries) {
                    bool isSolutionFile = zipArchiveEntry.FullName.Contains("Solution");
                    if (!isSolutionFile) {
                        // Not solution, unpack
                        zipArchiveEntry.ExtractToFile(exercisesFolderPath + zipArchiveEntry.Name, true);
                        unpackedFilePathList.Add(exercisesFolderPath + zipArchiveEntry.Name);
                    }
                }
            }

            CodeMonkeyInteractiveSO.SetActiveExerciseSO(this);
            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.Started);

            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();

            TryPingExerciseFile();
            LoadExerciseScene();
        }

        public void StopExercise() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            if (EditorApplication.isPlaying) {
                Debug.LogWarning("Stop Playing in Unity in order to Stop/Complete the exercise!");
                Debug.LogWarning("Stop Playing in Unity in order to Stop/Complete the exercise!");
                Debug.LogWarning("Stop Playing in Unity in order to Stop/Complete the exercise!");
                EditorApplication.isPlaying = false;
            }

            CodeMonkeyInteractiveSO.LoadDefaultScene();

            string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
            string exercisesFolderPath = Application.dataPath +
                $"/Lectures/{lectureFolder}/Exercises/";

            if (Directory.Exists(exercisesFolderPath)) {
                string[] fileArray = Directory.GetFiles(exercisesFolderPath);
                foreach (string fileName in fileArray) {
                    bool isMeta = fileName.Contains(".meta");
                    bool isZip = fileName.Contains(".zip");
                    if (!isMeta && !isZip) {
                        File.Delete(fileName);
                    }
                }
            }

            CodeMonkeyInteractiveSO.ClearActiveExerciseSO();
            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.None);

            AssetDatabase.Refresh();
        }

        public void CompleteExercise() {
            StopExercise();
            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.Completed);
        }

        public void ApplySolution() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
            string exercisesFolderPath = Application.dataPath +
                $"/Lectures/{lectureFolder}/Exercises/";
            string zipPath = exercisesFolderPath + $"{lectureFolder}_Exercise_{exerciseName}.zip";

            using (ZipArchive zipArchive = ZipFile.Open(zipPath, ZipArchiveMode.Read)) {
                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries) {
                    bool isSolutionFile = zipArchiveEntry.FullName.Contains("Solution");
                    if (isSolutionFile && !string.IsNullOrEmpty(zipArchiveEntry.Name)) {
                        // Is solution, unpack
                        zipArchiveEntry.ExtractToFile(exercisesFolderPath + zipArchiveEntry.Name, true);
                    }
                }
            }

            CodeMonkeyInteractiveSO.SetState(this, CodeMonkeyInteractiveSO.State.Started);

            AssetDatabase.Refresh();
        }

        public bool IsExerciseActive() {
            LectureSO lectureSO = LectureSO.GetLectureSO(this);

            string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
            string exercisesFolderPath = Application.dataPath +
                $"/Lectures/{lectureFolder}/Exercises/";

            if (Directory.Exists(exercisesFolderPath)) {
                string[] fileArray = Directory.GetFiles(exercisesFolderPath, "*.cs");
                if (fileArray.Length > 0) {
                    // This exercise is active!
                    return true;
                }
            }

            // Exercise not active
            return false;
        }


        public static bool IsAnyExerciseActive() {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();
            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                if (lectureSO.exerciseListSO.exerciseSOList.Count > 0) {
                    // This lecture has exercises, check if any are active
                    string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
                    string exercisesFolderPath = Application.dataPath +
                        $"/Lectures/{lectureFolder}/Exercises/";

                    if (Directory.Exists(exercisesFolderPath)) {
                        string[] fileArray = Directory.GetFiles(exercisesFolderPath, "*.cs");
                        if (fileArray.Length > 0) {
                            // This exercise is active!
                            return true;
                        }
                    }
                }
            }
            // No exercises are active
            return false;
        }

        public static void TryRemoveCompilationBlockers() {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();
            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                if (lectureSO.exerciseListSO.exerciseSOList.Count > 0) {
                    // This lecture has exercises, check if any are active
                    string lectureFolder = $"{lectureSO.lectureCode}_{lectureSO.lectureName}";
                    string exercisesFolderPath = Application.dataPath +
                        $"/Lectures/{lectureFolder}/Exercises/";

                    // Is the exercise active?
                    if (Directory.Exists(exercisesFolderPath)) {
                        bool wasAnyFileChanged = false;

                        // Try to remove Compilation blockers if they exist in any of the files
                        string[] fileArray = Directory.GetFiles(exercisesFolderPath, "*.cs");
                        foreach (string fileName in fileArray) {
                            string unpackedFileText = File.ReadAllText(fileName);
                            const string COMPILATION_COMMENT_START = "/* COMPILATION BLOCKER";
                            const string COMPILATION_COMMENT_END = "COMPILATION BLOCKER */";
                            if (unpackedFileText.Contains(COMPILATION_COMMENT_START)) {
                                unpackedFileText = unpackedFileText.Replace(COMPILATION_COMMENT_START, "");
                                unpackedFileText = unpackedFileText.Replace(COMPILATION_COMMENT_END, "");
                                File.WriteAllText(fileName, unpackedFileText);
                                wasAnyFileChanged = true;
                            }
                        }
                        if (wasAnyFileChanged) {
                            AssetDatabase.Refresh();
                            CompilationPipeline.RequestScriptCompilation();
                        }
                    }
                }
            }
        }

    }

}