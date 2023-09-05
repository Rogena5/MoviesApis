namespace MoviesApi.Dtos
{
    public class movieDetailsDtos
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Year { get; set; }

        public double Rate { get; set; }

        public string StoryLine { get; set; }

        public byte[] StoryPoster { get; set; }

        public byte GenreId { get; set; }

        public string GenreName { get; set; }
    }
}
