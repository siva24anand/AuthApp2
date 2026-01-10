namespace AuthApp2.Models
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = default!;
        public string DeviceId { get; set; } = default!;
    }
}
