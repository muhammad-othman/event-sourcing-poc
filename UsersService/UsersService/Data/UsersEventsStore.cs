using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService.Data
{
    public class UsersEventsStore : IUsersEventsStore
    {
        private readonly IMongoCollection<UserEvent> usersEventsCollection;

        private static List<User> usersList = new List<User>();
        public IEnumerable<User> Users { get; private set; }

        public UsersEventsStore()
        {
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            var db = dbClient.GetDatabase("eda");

            usersEventsCollection = db.GetCollection<UserEvent>("usersevents");
            Users = usersList;
        }

        public User AddUser(User newUser)
        {
            newUser.Id = Guid.NewGuid();

            var userEvent = new UserEvent
            {
                Id = Guid.NewGuid(),
                Type = EventType.Create,
                UserData = newUser
            };

            SaveAndProcessEvent(userEvent);

            return newUser;
        }

        public User UpdateUser(User updatedUser)
        {

            var userEvent = new UserEvent
            {
                Id = Guid.NewGuid(),
                Type = EventType.Update,
                UserData = updatedUser
            };

            SaveAndProcessEvent(userEvent);

            return updatedUser;
        }

        public void DeleteUser(Guid id)
        {
            var deletedUser = usersList.First(e => e.Id == id);
            var userEvent = new UserEvent
            {
                Id = Guid.NewGuid(),
                Type = EventType.Delete,
                UserData = deletedUser
            };

            SaveAndProcessEvent(userEvent);
        }

        private void SaveAndProcessEvent(UserEvent userEvent)
        {
            usersEventsCollection.InsertOne(userEvent);

            ProcessEvent(userEvent);
        }

        private void ProcessEvent(UserEvent userEvent)
        {
            switch (userEvent.Type)
            {
                case EventType.Create:
                    usersList.Add(userEvent.UserData);
                    break;
                case EventType.Update:
                    var oldUser = usersList.First(e => userEvent.UserData.Id == e.Id);

                    oldUser.FirstName = userEvent.UserData.FirstName;
                    oldUser.LastName = userEvent.UserData.LastName;
                    oldUser.Email = userEvent.UserData.Email;
                    oldUser.Age = userEvent.UserData.Age;
                    oldUser.CreditScore = userEvent.UserData.CreditScore;

                    break;
                case EventType.Delete:
                    usersList.RemoveAll(e => userEvent.UserData.Id == e.Id);
                    break;
            }
        }

        public async Task ReprocessEventsAsync(int limit = 0)
        {
            usersList = new List<User>();

            var collectionList = usersEventsCollection.Find(_ => true).SortBy(e => e.Created);

            if (limit != 0)
                collectionList.Limit(limit);

            var events = await collectionList.ToListAsync();

            foreach (var @event in events)
            {
                ProcessEvent(@event);
            }
        }
    }
}
