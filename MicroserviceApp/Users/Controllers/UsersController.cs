using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPIApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        UserContext userContext;
        ProductContextCopy productContextCopy;

        public UsersController(UserContext userContext, ProductContextCopy productContextCopy)
        {
            this.userContext = userContext;
            this.productContextCopy = productContextCopy;
            if (!userContext.Users.Any())
            {
                userContext.Users.Add(new User { Name = "Tom" });
                userContext.Users.Add(new User { Name = "Alice" });
                userContext.SaveChanges();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await userContext.Users.ToListAsync();
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            User user = await userContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            //var selectedProducts = productContextCopy.Products.Where(p => p.UserId == id);
            var selectedProducts = productContextCopy.Products.Where(p => p.UserId == id).Select(p => p.Name);
            GetUserModel model = new GetUserModel { currentUser = user, currentUserProducts = selectedProducts };

            string json = JsonConvert.SerializeObject(model);
            if (user == null)
                return NotFound();
            return new ObjectResult(model);
        }

        // POST api/users
        [HttpPost]
        public async Task<ActionResult<User>> Post(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            userContext.Users.Add(user);
            await userContext.SaveChangesAsync();
            return Ok(user);
        }

        // PUT api/users/
        [HttpPut]
        public async Task<ActionResult<User>> Put(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (!userContext.Users.Any(x => x.Id == user.Id))
            {
                return NotFound();
            }

            userContext.Update(user);
            await userContext.SaveChangesAsync();
            return Ok(user);
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            User user = userContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            userContext.Users.Remove(user);
            await userContext.SaveChangesAsync();
            return Ok(user);
        }
    }
}