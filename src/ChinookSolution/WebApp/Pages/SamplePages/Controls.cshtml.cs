#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespace
using ChinookSystem.ViewModels;
#endregion

namespace WebApp.Pages.SamplePages
{
    public class ControlsModel : PageModel
    {
        [TempData]
        public string Feedback { get; set; }

        [BindProperty]
        public string EmailText { get; set; }

        [BindProperty]
        public string PasswordText { get; set; }

        [BindProperty]
        // public DateTime DateText { get; set; } //= DateTime.Today;
        // If we dont want to have a predefined value we can juts use string instead of DateTime
        public string DateText { get; set; }


        [BindProperty]
        //public TimeSpan TimeText { get; set; } //= DateTime.Now.TimeOfDay;
        // If we dont want to have a predefined value we can juts use string instead of TimeSpan
        public string TimeText { get; set; }

        [BindProperty]
        public string Meal { get; set; }

        //assume this array is actually data retreive from the database
        public string[] Meals { get; set; } = new[] { "breakfast", "lunch", "dinner", "snacks" };

        [BindProperty]
        public bool AcceptanceBox { get; set; }

        [BindProperty]
        public string MessageBody { get; set; }

        [BindProperty]
        public int MyRide { get; set; }
        //Pretend that the following collection is data from a database
        //The collection is based on a 2 property calss called SelectionList
        //The data for the list will be created in a seperate method

        public List<SelectionList> Rides { get; set; }

        [BindProperty]
        public string VacationSpot { get; set; }

        public List<string> VacationSpots { get; set; }

        [BindProperty]
        public int ReviewRating { get; set; }

        public void OnGet()
        {
            PopulateList();
        }

        public void PopulateList()
        {
            //Create a pretend collection from the database representing different types of transportation (rides)
            Rides = new List<SelectionList>();
            Rides.Add(new SelectionList() { ValueId = 1, DisplayText = "Car" });
            Rides.Add(new SelectionList() { ValueId = 2, DisplayText = "Bus" });
            Rides.Add(new SelectionList() { ValueId = 3, DisplayText = "Bike" });
            Rides.Add(new SelectionList() { ValueId = 4, DisplayText = "Motorcycle" });
            Rides.Add(new SelectionList() { ValueId = 5, DisplayText = "Board" });
            Rides.Sort((x,y) => x.DisplayText.CompareTo(y.DisplayText));

            VacationSpots = new List<string>();
            VacationSpots.Add("California");
            VacationSpots.Add("Caribbean");
            VacationSpots.Add("Cruising");
            VacationSpots.Add("Europe");
            VacationSpots.Add("Florida");
            VacationSpots.Add("Mexico");
        }

        public IActionResult OnPostTextBox()
        {
            Feedback = $"Email: {EmailText}; Password: {PasswordText}; Date: {DateText}; Time: {TimeText}";
            return Page();
        }

        public IActionResult OnPostRadioCheckArea()
        {
            Feedback = $"Meal: {Meal}; Acceptance: {AcceptanceBox}; Message: {MessageBody};";
            return Page();
        }

        public IActionResult OnPostListSlider()
        {
            Feedback = $"Ride: {MyRide}; Vacation: {VacationSpot}; Review Rating: {ReviewRating};";
            PopulateList();
            return Page();
        }
    }


    //Insted of this class we use (using ChinookSystem.ViewModels;) namespace
    //public class SelectionList
    //{
    //    public int ValueId { get; set; }
    //    public string DisplayText { get; set; }
    //}
}
