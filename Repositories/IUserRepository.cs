using AuthApp2.Models;

namespace AuthApp2.Repositories
{
    public interface IUserRepository
    {
        public User GetUser(string userName, string password);

        public User GetUserByUserId(string userId);
    }
}
