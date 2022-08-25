namespace netApi.Models
{
    public class Office
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public int MaxOccupancy { get; set; }
    }
}