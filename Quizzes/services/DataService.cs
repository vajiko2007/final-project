using Newtonsoft.Json;
using static Quizzes.Models;
using Formatting = Newtonsoft.Json.Formatting;


namespace Quizzes.services
{
    public class DataService
    {
        private string UsersFilePath = @"C:\Users\user\Desktop\shualeduri\Quizzes\Quizzes\data\UserData.json";
        private string QuizzesFilePath = @"C:\Users\user\Desktop\shualeduri\Quizzes\Quizzes\data\QuizData.json";
        private string ResultsFilePath = @"C:\Users\user\Desktop\shualeduri\Quizzes\Quizzes\data\ResultData.json";

        public List<User> LoadUsers()
        {
            return LoadFromFile<List<User>>(UsersFilePath);
        }

        public void SaveUsers(List<User> users)
        {
            SaveToFile(UsersFilePath, users);
        }

        public List<Quiz> LoadQuizzes()
        {
            return LoadFromFile<List<Quiz>>(QuizzesFilePath);
        }

        public void SaveQuizzes(List<Quiz> quizzes)
        {
            SaveToFile(QuizzesFilePath, quizzes);
        }

        public List<QuizResult> LoadQuizResults()
        {
            return LoadFromFile<List<QuizResult>>(ResultsFilePath);
        }

        public void SaveQuizResults(List<QuizResult> quizResults)
        {
            SaveToFile(ResultsFilePath, quizResults);
        }

        private T LoadFromFile<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            return default;
        }

        private void SaveToFile<T>(string filePath, T data)
        {
            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }
    }
}
