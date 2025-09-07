namespace Library_Management.ModelViews
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int CopiesAvailable { get; set; }
        public string? ImageUrl { get; set; }

        public int SellerId { get; set; } // Optional: Include SellerId if needed
    }
}
