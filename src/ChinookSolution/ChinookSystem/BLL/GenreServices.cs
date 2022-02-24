using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.Entities;
using ChinookSystem.ViewModels;
#endregion

namespace ChinookSystem.BLL
{
    public class GenreServices
    {
        #region Constructor and Context Dependecy
        private readonly ChinookContext _context;

        //Obtain the context link from IServiceCollection when this 
        //  set of services is injected into the "outside user"
        internal GenreServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Services: Queries
        //obtain a list of Genres to be used in a select list 
        public List<SelectionList> GetAllGenres()
        {
            // We use IEnumerable here because we know that everything that is going to come back 
            //   from our database will be either IEnumerable or IQuerable. Then we change it to
            //   the .ToList() on our return statement.
            // Note: SelectionList is our class in the ViewModels folder, which is a container for 
            //       passing data to the public 
            IEnumerable<SelectionList> info = _context.Genres
                                                .Select(g => new SelectionList
                                                {
                                                    ValueId = g.GenreId,
                                                    DisplayText = g.Name
                                                });
                                              //.OrderBy(g => g.DisplayText);  //This sort is in Sql
            return info.ToList();
            // We can also do our sort on our return statements like:
            //return info.OrderBy(g => g.DisplayText).ToList();   // This sort is in RAM
        }
        #endregion
    }
}
