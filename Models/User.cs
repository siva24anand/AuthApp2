namespace AuthApp2.Models
{
    public class User
    {
        public string UserName { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PreferredName { get; set; } = default!;

    }
}
