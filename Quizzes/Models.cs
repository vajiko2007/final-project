using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quizzes
{
    public class Models
    {
        public class User
        {
            public string UserName { get; set; }
            public int TotalScore { get; set; }
            public List<Quiz> QuizzesCreated { get; set; } = new List<Quiz>();
            public List<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
        }

        public class Quiz
        {
            public string Title { get; set; }
            public List<Question> Questions { get; set; } = new List<Question>();
            public string CreatedBy { get; set; }
        }

        public class Question
        {
            public string Text { get; set; }
            public List<Answer> Answers { get; set; }
            public int CorrectAnswerIndex { get; set; }
        }

        public class Answer
        {
            public string Text { get; set; }
        }

        public class QuizResult
        {
            public string QuizTitle { get; set; }
            public int Score { get; set; }
            public DateTime DateTaken { get; set; }
            public string UserName { get; set; }
        }
    }
}
