namespace DotNet_Shoppable.Models
{
    public class StoreSearchModel
    {
        public string? Search {  get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? Sort { get; set; }
    }
}

// Model used to sort or search for products on store page