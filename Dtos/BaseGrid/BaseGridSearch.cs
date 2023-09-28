namespace dotnet_rpg.Dtos.BaseGrid
{
    public class BaseGridSearch
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Name";
        public string FilterBy { get; set; } = "";
        public bool IsSortAsc { get; set; } = true;
    }
}
