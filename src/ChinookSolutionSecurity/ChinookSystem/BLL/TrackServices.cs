﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.Entities;
using ChinookSystem.ViewModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;
#endregion


namespace ChinookSystem.BLL
{
    public class TrackServices
    {
        #region Constructor and Context Dependency
        private readonly ChinookContext _context;

        //obtain the context link from IServiceCollection when this
        //  set of service is injected into the "outside user"
        internal TrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Query
        public List<TrackSelection> Track_FetchTracksBy(string searcharg,
                                                        string searchby,
                                                        int pagenumber,
                                                        int pagesize,
                                                        out int totalcount)
        {
            if(string.IsNullOrWhiteSpace(searcharg))
            {
                throw new ArgumentNullException("No search string has been enterd.");
            }

            IEnumerable<TrackSelection> info = _context.Tracks
                                            .Where(x => (x.Album.Title.Contains(searcharg) && searchby.Equals("Album"))
                                                     || (x.Album.Artist.Name.Contains(searcharg) && searchby.Equals("Artist")))
                                            .Select(x => new TrackSelection
                                            {
                                                TrackId = x.TrackId,
                                                SongName = x.Name,
                                                AlbumTitle = x.Album.Title,
                                                ArtistName = x.Album.Artist.Name,
                                                Milliseconds = x.Milliseconds,
                                                Price =x.UnitPrice
                                            })
                                            .OrderBy(x => x.SongName);
            totalcount = info.Count();
            int skipRows = (pagenumber - 1) * pagesize;
            return info.Skip(skipRows).Take(pagesize).ToList();

        }
        #endregion
    }
}
