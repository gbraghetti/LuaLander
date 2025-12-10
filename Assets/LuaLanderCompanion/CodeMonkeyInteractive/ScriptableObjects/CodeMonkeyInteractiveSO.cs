using CodeMonkey.CSharpCourse.Interactive;
using CodeMonkey;
using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CodeMonkey.CSharpCourse.Interactive {

    [CreateAssetMenu()]
    public class CodeMonkeyInteractiveSO : ScriptableObject {


        private static CodeMonkeyInteractiveSO codeMonkeyInteractiveSO;


        private const long SECONDS_BETWEEN_CONTACTING_WEBSITE = 3600;


        public event EventHandler OnStateChanged;


        public enum State {
            None,
            Started,
            Completed,
        }

        [Serializable]
        public class QuizState {

            public QuizSO quizSO;
            public State state;

        }

        [Serializable]
        public class FrequentlyAskedQuestionState {

            public FrequentlyAskedQuestionSO frequentlyAskedQuestionSO;
            public State state;

        }

        [Serializable]
        public class ExerciseState {

            public ExerciseSO exerciseSO;
            public State state;

        }


        public string currentVersion;
        public List<QuizState> quizStateList;
        public List<FrequentlyAskedQuestionState> frequentlyAskedQuestionStateList;
        public List<ExerciseState> exerciseStateList;
        public LectureListSO lectureListSO;
        public SceneAsset defaultScene;

        [SerializeField] private ExerciseSO activeExerciseSO;
        [SerializeField] private LectureSO lastSelectedLectureSO;
        [SerializeField] private long checkedLastUpdateTimestamp;
        [SerializeField] private string lastUpdateVersion;
        [SerializeField] private LastQOTDResponse lastQOTDResponse;
        [SerializeField] private long lastQotdTimestamp;
        [SerializeField] private WebsiteLatestMessage websiteLatestMessage;
        [SerializeField] private long websiteLatestMessageTimestamp;
        [SerializeField] private WebsiteDynamicMessage websiteDynamicMessage;
        [SerializeField] private long websiteDynamicMessageTimestamp;
        [SerializeField] private LatestVideos websiteLatestVideos;
        [SerializeField] private long websiteLatestVideosTimestamp;
        [SerializeField] private string lastChatAIQuestionAsked;
        [SerializeField] private string lastChatAIQuestionAnswer;
        [SerializeField] private ChatAIResponseAskSuccess lastChatAIResponseAskSuccess;


        public static void SetState(QuizSO quizSO, State state) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            for (int i = 0; i < codeMonkeyInteractiveSO.quizStateList.Count; i++) {
                QuizState quizState = codeMonkeyInteractiveSO.quizStateList[i];
                if (quizState.quizSO == quizSO) {
                    quizState.state = state;
                    Save();
                    codeMonkeyInteractiveSO.OnStateChanged?.Invoke(null, EventArgs.Empty);
                    return;
                }
            }
            // No entry in list yet
            codeMonkeyInteractiveSO.quizStateList.Add(new QuizState {
                quizSO = quizSO,
                state = state,
            });
            Save();
            codeMonkeyInteractiveSO.OnStateChanged?.Invoke(null, EventArgs.Empty);
        }

        public static State GetState(QuizSO quizSO) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            foreach (QuizState quizState in codeMonkeyInteractiveSO.quizStateList) {
                if (quizState.quizSO == quizSO) {
                    return quizState.state;
                }
            }
            return State.None;
        }

        public static void SetState(FrequentlyAskedQuestionSO frequentlyAskedQuestionSO, State state) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            for (int i = 0; i < codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList.Count; i++) {
                FrequentlyAskedQuestionState frequentlyAskedQuestionState = codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList[i];
                if (frequentlyAskedQuestionState.frequentlyAskedQuestionSO == frequentlyAskedQuestionSO) {
                    frequentlyAskedQuestionState.state = state;
                    Save();
                    codeMonkeyInteractiveSO.OnStateChanged?.Invoke(null, EventArgs.Empty);
                    return;
                }
            }
            // No entry in list yet
            codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList.Add(new FrequentlyAskedQuestionState {
                frequentlyAskedQuestionSO = frequentlyAskedQuestionSO,
                state = state
            });
            Save();
            codeMonkeyInteractiveSO.OnStateChanged?.Invoke(null, EventArgs.Empty);
        }

        public static State GetState(FrequentlyAskedQuestionSO frequentlyAskedQuestionSO) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            foreach (FrequentlyAskedQuestionState frequentlyAskedQuestionState in codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList) {
                if (frequentlyAskedQuestionState.frequentlyAskedQuestionSO == frequentlyAskedQuestionSO) {
                    return frequentlyAskedQuestionState.state;
                }
            }
            return State.None;
        }

        public static void SetState(ExerciseSO exerciseSO, State state) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            for (int i = 0; i < codeMonkeyInteractiveSO.exerciseStateList.Count; i++) {
                ExerciseState exerciseState = codeMonkeyInteractiveSO.exerciseStateList[i];
                if (exerciseState.exerciseSO == exerciseSO) {
                    exerciseState.state = state;
                    Save();
                    codeMonkeyInteractiveSO.OnStateChanged?.Invoke(null, EventArgs.Empty);
                    return;
                }
            }
            // No entry in list yet
            codeMonkeyInteractiveSO.exerciseStateList.Add(new ExerciseState {
                exerciseSO = exerciseSO,
                state = state
            });
            Save();
            codeMonkeyInteractiveSO.OnStateChanged?.Invoke(null, EventArgs.Empty);
        }

        public static State GetState(ExerciseSO exerciseSO) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            foreach (ExerciseState exerciseState in codeMonkeyInteractiveSO.exerciseStateList) {
                if (exerciseState.exerciseSO == exerciseSO) {
                    return exerciseState.state;
                }
            }
            return State.None;
        }

        public static CodeMonkeyInteractiveSO GetCodeMonkeyInteractiveSO() {
            if (codeMonkeyInteractiveSO != null) {
                return codeMonkeyInteractiveSO;
            }
            string[] codeMonkeyInteractiveSOGuidArray = AssetDatabase.FindAssets(nameof(CodeMonkeyInteractiveSO));

            foreach (string codeMonkeyInteractiveSOGuid in codeMonkeyInteractiveSOGuidArray) {
                string codeMonkeyInteractiveSOPath = AssetDatabase.GUIDToAssetPath(codeMonkeyInteractiveSOGuid);
                codeMonkeyInteractiveSO = AssetDatabase.LoadAssetAtPath<CodeMonkeyInteractiveSO>(codeMonkeyInteractiveSOPath);
                return codeMonkeyInteractiveSO;
            }

            Debug.LogError("Cannot find CodeMonkeyInteractiveSO!");
            return null;
        }

        public static void GetCompletionStats(
            out int faqCompleted, out int faqTotals,
            out int quizCompleted, out int quizTotals,
            out int exercisesCompleted, out int exercisesTotals) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();

            faqCompleted = 0;
            faqTotals = 0;
            quizCompleted = 0;
            quizTotals = 0;
            exercisesCompleted = 0;
            exercisesTotals = 0;

            foreach (FrequentlyAskedQuestionState frequentlyAskedQuestionState in codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList) {
                if (frequentlyAskedQuestionState.state == State.Completed) {
                    faqCompleted++;
                }
            }
            foreach (QuizState quizState in codeMonkeyInteractiveSO.quizStateList) {
                if (quizState.state == State.Completed) {
                    quizCompleted++;
                }
            }
            foreach (ExerciseState exerciseState in codeMonkeyInteractiveSO.exerciseStateList) {
                if (exerciseState.state == State.Completed) {
                    exercisesCompleted++;
                }
            }

            foreach (LectureSO lectureSO in codeMonkeyInteractiveSO.lectureListSO.lectureSOList) {
                faqTotals += lectureSO.frequentlyAskedQuestionListSO.frequentlyAskedQuestionSOList.Count;
                quizTotals += lectureSO.quizListSO.quizSOList.Count;
                //exercisesTotals += lectureSO.exerciseListSO.exerciseSOList.Count;
            }
        }

        public static void SetLastSelectedLectureSO(LectureSO lectureSO) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            codeMonkeyInteractiveSO.lastSelectedLectureSO = lectureSO;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);
        }

        public static LectureSO GetLastSelectedLectureSO() {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            return codeMonkeyInteractiveSO.lastSelectedLectureSO;
        }

        public static void ClearActiveExerciseSO() {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            codeMonkeyInteractiveSO.activeExerciseSO = null;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);
        }

        public static void SetActiveExerciseSO(ExerciseSO exerciseSO) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            codeMonkeyInteractiveSO.activeExerciseSO = exerciseSO;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);
        }

        public static ExerciseSO GetActiveExerciseSO() {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            return codeMonkeyInteractiveSO.activeExerciseSO;
        }

        public static void LoadDefaultScene() {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(codeMonkeyInteractiveSO.defaultScene));
        }


        [Serializable]
        private struct GenericActionJSONData {
            public string at;
        }

        [Serializable]
        private struct WebsiteResponse {
            public int returnCode;
            public string returnText;
        }

        [Serializable]
        private struct WebsiteResponse<T> {
            public int returnCode;
            public T returnText;
        }

        public static void CheckForUpdates(Action<string, string> onFoundUpdate) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (codeMonkeyInteractiveSO.lastUpdateVersion == null || codeMonkeyInteractiveSO.lastUpdateVersion == "") {
                codeMonkeyInteractiveSO.lastUpdateVersion = codeMonkeyInteractiveSO.currentVersion;
            }

            long secondsBetweenCheckingForUpdates = 3600;
            if (unixTimestamp - codeMonkeyInteractiveSO.checkedLastUpdateTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onFoundUpdate(codeMonkeyInteractiveSO.currentVersion, codeMonkeyInteractiveSO.lastUpdateVersion);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.checkedLastUpdateTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "courselunarlanderversion",
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onFoundUpdate(codeMonkeyInteractiveSO.currentVersion, codeMonkeyInteractiveSO.lastUpdateVersion);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            codeMonkeyInteractiveSO.lastUpdateVersion = websiteResponse.returnText;
                            onFoundUpdate(codeMonkeyInteractiveSO.currentVersion, codeMonkeyInteractiveSO.lastUpdateVersion);
                        } else {
                            // Something went wrong
                            onFoundUpdate(codeMonkeyInteractiveSO.currentVersion, codeMonkeyInteractiveSO.lastUpdateVersion);
                        }
                    }
                } catch (Exception) {
                    onFoundUpdate(codeMonkeyInteractiveSO.currentVersion, codeMonkeyInteractiveSO.lastUpdateVersion);
                }
                unityWebRequest.Dispose();
            };
        }

        [Serializable]
        public struct LastQOTDResponse {
            public string questionId;
            public string questionText;
            public string answerA;
            public string answerB;
            public string answerC;
            public string answerD;
            public string answerE;
        }

        public static void GetLastQOTD(Action<LastQOTDResponse> onResponse) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long secondsBetweenCheckingForUpdates = SECONDS_BETWEEN_CONTACTING_WEBSITE;
            if (unixTimestamp - codeMonkeyInteractiveSO.lastQotdTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.lastQotdTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "getLastQotd",
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        downloadText = downloadText.Replace("&lt;", "<");
                        downloadText = downloadText.Replace("&gt;", ">");
                        downloadText = downloadText.Replace("&#60;", "<");
                        downloadText = downloadText.Replace("&#62;", ">");
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            LastQOTDResponse lastQOTDResponse = JsonUtility.FromJson<LastQOTDResponse>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.lastQOTDResponse = lastQOTDResponse;
                            onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                        } else {
                            // Something went wrong
                            onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                        }
                    }
                } catch (Exception) {
                    onResponse(codeMonkeyInteractiveSO.lastQOTDResponse);
                }
                unityWebRequest.Dispose();
            };
        }



        [Serializable]
        public struct WebsiteLatestMessage {
            public string text;
        }

        public static void GetLatestMessage(Action<WebsiteLatestMessage> onResponse) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long secondsBetweenCheckingForUpdates = SECONDS_BETWEEN_CONTACTING_WEBSITE;
            if (unixTimestamp - codeMonkeyInteractiveSO.websiteLatestMessageTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.websiteLatestMessageTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "courselunarlanderlatestMessage",
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            WebsiteLatestMessage websiteLatestMessage = JsonUtility.FromJson<WebsiteLatestMessage>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.websiteLatestMessage = websiteLatestMessage;
                            onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                        } else {
                            // Something went wrong
                            onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                        }
                    }
                } catch (Exception) {
                    onResponse(codeMonkeyInteractiveSO.websiteLatestMessage);
                }
                unityWebRequest.Dispose();
            };
        }



        [Serializable]
        public struct WebsiteDynamicMessage {
            public string text;
            public string url;
        }

        public static void GetWebsiteDynamicMessage(Action<WebsiteDynamicMessage> onResponse) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long secondsBetweenCheckingForUpdates = SECONDS_BETWEEN_CONTACTING_WEBSITE;
            if (unixTimestamp - codeMonkeyInteractiveSO.websiteDynamicMessageTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onResponse(codeMonkeyInteractiveSO.websiteDynamicMessage);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.websiteDynamicMessageTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "courselunarlanderdynamicmessage",
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onResponse(codeMonkeyInteractiveSO.websiteDynamicMessage);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            WebsiteDynamicMessage websiteDynamicMessage = JsonUtility.FromJson<WebsiteDynamicMessage>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.websiteDynamicMessage = websiteDynamicMessage;
                            onResponse(codeMonkeyInteractiveSO.websiteDynamicMessage);
                        } else {
                            // Something went wrong
                            onResponse(codeMonkeyInteractiveSO.websiteDynamicMessage);
                        }
                    }
                } catch (Exception) {
                    onResponse(codeMonkeyInteractiveSO.websiteDynamicMessage);
                }
                unityWebRequest.Dispose();
            };
        }




        [Serializable]
        public class LatestVideos {
            public LatestVideoSingle[] videos;
        }

        [Serializable]
        public class LatestVideoSingle {
            public string youTubeId;
            public string title;
        }

        public static void GetWebsiteLatestVideos(Action<LatestVideos> onResponse) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            long secondsBetweenCheckingForUpdates = SECONDS_BETWEEN_CONTACTING_WEBSITE;
            if (unixTimestamp - codeMonkeyInteractiveSO.websiteLatestVideosTimestamp < secondsBetweenCheckingForUpdates) {
                // Too soon
                onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                return;
            }

            // Enough time has passed to check for update
            codeMonkeyInteractiveSO.websiteLatestVideosTimestamp = unixTimestamp;
            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new GenericActionJSONData {
                at = "getLastVideos",
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        //onError(unityWebRequest.error);
                        onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            LatestVideos websiteLatestVideos = JsonUtility.FromJson<LatestVideos>(websiteResponse.returnText);
                            codeMonkeyInteractiveSO.websiteLatestVideos = websiteLatestVideos;
                            onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                        } else {
                            // Something went wrong
                            onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                        }
                    }
                } catch (Exception) {
                    onResponse(codeMonkeyInteractiveSO.websiteLatestVideos);
                }
                unityWebRequest.Dispose();
            };
        }


        [Serializable]
        private struct ChatAIQuestionAskJSONData {
            public string at;
            public string question;
        }

        [Serializable]
        private struct ChatAIResponseCode {
            public int code;
        }

        [Serializable]
        public struct ChatAIResponseMessage {
            public int code;
            public string message;
        }

        [Serializable]
        public struct ChatAIResponseAskSuccess {
            public int code;
            public string runId;
            public string threadId;
        }

        public static void ContactWebsiteChatAIAsk(string question, Action<ChatAIResponseAskSuccess> onSuccess, Action<string> onOtherResponse, Action<string> onError) {
            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            string jsonData = JsonUtility.ToJson(new ChatAIQuestionAskJSONData {
                at = "courselunarlanderaichat_ask",
                question = question,
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        onError(unityWebRequest.error);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        //Debug.Log("DownloadText: " + downloadText);
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            ChatAIResponseCode chatAIResponseCode = JsonUtility.FromJson<ChatAIResponseCode>(websiteResponse.returnText);

                            switch (chatAIResponseCode.code) {
                                case 1: // Success!
                                    ChatAIResponseAskSuccess chatAIResponseAskSuccess = JsonUtility.FromJson<ChatAIResponseAskSuccess>(websiteResponse.returnText);

                                    CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();
                                    codeMonkeyInteractiveSO.lastChatAIQuestionAsked = question;
                                    codeMonkeyInteractiveSO.lastChatAIQuestionAnswer = "";
                                    codeMonkeyInteractiveSO.lastChatAIResponseAskSuccess = chatAIResponseAskSuccess;
                                    EditorUtility.SetDirty(codeMonkeyInteractiveSO);

                                    onSuccess(chatAIResponseAskSuccess);
                                    break;
                                default:
                                    ChatAIResponseMessage chatAIResponseMessage = JsonUtility.FromJson<ChatAIResponseMessage>(websiteResponse.returnText);
                                    onOtherResponse(chatAIResponseMessage.message);
                                    break;
                            }
                        } else {
                            // Something went wrong
                            onError("Something went wrong...");
                        }
                    }
                } catch (Exception) {
                    onError("Exception!");
                }
                unityWebRequest.Dispose();
            };
        }



        [Serializable]
        private struct ChatAIQuestionGetMessageJSONData {
            public string at;
            public string runId;
            public string threadId;
        }

        public static void ContactWebsiteChatAIGetMessage(string question, Action<string> onResponse, Action onResponseNotYetReady, Action<string> onError) {
            string url = "https://unitycodemonkey.com/generic_action_json.php";
            UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");

            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();

            string jsonData = JsonUtility.ToJson(new ChatAIQuestionGetMessageJSONData {
                at = "courselunarlanderaichat_getmessage",
                runId = codeMonkeyInteractiveSO.lastChatAIResponseAskSuccess.runId,
                threadId = codeMonkeyInteractiveSO.lastChatAIResponseAskSuccess.threadId,
            });
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            unityWebRequest.SendWebRequest().completed += (AsyncOperation asyncOperation) => {
                try {
                    UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = asyncOperation as UnityWebRequestAsyncOperation;

                    if (unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ConnectionError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                        unityWebRequestAsyncOperation.webRequest.result == UnityWebRequest.Result.ProtocolError) {
                        // Error
                        onError(unityWebRequest.error);
                    } else {
                        string downloadText = unityWebRequest.downloadHandler.text;
                        WebsiteResponse websiteResponse = JsonUtility.FromJson<WebsiteResponse>(downloadText);
                        if (websiteResponse.returnCode == 1) {
                            // Success
                            codeMonkeyInteractiveSO.lastChatAIQuestionAnswer = websiteResponse.returnText;
                            EditorUtility.SetDirty(codeMonkeyInteractiveSO);

                            onResponse(websiteResponse.returnText);
                        } else {
                            // Something went wrong
                            if (websiteResponse.returnCode == 2) {
                                // Still pending
                                onResponseNotYetReady();
                            } else {
                                onError("Something went wrong...");
                            }
                        }
                    }
                } catch (Exception e) {
                    onError("Exception! " + e);
                }
                unityWebRequest.Dispose();
            };
        }

        public static void GetLastChatAIData(out string lastChatAIQuestionAsked, out string lastChatAIQuestionAnswer) {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();

            lastChatAIQuestionAsked = codeMonkeyInteractiveSO.lastChatAIQuestionAsked;
            lastChatAIQuestionAnswer = codeMonkeyInteractiveSO.lastChatAIQuestionAnswer;
        }








        [Serializable]
        public class SaveObject {
            public List<SaveObjectSingle> faqList;
            public List<SaveObjectSingle> quizList;
            public List<SaveObjectSingle> exerciseList;
        }

        [Serializable]
        public class SaveObjectSingle {
            public string name;
            public State state;
        }

        public static void Save() {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();

            SaveObject saveObject = new SaveObject();
            saveObject.faqList = new List<SaveObjectSingle>();
            saveObject.quizList = new List<SaveObjectSingle>();
            saveObject.exerciseList = new List<SaveObjectSingle>();

            foreach (QuizState quizState in codeMonkeyInteractiveSO.quizStateList) {
                if (quizState.quizSO == null) continue;
                saveObject.quizList.Add(new SaveObjectSingle {
                    name = quizState.quizSO.name,
                    state = quizState.state,
                });
            }
            foreach (FrequentlyAskedQuestionState frequentlyAskedQuestionState in codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList) {
                if (frequentlyAskedQuestionState.frequentlyAskedQuestionSO == null) continue;
                saveObject.faqList.Add(new SaveObjectSingle {
                    name = frequentlyAskedQuestionState.frequentlyAskedQuestionSO.name,
                    state = frequentlyAskedQuestionState.state,
                });
            }
            foreach (ExerciseState exerciseState in codeMonkeyInteractiveSO.exerciseStateList) {
                if (exerciseState.exerciseSO == null) continue;
                saveObject.exerciseList.Add(new SaveObjectSingle {
                    name = exerciseState.exerciseSO.name,
                    state = exerciseState.state,
                });
            }

            string jsonSave = JsonUtility.ToJson(saveObject);
            PlayerPrefs.SetString("CodeMonkeyInteractiveSO", jsonSave);
            PlayerPrefs.Save();
        }

        public static bool TryLoad() {
            CodeMonkeyInteractiveSO codeMonkeyInteractiveSO = GetCodeMonkeyInteractiveSO();

            bool isSOEmpty =
                codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList.Count == 0 &&
                codeMonkeyInteractiveSO.quizStateList.Count == 0 &&
                codeMonkeyInteractiveSO.exerciseStateList.Count == 0;

            if (isSOEmpty && PlayerPrefs.HasKey("CodeMonkeyInteractiveSO")) {
                string json = PlayerPrefs.GetString("CodeMonkeyInteractiveSO");
                Debug.Log("Loading saved CodeMonkeyInteracticeSO...");

                codeMonkeyInteractiveSO.quizStateList.Clear();
                codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList.Clear();
                codeMonkeyInteractiveSO.exerciseStateList.Clear();

                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

                foreach (SaveObjectSingle quizSave in saveObject.quizList) {
                    foreach (LectureSO lectureSO in codeMonkeyInteractiveSO.lectureListSO.lectureSOList) {
                        foreach (QuizSO quizSO in lectureSO.quizListSO.quizSOList) {
                            if (quizSO.name == quizSave.name) {
                                codeMonkeyInteractiveSO.quizStateList.Add(new QuizState {
                                    quizSO = quizSO,
                                    state = quizSave.state
                                });
                            }
                        }
                    }
                }

                foreach (SaveObjectSingle faqSave in saveObject.faqList) {
                    foreach (LectureSO lectureSO in codeMonkeyInteractiveSO.lectureListSO.lectureSOList) {
                        foreach (FrequentlyAskedQuestionSO frequentlyAskedQuestionSO in lectureSO.frequentlyAskedQuestionListSO.frequentlyAskedQuestionSOList) {
                            if (frequentlyAskedQuestionSO.name == faqSave.name) {
                                codeMonkeyInteractiveSO.frequentlyAskedQuestionStateList.Add(new FrequentlyAskedQuestionState {
                                    frequentlyAskedQuestionSO = frequentlyAskedQuestionSO,
                                    state = faqSave.state
                                });
                            }
                        }
                    }
                }

                foreach (SaveObjectSingle exerciseSave in saveObject.exerciseList) {
                    foreach (LectureSO lectureSO in codeMonkeyInteractiveSO.lectureListSO.lectureSOList) {
                        foreach (ExerciseSO exerciseSO in lectureSO.exerciseListSO.exerciseSOList) {
                            if (exerciseSO.name == exerciseSave.name) {
                                codeMonkeyInteractiveSO.exerciseStateList.Add(new ExerciseState {
                                    exerciseSO = exerciseSO,
                                    state = exerciseSave.state
                                });
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

    }

}



