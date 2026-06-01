namespace CupidServer.Models
{
    public class LoveLetter
    {
        public string SenderUsername { get; set; } = string.Empty;

        public string SenderCity { get; set; } = string.Empty;

        public int SenderAge { get; set; }

        public string SenderPhoneNumber { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}