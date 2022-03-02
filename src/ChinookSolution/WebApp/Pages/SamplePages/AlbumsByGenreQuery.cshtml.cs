#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using ChinookSystem.BLL;
using ChinookSystem.ViewModels;
using WebApp.Helpers;
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

        [BindProperty(SupportsGet = true)]
        public int? GenreId { get; set; }

        [BindProperty]
        public List<AlbumsListBy> AlbumsByGenre { get; set; }

        #region Paginator Variables
        //My desired Page Size
        private const int PAGE_SIZE = 5;

        //Instance for the paginator class
        public Paginator Pager { get; set; }
        #endregion


        //CurrentPage value will appear on your url as a Request parameter value 
        //    url address..?currentPage=n
        public void OnGet(int? currentPage)
        {
            //OnGet is executed as the page first is processed (as it comes up)

            //Consume a service: GetAllGenres in register services of _genreServices
            GenreList = _genreServices.GetAllGenres();
            //Sort the List<T> using the method .Sort 
            GenreList.Sort((x, y) => x.DisplayText.CompareTo(y.DisplayText));

            //If I want a decending sort
            //GenreList.Sort((x, y) => y.DisplayText.CompareTo(x.DisplayText));

            //remember that this method executes as the page FIRST comes up BEFORE
            //   anything has happende on the page (including the FIRST display)
            //any code in this method MUST handle the possibility of missing data for the query argument

            if (GenreId.HasValue && GenreId.Value >0)
            {
                //Installation of the paginator setup 
                //First: Determine the page number to use with the paginator
                int pageNumber = currentPage.HasValue ? currentPage.Value : 1;

                //Second: Use the page state to setup data needed for paging
                PageState current = new PageState(pageNumber, PAGE_SIZE);

                //Third: Total rows in the complete query collection (Data needed for paging)
                int totalrows = 0;

                //For efficiency of data being transferred, we will pass the current page number
                //    and the desired page size to the backend query
                //The returned collection (AlbumsByGenre) will ONLY have the rows of the whole query
                //   collection that will actually be shown (PAGE_SIZE or less rows)
                //The total number of records for the whole query collcetion will be returned as 
                //   an out parameter. This value is needed by the Paginator to set up its display logic.
                AlbumsByGenre = _albumServices.AlbumsByGenre((int)GenreId, pageNumber, PAGE_SIZE, out totalrows);

                //Once the query is complete, use the returned total rows in instanciating 
                //  an instance of the Paginator 
                Pager = new Paginator(totalrows, current);
            }
            
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
                FeedBack = $"You select genre id of {GenreId}";
            }
            return RedirectToPage(new {GenreId = GenreId});  // This causes a Get request which forces OnGet execution 
        }

        public IActionResult OnPostNew()
        {
            return RedirectToPage("/SamplePages/CRUDAlbum");
        }
    }
}
