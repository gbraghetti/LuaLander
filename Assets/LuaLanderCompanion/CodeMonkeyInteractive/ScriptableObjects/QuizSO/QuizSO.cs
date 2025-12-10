using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.CSharpCourse.Interactive {

    [CreateAssetMenu()]
    public class QuizSO : ScriptableObject {


        [TextArea(5, 10)]
        public string question;
        public List<string> optionList;
        public int correctOptionIndex;

        [TextArea(20, 40)]
        public string answer;


    }

}