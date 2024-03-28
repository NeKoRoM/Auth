using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<UserEntity> _userManager;


        public ProductsController(DataContext context, UserManager<UserEntity> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: api/Products
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var products = await _context.Products.Where(p => p.UserId == userId).ToListAsync();

            return products;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Перевіряємо, чи продукт належить поточному користувачу
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (product.UserId != userId)
            {
                return Forbid(userId); // Повертаємо 403 Forbidden, якщо продукт не належить користувачу
            }

            return product;
        }

        // POST: api/Products
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // Отримати токен JWT з заголовка запиту
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(); // Повернути 401 Unauthorized, якщо користувач не аутентифікований
            }
            

            // Встановити власника продукту на поточного користувача
            product.UserId = userId;

            // Зберегти продукт в базі даних
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }


        // PUT: api/Products/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (product.UserId != userId)
            {
                return Forbid(); 
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (product.UserId != userId)
            {
                return Forbid(); 
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
