namespace BookstoreCatalog.Mvc.ViewModels
{
    public class BookEditViewModel : BookCreateViewModel
    {
        public int Id { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string? RowVersion { get; set; } = string.Empty; 
    }
}