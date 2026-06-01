using CupidServer.Data;
using CupidServer.Models;
using System.Security.Cryptography;

namespace CupidServer.Services
{
    public class CupidService
    {
        private readonly string[] _messages =
        {
            "Radujem se našem susretu!",
            "Želim da se upoznamo.",
            "Nisam zainteresovan/a za upoznavanje."
        };

        public Person? FindBestMatch(Person receiver)
        {
            Person? bestCandidate = null;
            int bestScore = -1;

            foreach (Person candidate in InMemoryStore.Persons)
            {
                if (candidate.Username.Equals(receiver.Username, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (receiver.BlockedUsers.Contains(candidate.Username, StringComparer.OrdinalIgnoreCase))
                    continue;

                int score = CalculateScore(receiver, candidate);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCandidate = candidate;
                }
            }

            return bestCandidate;
        }

        public LoveLetter CreateLoveLetter(Person sender)
        {
            string message = GetRandomMessage();

            return new LoveLetter
            {
                SenderUsername = sender.Username,
                SenderCity = sender.City,
                SenderAge = sender.Age,
                SenderPhoneNumber = message == "Nisam zainteresovan/a za upoznavanje."
                    ? string.Empty
                    : sender.PhoneNumber,
                Message = message
            };
        }

        private int CalculateScore(Person receiver, Person candidate)
        {
            int score = 0;

            if (receiver.City.Equals(candidate.City, StringComparison.OrdinalIgnoreCase))
                score += 30;

            if (Math.Abs(receiver.Age - candidate.Age) <= 2)
                score += 20;

            score += RandomNumberGenerator.GetInt32(0, 101);

            return score;
        }

        private string GetRandomMessage()
        {
            //RNGCryptoServiceProvider ovo je samo modernije
            int index = RandomNumberGenerator.GetInt32(0, _messages.Length);
            return _messages[index];
        }
    }
}