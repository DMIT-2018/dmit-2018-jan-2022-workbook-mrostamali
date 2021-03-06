#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.SamplePages
{
    public class BasicsModel : PageModel
    {
        //basicly this is an  object, treat it as such

        //data fields
        public string MyName;

        //properties

        //The annotation [TempData] stores data until it's read in another immediate request
        //This annotation attribute has two method called keep(string) and Peek(string) (used on content page)
        //This information is keep int a dictionary (name/value pair)
        //Useful to redirect when data is required for more than a single request
        //Implemented by TempData providers using either cookies or session state
        //TempData is NOT bound to any particular control like BindProperty
        [TempData]
        public string FeedBack { get; set; }

        //The annotation BindProperty ties a property in the PageModel class directly to a control on the Content Page
        //Data is transferred between the two automatically
        //On the Content page, the control to use this property will have a helper-tag called asp-for 

        // to retain a value in a control tied to this property AND retained via the @page use the supportGet attribute = true
        [BindProperty(SupportsGet = true)]
        public int? id { get; set; }

        //constructors

        //behaviours (aka methods)
        public void OnGet()
        {
            //executes in response to a Get Request from the browser
            //when the page is "first" accessed, the browser issues a Get request
            //when the page is refreshed, WITHOUT a Post request, the browser issues a Get request
            //when the page is retrieved in response to a form's POST using RedirectToPage()
            //IF NOT RedirectToPage() is used on the POST, there is NO Get requested issued
       
            //Server-side processing
            //contains no html

            Random rnd = new Random();
            int oddeven = rnd.Next(0,25);
            if(oddeven % 2 == 0)
            {
                MyName = $"Don is even {oddeven}";
            }
            else
            {
                MyName = null;
            }
        }

        //processing in response to a request from a form on a web page
        //this request is referred to as a Post (method="post")

        //General Post
        //A general post occurs when a asp-page-handler is NOT used
        //the return datatype can be void, however, you will normally encounter the datatype IActionResult
        //The IActionResult requires some type of request action on the return statement of the method OnPost()
        //Typical actions:
        //  Page()
        //      :does NOT issue a OnGet request
        //      :remains on the current page
        //      :a good action for form processing involving validation and with the catch of aa try/catch
        //  RedirectToPage()
        //      :Does issue an OnGet request
        //      :is used to retaining input values via the @page and your BindProperty form controls on your form on the content Page

        public IActionResult OnPost()
        {
            //This line of code is used to cause a delay in processing so we can
            //  see on the Network Activity some type of simulated processing
            Thread.Sleep(2000);  // It will be 2 seconds, because it is on milliseconds

            //Retreive data via the Request object
            //Request: web page to server
            //Response: server to web page
            string buttonvalue = Request.Form["theButton"];
            // If we want to pass data to other pages we can use Request.String
            FeedBack = $"Button press is {buttonvalue} with numeric input of {id}";
            //return Page(); //Does not issue an OnGet() 
            return RedirectToPage(new { id = id }); //Request for OnGet()
        }
    }
}
