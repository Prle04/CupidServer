using CupidServer.Models;

namespace CupidServer.Data
{
    public static class InMemoryStore
    {
        public static List<Person> Persons { get; set; } = new();
    }
}