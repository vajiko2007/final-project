using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quizzes.Models;

namespace Quizzes.services
{
    public class QuizResultService
    {
        private readonly DataService _dataService;
        private readonly List<QuizResult> _quizResults;

        public QuizResultService(List<QuizResult> quizResults, DataService dataService)
        {
            _quizResults = quizResults;
            _dataService = dataService;
        }
        public void RecordResult(User user, string quizTitle, int score)
        {
            var result = new QuizResult
            {
                QuizTitle = quizTitle,
                Score = score,
                DateTaken = DateTime.Now,
                UserName = user.UserName 
            };

            _quizResults.Add(result);
            _dataService.SaveQuizResults(_quizResults);

            user.TotalScore += score;

            var allUsers = _dataService.LoadUsers();

            var existingUser = allUsers.FirstOrDefault(u => u.UserName == user.UserName);

            if (existingUser != null)
            {
                existingUser.TotalScore = user.TotalScore;
            }
            else
            {
                allUsers.Add(user);
            }

            _dataService.SaveUsers(allUsers);

            Console.WriteLine($"User '{user.UserName}' total score updated to {user.TotalScore}");
        }

        public List<QuizResult> GetUserResults(User user)
        {
            return _quizResults.Where(r => r.UserName == user.UserName).ToList();
        }
    }
}