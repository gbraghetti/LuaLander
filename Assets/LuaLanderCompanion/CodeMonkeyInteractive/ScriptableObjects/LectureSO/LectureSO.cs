using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.CSharpCourse.Interactive {

    [CreateAssetMenu()]
    public class LectureSO : ScriptableObject {


        public int lectureCode;
        public int lectureSectionNumber;
        public string lectureName;
        public string lectureTitle;
        public FrequentlyAskedQuestionListSO frequentlyAskedQuestionListSO;
        public QuizListSO quizListSO;
        public ExerciseListSO exerciseListSO;

        [TextArea(10, 20)]
        public string lectureDescription;



        public enum Section {
            Beginner,
            Intermediate,
            Advanced
        }

        public Section GetLectureSection() {
            switch (lectureCode.ToString()[0]) {
                default:
                case '1':
                    return Section.Beginner;
                case '2':
                    return Section.Intermediate;
                case '3':
                    return Section.Advanced;
            }
        }

        public string GetLectureFolderPath() {
            return Application.dataPath + $"/Lectures/{lectureCode}_{lectureName}/";
        }


        public class LectureStats {
            public int faqDone;
            public int faqTotal;
            public int quizDone;
            public int quizTotal;
            public int exercisesDone;
            public int exercisesTotal;
        }

        public LectureStats GetLectureStats() {
            GetLectureStats(out int faqDone,
                out int faqTotal,
                out int quizDone,
                out int quizTotal,
                out int exercisesDone,
                out int exercisesTotal);

            LectureStats lectureStats = new LectureStats();
            lectureStats.faqDone = faqDone;
            lectureStats.faqTotal = faqTotal;
            lectureStats.quizDone = quizDone;
            lectureStats.quizTotal = quizTotal;
            lectureStats.exercisesDone = exercisesDone;
            lectureStats.exercisesTotal = exercisesTotal;
            return lectureStats;
        }

        public void GetLectureStats(out int faqDone,
            out int faqTotal,
            out int quizDone,
            out int quizTotal,
            out int exercisesDone,
            out int exercisesTotal) 
        {

            faqDone = 0;
            faqTotal = frequentlyAskedQuestionListSO.frequentlyAskedQuestionSOList.Count;
            foreach (FrequentlyAskedQuestionSO frequentlyAskedQuestionSO in frequentlyAskedQuestionListSO.frequentlyAskedQuestionSOList) {
                if (CodeMonkeyInteractiveSO.GetState(frequentlyAskedQuestionSO) == CodeMonkeyInteractiveSO.State.Completed) {
                    faqDone++;
                }
            }

            quizDone = 0;
            quizTotal = quizListSO.quizSOList.Count;
            foreach (QuizSO quizSO in quizListSO.quizSOList) {
                if (CodeMonkeyInteractiveSO.GetState(quizSO) == CodeMonkeyInteractiveSO.State.Completed) {
                    quizDone++;
                }
            }

            exercisesDone = 0;
            exercisesTotal = 0;
            /*
            exercisesTotal = exerciseListSO.exerciseSOList.Count;
            foreach (ExerciseSO exerciseSO in exerciseListSO.exerciseSOList) {
                if (CodeMonkeyInteractiveSO.GetState(exerciseSO) == CodeMonkeyInteractiveSO.State.Completed) {
                    exercisesDone++;
                }
            }
            */
        }



        public static LectureSO GetLectureSO(string partialLectureSOName) {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();

            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                if (lectureSO.name.Contains(partialLectureSOName)) {
                    return lectureSO;
                }
            }

            return null;
        }

        public static LectureSO GetLectureSO(FrequentlyAskedQuestionSO forFrequentlyAskedQuestionSO) {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();

            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                foreach (FrequentlyAskedQuestionSO frequentlyAskedQuestionSO in lectureSO.frequentlyAskedQuestionListSO.frequentlyAskedQuestionSOList) {
                    if (frequentlyAskedQuestionSO == forFrequentlyAskedQuestionSO) {
                        return lectureSO;
                    }
                }
            }

            return null;
        }

        public static LectureSO GetLectureSO(QuizSO forQuizSO) {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();

            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                foreach (QuizSO quizSO in lectureSO.quizListSO.quizSOList) {
                    if (quizSO == forQuizSO) {
                        return lectureSO;
                    }
                }
            }

            return null;
        }

        public static LectureSO GetLectureSO(ExerciseSO forExerciseSO) {
            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();

            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                foreach (ExerciseSO exerciseSO in lectureSO.exerciseListSO.exerciseSOList) {
                    if (exerciseSO == forExerciseSO) {
                        return lectureSO;
                    }
                }
            }

            return null;
        }

        public static List<LectureSO> GetLectureList(Section section) {
            List<LectureSO> lectureSOList = new List<LectureSO>();

            LectureListSO lectureListSO = LectureListSO.GetLectureListSO();
            foreach (LectureSO lectureSO in lectureListSO.lectureSOList) {
                if (lectureSO.GetLectureSection() == section) {
                    lectureSOList.Add(lectureSO);
                }
            }

            return lectureSOList;
        }

    }

}