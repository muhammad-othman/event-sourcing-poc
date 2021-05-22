using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersService.Data;

namespace UsersService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersEventsStore usersEventsStore;

        public UsersController(IUsersEventsStore usersEventsStore)
        {
            this.usersEventsStore = usersEventsStore;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(usersEventsStore.Users);
        }

        [HttpPost]
        public IActionResult Post(User user)
        {
            var addedUser = usersEventsStore.AddUser(user);
            return Ok(addedUser);
        }

        [HttpPut]
        public IActionResult Put(User user)
        {
            var updatedUser = usersEventsStore.UpdateUser(user);
            return Ok(updatedUser);
        }


        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            usersEventsStore.DeleteUser(id);
            return Ok();
        }
    }
}
