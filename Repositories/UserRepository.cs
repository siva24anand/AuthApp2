using AuthApp2.Models;

namespace AuthApp2.Repositories
{
    public class UserRepository : IUserRepository
    {
        public List<User> _users = new List<User>
        {
            new User
            {
                UserName = "User123",
                PasswordHash = "Password123!",
                UserId = "User123_Id",
                Role = "Admin",
                Email = "johm@email.com",
                PreferredName="John"
            }
        };

        public User GetUser(string userName, string password)
        {
            return _users.FirstOrDefault(u => u.UserName == userName && u.PasswordHash == password)!;
        }

        public User GetUserByUserId(string userId)
        {
            return _users.FirstOrDefault(u => u.UserId == userId);
        }

    }
}
