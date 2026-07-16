namespace BookstoreCatalog.Mvc.ViewModels
{
    public class BookEditViewModel : BookCreateViewModel
    {
        public int Id { get; set; }
        public string? RowVersion { get; set; } = string.Empty; 
        public string? ImagePath { get; set; }
    }
}