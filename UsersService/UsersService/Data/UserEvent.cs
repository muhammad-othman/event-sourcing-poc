using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService.Data
{
    public class UserEvent
    {
        [BsonId]
        public Guid Id { get; set; }
        public EventType Type { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public User UserData { get; set; }
    }

    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public double CreditScore { get; set; }
        public string Email { get; set; }
    }

    public enum EventType
    {
        Create,
        Update,
        Delete
    }
}
