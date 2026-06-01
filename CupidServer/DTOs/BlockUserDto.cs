namespace CupidServer.DTOs
{
    public class BlockUserDto
    {
        public string Username { get; set; } = string.Empty;

        public string BlockedUsername { get; set; } = string.Empty;
    }
}