namespace MoviesApi.models
{
    public class Movie
    {
        public int Id { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }
        public string Year { get; set; }
        public double Rate { get; set; }

        [MaxLength(2500)]
        public string StoryLine { get; set; }

        public byte[] StoryPoster { get; set; }

        public byte GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
