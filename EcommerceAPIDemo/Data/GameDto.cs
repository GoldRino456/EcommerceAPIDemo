namespace EcommerceAPIDemo.Data
{
    public class GameDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<GameCategory> Categories { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public DateTime ReleaseDate { get; set; }
        public double Price { get; set; }
        public double FileSize { get; set; }
        public string SystemRequirements { get; set; }
    }
}
