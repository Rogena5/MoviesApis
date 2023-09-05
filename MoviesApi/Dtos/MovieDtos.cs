namespace MoviesApi.Dtos
{
    public class MovieDtos
    {
        [MaxLength(250)]

        public string Title { get; set; }

        public string Year { get; set; }

        public double Rate { get; set; }

        [MaxLength(2500)]
        public string StoryLine { get; set; }


        public IFormFile? StoryPoster { get; set; }

        public byte GenreId { get; set; }
    }
}
