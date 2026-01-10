namespace AuthApp2.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string DeviceId { get; set; } = default!;
    }
}
