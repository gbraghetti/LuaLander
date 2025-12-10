using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.CSharpCourse.Interactive {

    [CreateAssetMenu()]
    public class FrequentlyAskedQuestionSO : ScriptableObject {


        public string title;

        [TextArea(10, 20)]
        public string text;


    }

}