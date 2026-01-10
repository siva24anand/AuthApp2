namespace AuthApp2.Models
{
    public class RefreshToken
    {
        public string TokenHash { get; set; } = default!;
        public string UserId { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }

        
        public string? ReplacedByToken { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
