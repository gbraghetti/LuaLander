using CodeMonkey.CSharpCourse.Interactive;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public static class ExerciseUtils {


    public const string SUCCESS = "<color=#00ff00>SUCCESS!</color>";
    public const string INCORRECT = "<color=#aa1111>Incorrect!</color>";
    public static readonly Color COLOR_WARNING = new Color(1f, .7f, .15f);



    public static void TimedMessage(TextMeshProUGUI textMeshUI, string message, ref float currentTimer, float currentTimerIncrease = .9f, bool appendNewline = true, bool add = true, Color? color = null) {
        TimedMessage(textMeshUI, message, currentTimer, appendNewline, add, color);
        currentTimer += currentTimerIncrease;
    }
    
    public static void TimedMessage(TextMeshProUGUI textMeshUI, string message, float timer, bool appendNewline = true, bool add = true, Color? color = null) {
        if (color != null) {
            message = $"<color=#{ColorUtility.ToHtmlStringRGB(color.Value)}>{message}</color>";
        }
        if (appendNewline) {
            message += "\n";
        }
        FunctionTimer.Create(() => {
            if (add) {
                textMeshUI.text += message;
            } else {
                textMeshUI.text = message;
            }
            Debug.Log(message.Replace("\n", ""));
        }, timer);
    }

    public static bool TryGetLectureExerciseCSText(string lectureCode, out string lectureText) {
        LectureSO lectureSO = LectureSO.GetLectureSO(lectureCode);
        string exerciseFilename = lectureSO.GetLectureFolderPath() + "Exercises/Exercise.cs";

        if (File.Exists(exerciseFilename)) {
            lectureText = File.ReadAllText(exerciseFilename);
            return true;
        } else {
            // Does not exist
            lectureText = null;
            return false;
        }
    }


}