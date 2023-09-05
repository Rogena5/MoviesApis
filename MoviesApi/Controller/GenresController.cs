using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MoviesApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetALLasync()
        {
            var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return Ok(genres);
        }
        [HttpPost]
        public async Task<IActionResult> AddAllAsync(GenreDtos dto)
        {
            var genre = new Genre { Name = dto.Name };
            await _context.Genres.AddAsync(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpPut(template: "{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] GenreDtos dtos)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return NotFound($"No genre was found with ID: {id}");

            genre.Name = dtos.Name;
          
            return Ok(genre);
        }
        [HttpDelete(template: "{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _context.Genres.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return NotFound($"No genre was found with ID: {id}");

            _context.Remove(genre);
          
            return Ok(genre);  
        }
        
    }

}



