namespace dotnet_rpg.Dtos.BaseGrid
{
    public class BaseGridResponse<T>
    {
        public int TotalCount { get; set; }
        public List<T> Items { get; set; } = new List<T> { };
    }
}
