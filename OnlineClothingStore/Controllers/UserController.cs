using Microsoft.AspNetCore.Mvc;
using OnlineClothingStore.DTOs;
using OnlineClothingStore.Models;

namespace OnlineClothingStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static List<User> Users = new List<User> 
        {
            new User { Id = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", PasswordHash = "hashed_pw_1", Role = "Customer" },
            new User { Id = 2, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com", PasswordHash = "hashed_pw_2", Role = "Admin" },
            new User { Id = 3, FirstName = "Charlie", LastName = "Lee", Email = "charlie@example.com", PasswordHash = "hashed_pw_3", Role = "Customer" },
        };

        [HttpPost]
        public ActionResult RegisterUser(AddUserDTO userDTO)
        {
            var user = new User
            {
                Id = Users.Max(u => u.Id) + 1,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Email = userDTO.Email,
                PasswordHash = userDTO.Password,
                Role = "Customer"
            };

            Users.Add(user);

            var cart = new Cart
            {
                Id = CartController.Carts.Max(c => c.Id) + 1,
                UserId = user.Id,
                Items = new List<CartItem>()
            };

            CartController.Carts.Add(cart);

            return Ok();
        }
    }
}
