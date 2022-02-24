#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using ChinookSystem.BLL;
using ChinookSystem.ViewModels;
#endregion

namespace WebApp.Pages.SamplePages
{
    public class AlbumsByGenreQueryModel : PageModel
    {
        #region Private Variable and DI Constructor
        private readonly ILogger<IndexModel> _logger;
        private readonly AlbumServices _albumServices;
        private readonly GenreServices _genreServices;

        public AlbumsByGenreQueryModel(ILogger<IndexModel> logger, 
                                        AlbumServices albumServices,
                                        GenreServices genreServices)
        {
            _logger = logger;
            _albumServices = albumServices;
            _genreServices = genreServices;
        }
        #endregion

        #region Feedback and ErrorHandling
        [TempData]
        public string FeedBack { get; set; }
        public bool HasFeedback => !string.IsNullOrWhiteSpace(FeedBack);

        [TempData]
        public string ErrorMsg { get; set; }
        public bool HasErrorMsg => !string.IsNullOrWhiteSpace(ErrorMsg);
        #endregion


        [BindProperty]
        public List<SelectionList> GenreList { get; set; }

        [BindProperty]
        public int GenreId { get; set; }

        public void OnGet()
        {
            GenreList = _genreServices.GetAllGenres();
            //Sort the List<T> using the method .Sort 
            GenreList.Sort((x, y) => x.DisplayText.CompareTo(y.DisplayText));

            //If I want a decending sort
            //GenreList.Sort((x, y) => y.DisplayText.CompareTo(x.DisplayText));
        }

        public IActionResult OnPost()
        {
            if(GenreId == 0)
            {
                //This is the prompt line test
                FeedBack = "You did not select a genre";
            }
            else
            {
                FeedBack = $"You select genre if od {GenreId}";
            }
            return RedirectToPage();  // This causes a Get request which forces OnGet execution 
        }
    }
}
