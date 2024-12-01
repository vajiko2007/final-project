using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quizzes.Models;

namespace Quizzes.services
{
    public class UserService
    {
        private readonly List<User> Users;

        public UserService(List<User> users)
        {
            Users = users;
        }

        public User RegisterUser(string username)
        {
            var user = new User { UserName = username };
            Users.Add(user);
            return user;
        }

        public User GetUserByUsername(string username)
        {
            return Users.FirstOrDefault(u => u.UserName == username);
        }

        public List<User> GetTop10Users()
        {
            return Users.OrderByDescending(u => u.TotalScore).Take(10).ToList();
        }
    }
}
