#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using ChinookSystem.BLL;
using ChinookSystem.ViewModels;
#endregion

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        #region Private Variable and DI Constructor
        private readonly ILogger<IndexModel> _logger;
        private readonly AboutServices _aboutServices;

        public IndexModel(ILogger<IndexModel> logger, AboutServices aboutServices)
        {
            _logger = logger;
            _aboutServices = aboutServices;
        }
        #endregion

        #region Feedback and ErrorHandling
        //[TempData]
        // If we make it a TempData, it will be shown on every page because it will be stored on the RAM
        public string FeedBack { get; set; }
        public bool HasFeedback => !string.IsNullOrWhiteSpace(FeedBack);
        #endregion

        public void OnGet()
        {
            //Consume a service
            DbVersionInfo info = _aboutServices.GetDbVersion();
            if (info == null)
            {
                FeedBack = "Version unknown";
            }
            else
            {
                FeedBack = $"Version: {info.Major}.{info.Minor}.{info.Build}" +
                            $"Release date of {info.ReleaseDate.ToShortDateString()}";
            }
        }
    }
}