using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMonkey.CSharpCourse.Companion {

    [CreateAssetMenu()]
    public class CodeMonkeyCompanionSO : ScriptableObject {



        private CodeMonkeyCompanion.OnCompanionMessageEventArgs lastCompanionMessageEventArgs;




        public static CodeMonkeyCompanionSO GetCodeMonkeyCompanionSO() {
            string[] codeMonkeyCompanionSOGuidArray = AssetDatabase.FindAssets(nameof(CodeMonkeyCompanionSO));

            foreach (string codeMonkeyCompanionSOGuid in codeMonkeyCompanionSOGuidArray) {
                string codeMonkeyCompanionSOPath = AssetDatabase.GUIDToAssetPath(codeMonkeyCompanionSOGuid);
                return AssetDatabase.LoadAssetAtPath<CodeMonkeyCompanionSO>(codeMonkeyCompanionSOPath);
            }

            Debug.LogError("Cannot find CodeMonkeyCompanionSO!");
            return null;
        }

        public static void ClearLastCompanionMessageEventArgs() {
            CodeMonkeyCompanionSO codeMonkeyCompanionSO = GetCodeMonkeyCompanionSO();
            codeMonkeyCompanionSO.lastCompanionMessageEventArgs = null;
        }

        public static void SetLastCompanionMessageEventArgs(CodeMonkeyCompanion.OnCompanionMessageEventArgs onCompanionMessageEventArgs) {
            CodeMonkeyCompanionSO codeMonkeyCompanionSO = GetCodeMonkeyCompanionSO();
            codeMonkeyCompanionSO.lastCompanionMessageEventArgs = onCompanionMessageEventArgs;
        }

        public static CodeMonkeyCompanion.OnCompanionMessageEventArgs GetLastCompanionMessageEventArgs() {
            CodeMonkeyCompanionSO codeMonkeyCompanionSO = GetCodeMonkeyCompanionSO();
            return codeMonkeyCompanionSO.lastCompanionMessageEventArgs;
        }


    }

}