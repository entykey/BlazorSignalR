namespace BlazorSignalR.Shared
{
    public class Book
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Isbn { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public double Price { get; set; }
    }
}
