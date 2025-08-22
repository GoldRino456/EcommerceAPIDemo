namespace EcommerceAPIDemo.Data
{
    public class GameCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GameProduct> GamesInCategory { get; } = [];
    }
}
