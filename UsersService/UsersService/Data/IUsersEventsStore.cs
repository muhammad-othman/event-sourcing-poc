using System;
using System.Collections.Generic;

namespace UsersService.Data
{
    public interface IUsersEventsStore
    {
        IEnumerable<User> Users { get; }
        User AddUser(User newUser);
        void DeleteUser(Guid id);
        User UpdateUser(User updatedUser);
    }
}