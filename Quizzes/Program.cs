using Quizzes.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quizzes.Models;
using static Quizzes.services.QuizService;
using static Quizzes.services.QuizResultService;

namespace Quizes
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var dataService = new DataService();
                var users = dataService.LoadUsers() ?? new List<User>();
                var quizzes = dataService.LoadQuizzes() ?? new List<Quiz>();
                var quizResults = dataService.LoadQuizResults() ?? new List<QuizResult>();

                var userService = new UserService(users);
                var quizService = new QuizService(quizzes);
                var quizResultService = new QuizResultService(quizResults, dataService);

                Console.WriteLine("Welcome to the Quiz App!");
                string username = null;
                User user = null;

                while (user == null)
                {
                    Console.WriteLine("Enter your username to login or register:");
                    username = Console.ReadLine();

                    try
                    {
                        user = userService.GetUserByUsername(username);

                        if (user == null)
                        {
                            Console.WriteLine("Username does not exist. Registering a new user.");
                            user = userService.RegisterUser(username);
                            dataService.SaveUsers(users);
                            Console.WriteLine($"User '{username}' registered successfully!");
                        }
                        else
                        {
                            Console.WriteLine($"Welcome back, {username}!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during user registration or login: {ex.Message}");
                    }
                }

                while (true)
                {
                    try
                    {
                        Console.WriteLine("Choose an option:");
                        Console.WriteLine("1. View Top 10 Users");
                        Console.WriteLine("2. Create a Quiz");
                        Console.WriteLine("3. Solve a Quiz");
                        Console.WriteLine("4. View Your Quizzes(edit/deleat)");
                        Console.WriteLine("5. Log Out");
                        Console.WriteLine("6. Exit");

                        var option = Console.ReadLine();

                        if (option == "1")
                        {
                            var topUsers = userService.GetTop10Users();
                            foreach (var u in topUsers)
                            {
                                Console.WriteLine($"{u.UserName} - {u.TotalScore} points");
                            }
                        }
                        else if (option == "2")
                        {
                            try
                            {
                                Console.WriteLine("Enter the title of your quiz:");
                                string quizTitle = Console.ReadLine();
                                List<Question> questions = new List<Question>();

                                Console.WriteLine("You need to create exactly 5 questions for your quiz.");

                                for (int i = 1; i <= 5; i++)
                                {
                                    Console.WriteLine($"Enter question {i}:");
                                    string questionText = Console.ReadLine();

                                    var question = new Question { Text = questionText, Answers = new List<Answer>() };

                                    Console.WriteLine("Enter 4 answers for this question:");

                                    for (int j = 1; j <= 4; j++)
                                    {
                                        Console.WriteLine($"Enter answer {j}:");
                                        string answerText = Console.ReadLine();
                                        question.Answers.Add(new Answer { Text = answerText });
                                    }

                                    Console.WriteLine("Enter the index of the correct answer (1-4):");
                                    int correctAnswerIndex = int.Parse(Console.ReadLine()) - 1;

                                    question.CorrectAnswerIndex = correctAnswerIndex;

                                    questions.Add(question);
                                }

                                quizService.CreateQuiz(quizTitle, questions, user.UserName);
                                dataService.SaveQuizzes(quizzes);
                                Console.WriteLine("Quiz created successfully!");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while creating the quiz: {ex.Message}");
                            }
                        }
                        else if (option == "3")
                        {
                            try
                            {
                                Console.WriteLine("Quizzes created by other users:");
                                var otherUserQuizzes = quizService.GetQuizzesByOtherUsers(user.UserName);

                                if (!otherUserQuizzes.Any())
                                {
                                    Console.WriteLine("There are no quizzes created by other users yet.");
                                }
                                else
                                {
                                    for (int i = 0; i < otherUserQuizzes.Count; i++)
                                    {
                                        Console.WriteLine($"{i + 1}. {otherUserQuizzes[i].Title}");
                                    }

                                    Console.WriteLine("Choose a quiz to solve (enter the quiz number, or 0 to go back):");
                                    int quizChoice = int.Parse(Console.ReadLine());

                                    if (quizChoice == 0)
                                    {
                                        continue;
                                    }

                                    if (quizChoice > 0 && quizChoice <= otherUserQuizzes.Count)
                                    {
                                        var selectedQuiz = otherUserQuizzes[quizChoice - 1];
                                        Console.WriteLine($"You selected the quiz: {selectedQuiz.Title}");

                                        DateTime startTime = DateTime.Now;
                                        int score = 0;

                                        foreach (var question in selectedQuiz.Questions)
                                        {
                                            TimeSpan timeElapsed = DateTime.Now - startTime;
                                            if (timeElapsed.TotalMinutes > 2)
                                            {
                                                Console.WriteLine("You have exceeded the time limit of 2 minutes! You will not receive any points.");
                                                score = 0;
                                                break;
                                            }

                                            Console.WriteLine(question.Text);
                                            for (int i = 0; i < question.Answers.Count; i++)
                                            {
                                                Console.WriteLine($"{i + 1}. {question.Answers[i].Text}");
                                            }

                                            Console.WriteLine("Enter the number of your answer:");
                                            int answerIndex = int.Parse(Console.ReadLine()) - 1;

                                            if (answerIndex == question.CorrectAnswerIndex)
                                            {
                                                score += 20;
                                                Console.WriteLine("Correct answer! +20 points");
                                            }
                                            else
                                            {
                                                score -= 20;
                                                Console.WriteLine("Incorrect answer! -20 points");
                                            }
                                        }

                                        if (score != 0)
                                        {
                                            quizResultService.RecordResult(user, selectedQuiz.Title, score);
                                            Console.WriteLine($"Your score: {score}/100");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid choice, going back to main menu.");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while solving the quiz: {ex.Message}");
                            }
                        }
                        else if (option == "4")
                        {
                            try
                            {
                                Console.WriteLine("Your created quizzes:");
                                var createdQuizzes = quizzes.Where(q => q.CreatedBy == user.UserName).ToList();

                                if (createdQuizzes.Count == 0)
                                {
                                    Console.WriteLine("You haven't created any quizzes yet.");
                                }
                                else
                                {
                                    for (int i = 0; i < createdQuizzes.Count; i++)
                                    {
                                        Console.WriteLine($"{i + 1}. {createdQuizzes[i].Title}");
                                    }

                                    Console.WriteLine("Choose a quiz to edit or delete (enter the quiz number, or 0 to go back):");
                                    int quizChoice = int.Parse(Console.ReadLine());

                                    if (quizChoice == 0)
                                    {
                                        continue;
                                    }

                                    if (quizChoice > 0 && quizChoice <= createdQuizzes.Count)
                                    {
                                        var selectedQuiz = createdQuizzes[quizChoice - 1];
                                        Console.WriteLine($"You selected the quiz: {selectedQuiz.Title}");
                                        Console.WriteLine("What would you like to do?");
                                        Console.WriteLine("1. Edit this quiz");
                                        Console.WriteLine("2. Delete this quiz");
                                        Console.WriteLine("3. Go back to the main menu");

                                        int actionChoice = int.Parse(Console.ReadLine());

                                        if (actionChoice == 1)
                                        {
                                            try
                                            {
                                                Console.WriteLine("You are now editing the quiz.");
                                                List<Question> newQuestions = new List<Question>();

                                                for (int i = 1; i <= selectedQuiz.Questions.Count; i++)
                                                {
                                                    var question = selectedQuiz.Questions[i - 1];
                                                    Console.WriteLine($"Editing question {i}: {question.Text}");
                                                    question.Text = Console.ReadLine();

                                                    for (int j = 0; j < question.Answers.Count; j++)
                                                    {
                                                        Console.WriteLine($"Current answer {j + 1}: {question.Answers[j].Text}");
                                                        Console.WriteLine("Enter new answer (leave blank to keep current answer):");
                                                        string newAnswer = Console.ReadLine();
                                                        if (!string.IsNullOrEmpty(newAnswer))
                                                        {
                                                            question.Answers[j].Text = newAnswer;
                                                        }
                                                    }

                                                    Console.WriteLine("Enter the index of the correct answer (1-4):");
                                                    int correctAnswerIndex = int.Parse(Console.ReadLine()) - 1;
                                                    question.CorrectAnswerIndex = correctAnswerIndex;

                                                    newQuestions.Add(question);
                                                }

                                                bool editSuccess = quizService.EditQuiz(user.UserName, selectedQuiz.Title, newQuestions);
                                                if (editSuccess)
                                                {
                                                    dataService.SaveQuizzes(quizzes);
                                                    Console.WriteLine("Quiz edited successfully!");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Failed to edit the quiz.");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine($"Error while editing the quiz: {ex.Message}");
                                            }
                                        }
                                        else if (actionChoice == 2)
                                        {
                                            try
                                            {
                                                bool deleteSuccess = quizService.DeleteQuiz(user.UserName, selectedQuiz.Title);
                                                if (deleteSuccess)
                                                {
                                                    dataService.SaveQuizzes(quizzes);
                                                    Console.WriteLine("Quiz deleted successfully!");
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Failed to delete the quiz.");
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine($"Error while deleting the quiz: {ex.Message}");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while viewing your quizzes: {ex.Message}");
                            }
                        }
                        else if (option == "5") 
                        {
                            Console.WriteLine("You have been logged out.");
                            user = null;

                            while (user == null)
                            {
                                Console.WriteLine("Enter your username to login or register:");
                                username = Console.ReadLine();

                                try
                                {
                                    user = userService.GetUserByUsername(username);

                                    if (user == null)
                                    {
                                        Console.WriteLine("Username does not exist. Registering a new user.");
                                        user = userService.RegisterUser(username);
                                        dataService.SaveUsers(users);
                                        Console.WriteLine($"User '{username}' registered successfully!");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Welcome back, {username}!");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error during user registration or login: {ex.Message}");
                                }
                            }
                        }
                        else if (option == "6")
                        {
                            Console.WriteLine("Exiting the application. Goodbye!");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid option. Please choose again.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected error occurred: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while running the application: {ex.Message}");
            }
        }
    }
}
