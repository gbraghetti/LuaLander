using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMonkey.CSharpCourse.Interactive {

    [CreateAssetMenu()]
    public class LectureListSO : ScriptableObject {


        public List<LectureSO> lectureSOList;



        public static LectureListSO GetLectureListSO() {
            string[] lectureListSOGuidArray = AssetDatabase.FindAssets(nameof(LectureListSO));

            foreach (string lectureListSOGuid in lectureListSOGuidArray) {
                string lectureListAssetPath = AssetDatabase.GUIDToAssetPath(lectureListSOGuid);
                return AssetDatabase.LoadAssetAtPath<LectureListSO>(lectureListAssetPath);
            }

            Debug.LogError("Cannot find LectureListSO!");
            return null;
        }

    }

}