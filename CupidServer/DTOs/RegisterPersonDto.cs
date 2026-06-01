namespace CupidServer.DTOs
{
    public class RegisterPersonDto
    {
        public string Username { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public int Age { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string ConnectionId { get; set; } = string.Empty;
    }
}