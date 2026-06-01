using CupidServer.Data;
using CupidServer.DTOs;
using CupidServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CupidServer.Controllers
{
    [ApiController]
    [Route("api/person")]
    public class PersonController : ControllerBase
    {
        [HttpPost("init")]
        public IActionResult InitSinglePerson([FromBody] RegisterPersonDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.City))
                return BadRequest("City is required.");

            if (dto.Age <= 0)
                return BadRequest("Age must be a positive number.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return BadRequest("Phone number is required.");

            if (string.IsNullOrWhiteSpace(dto.ConnectionId))
                return BadRequest("ConnectionId is required.");

            bool exists = InMemoryStore.Persons.Any(p =>
                p.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase));

            if (exists)
                return BadRequest("Username already exists.");

            var person = new Person
            {
                Username = dto.Username,
                City = dto.City,
                Age = dto.Age,
                PhoneNumber = dto.PhoneNumber,
                ConnectionId = dto.ConnectionId
            };

            InMemoryStore.Persons.Add(person);

            Console.WriteLine($"Registered: {person.Username}, {person.City}, {person.Age}, {person.PhoneNumber}, Conn={person.ConnectionId}");

            return Ok($"Person {person.Username} successfully registered.");
        }

        [HttpPost("confirm")]
        public IActionResult ConfirmLetter([FromBody] ConfirmLetterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest("Username is required.");

            var person = InMemoryStore.Persons.FirstOrDefault(p =>
                p.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase));

            if (person == null)
                return NotFound("Person not found.");

            person.WaitingForConfirmation = false;

            Console.WriteLine($"{person.Username} confirmed received letter.");

            return Ok("Letter confirmed. You can now receive new letters.");
        }

        [HttpPost("block")]
        public IActionResult BlockUser([FromBody] BlockUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.BlockedUsername))
                return BadRequest("Blocked username is required.");

            if (dto.Username.Equals(dto.BlockedUsername, StringComparison.OrdinalIgnoreCase))
                return BadRequest("You cannot block yourself.");

            var person = InMemoryStore.Persons.FirstOrDefault(p =>
                p.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase));

            if (person == null)
                return NotFound("Person not found.");

            var blockedPersonExists = InMemoryStore.Persons.Any(p =>
                p.Username.Equals(dto.BlockedUsername, StringComparison.OrdinalIgnoreCase));

            if (!blockedPersonExists)
                return NotFound("Blocked user not found.");

            person.BlockedUsers.Add(dto.BlockedUsername);

            Console.WriteLine($"{person.Username} blocked {dto.BlockedUsername}");

            return Ok($"User {dto.BlockedUsername} successfully blocked.");
        }
    }
}