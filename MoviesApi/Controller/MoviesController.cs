using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.models;

namespace MoviesApi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
      
    {
        private new List<string> _allowedFiles = new List<string> {".jpg", ".png", ".jpeg" };
        private readonly ApplicationDbContext _context;
        private long _maxSize = 1048576;
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies
                .OrderByDescending(x => x.Rate) 
                .Include(g => g.Genre)
                .Select(m => new movieDetailsDtos
                {
                    id = m.Id,  
                    Rate = m.Rate,
                    GenreName = m.Genre.Name,
                    StoryPoster = m.StoryPoster,
                    StoryLine = m.StoryLine,    
                    GenreId = m.GenreId,
                    Title = m.Title,
                    Year = m.Year,
                })
                .ToListAsync();
            return Ok(movies);
        }

        [HttpGet(template: "{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id);  
            if (movie == null)  return NotFound();
            var dto = new movieDetailsDtos
            {
                id = movie.Id,
                Rate = movie.Rate,
                GenreName = movie.Genre.Name,
                StoryPoster = movie.StoryPoster,
                StoryLine = movie.StoryLine,
                GenreId = movie.GenreId,
                Title = movie.Title,
                Year = movie.Year,

            };
            return Ok(movie);
        }

        [HttpGet(template: "GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _context.Movies
                .Where(m => m.GenreId == genreId)   
              .OrderByDescending(x => x.Rate)
              .Include(g => g.Genre)
              .Select(m => new movieDetailsDtos
              {
                  id = m.Id,
                  Rate = m.Rate,
                  GenreName = m.Genre.Name,
                  StoryPoster = m.StoryPoster,
                  StoryLine = m.StoryLine,
                  GenreId = m.GenreId,
                  Title = m.Title,
                  Year = m.Year,
              })
              .ToListAsync();
            return Ok(movies);

        }

        [HttpPost]
        public async Task<IActionResult> CreateMovieAsync([FromForm] MovieDtos dto)
        {
            if(dto.StoryPoster == null)  return BadRequest("poster is required") ;
            if (!_allowedFiles.Contains(Path.GetExtension(dto.StoryPoster.FileName).ToLower() )){
                return BadRequest(error: "only jpg and png are allowed");
            }
            if(dto.StoryPoster.Length > _maxSize)  return BadRequest(error: "max size is one GB"); 
            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre) return BadRequest(error: "invalid genre id");
            using var dataStream = new MemoryStream();
            await dto.StoryPoster.CopyToAsync(dataStream);
                var movie = new Movie
                {
                    GenreId = dto.GenreId,
                    Title = dto.Title,
                    Rate = dto.Rate,
                    StoryPoster = dataStream.ToArray(),
                    StoryLine = dto.StoryLine,
                    Year = dto.Year,
                };
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return Ok(movie);   
        }

        [HttpPut (template:"{id}")]
        public async Task<IActionResult> UpdateMovieAsync(int id, [FromForm] MovieDtos dto)
        {
            var movie = await _context.Movies.FindAsync(id);
            var isValidGenre = await _context.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!isValidGenre) return BadRequest(error: "invalid genre id");
            if (movie == null) return BadRequest("no movie with this id");
           if(dto.StoryPoster != null)
            {
                if (!_allowedFiles.Contains(Path.GetExtension(dto.StoryPoster.FileName).ToLower()))
                {
                    return BadRequest(error: "only jpg and png are allowed");
                }
                if (dto.StoryPoster.Length > _maxSize) return BadRequest(error: "max size is one GB");
                using var dataStream = new MemoryStream();
                await dto.StoryPoster.CopyToAsync(dataStream);
                movie.StoryPoster = dataStream.ToArray();
            }
            movie.Title = dto.Title;
            movie.Rate = dto.Rate;
            movie.StoryLine = dto.StoryLine;
            movie.Year = dto.Year;
            movie.GenreId = dto.GenreId;
            _context.SaveChanges();
            return Ok(movie);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound("not found ");
            _context.Remove(movie);
            _context.SaveChanges();
            return Ok(movie);

        }
    }
}
