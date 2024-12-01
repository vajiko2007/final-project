using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quizzes.Models;

namespace Quizzes.services
{
    public class QuizService
    {
        private readonly List<Quiz> Quizzes;

        public QuizService(List<Quiz> quizzes)
        {
            Quizzes = quizzes;
        }

        public Quiz CreateQuiz(string title, List<Question> questions, string createdBy)
        {
            var quiz = new Quiz
            {
                Title = title,
                Questions = questions,
                CreatedBy = createdBy
            };
            Quizzes.Add(quiz);
            return quiz;
        }

        public List<Quiz> GetQuizzesByOtherUsers(string currentUser)
        {
            return Quizzes.Where(q => q.CreatedBy != currentUser).ToList();
        }

        public Quiz GetQuizByTitle(string title)
        {
            return Quizzes.FirstOrDefault(q => q.Title == title);
        }

        public bool IsOwnQuiz(string username, string quizTitle)
        {
            var quiz = Quizzes.FirstOrDefault(q => q.Title == quizTitle);
            return quiz?.CreatedBy == username;
        }

        public bool EditQuiz(string username, string quizTitle, List<Question> newQuestions)
        {
            var quiz = Quizzes.FirstOrDefault(q => q.Title == quizTitle && q.CreatedBy == username);
            if (quiz != null)
            {
                quiz.Questions = newQuestions;
                return true;
            }
            return false;
        }

        public bool DeleteQuiz(string username, string quizTitle)
        {
            var quiz = Quizzes.FirstOrDefault(q => q.Title == quizTitle && q.CreatedBy == username);
            if (quiz != null)
            {
                Quizzes.Remove(quiz);
                return true;
            }
            return false;
        }
    }
}
