#nullable disable
using System;
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
    public class PlaylistTrackServices
    {

        #region Constructor and Context Dependecy
        private readonly ChinookContext _context;

        //Obtain the context link from IServiceCollection when this 
        //  set of services is injected into the "outside user"
        internal PlaylistTrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public List<PlaylistTrackInfo> PlaylistTrack_GetUserPlaylistTracks (string playlistname,
                                                                            string username)
        {
            IEnumerable<PlaylistTrackInfo> Info = _context.PlaylistTracks
                                                  .Where(x => x.Playlist.Name.Equals(playlistname)
                                                            && x.Playlist.UserName.Equals(username))
                                                  .Select(x => new PlaylistTrackInfo
                                                  {
                                                      TrackId = x.TrackId,
                                                      TrackNumber = x.TrackNumber,
                                                      SongName = x.Track.Name,
                                                      Milliseconds = x.Track.Milliseconds
                                                  })
                                                  .OrderBy(x => x.TrackNumber);
            return Info.ToList();
        }
        #endregion

        #region Commands
        public void PlaylistTrack_AddTrack(string playlistname, string username, int trackid)
        {
            //Create local variables
            Track trackExists = null;
            Playlist playlistExists = null;
            PlaylistTrack playlisttrackExists = null;
            int tracknumber = 0;

            //Create a List<Exception> to contain all discovered errors
            // You can also call it brokenrules
            List<Exception> errorlist = new List<Exception> ();

            //Business Logic
            //These are processing riles that need to be satisfied for valid data
            // rule: a track can only exist once on a playlist
            // rule: each track on a playlist is assigned a continuous track number 
            //
            // If the business rules are passed, consider the data valid, then 
            // a) Stage your trasaction work (Adds, Updates, Deletes)
            // b) Execute a SINGLE .SaveChanges() - commits to database 

            //Parameter validation
            if(string.IsNullOrWhiteSpace(playlistname))
            {
                throw new ArgumentNullException("Playlist name is missing");
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("User name is missing");
            }

            trackExists = _context.Tracks
                          .Where(x => x.TrackId == trackid)
                          .FirstOrDefault();
            if (trackExists == null)
            {
                errorlist.Add(new Exception("Selected Track no longer is on file. Refresh Track table"));
            }

            //Business process 
            playlistExists = _context.Playlists
                            .Where(x => x.Name.Equals(playlistname)
                                    && x.UserName.Equals(username))
                            .FirstOrDefault();

            if(playlistExists == null)
            {
                // new playlist

                playlistExists = new Playlist()
                {
                    Name = playlistname,
                    UserName = username
                };
                //Stage (Only in memory) until we do a save changes
                _context.Playlists.Add(playlistExists);
                tracknumber = 1;
            }
            else
            {
                //playlist already exists

                //Rule: unique tracks on playlist
                playlisttrackExists = _context.PlaylistTracks
                            .Where(x => x.Playlist.Name.Equals(playlistname)
                                    && x.Playlist.UserName.Equals(username)
                                    && x.TrackId == trackid)
                            .FirstOrDefault();
                if(playlisttrackExists != null)
                {
                    var songname = _context.Tracks
                                    .Where(x => x.TrackId == trackid)
                                    .Select(x => x.Name)
                                    .SingleOrDefault();
                    //Rule Violation
                    errorlist.Add(new Exception($"Selected Track {songname} is already on the playlist."));
                }
                else
                {
                    tracknumber = _context.PlaylistTracks
                            .Where(x => x.Playlist.Name.Equals(playlistname)
                                    && x.Playlist.UserName.Equals(username))
                            .Count();
                    tracknumber++;
                }
            }

            //Add the track to the playlist 
            //Create an instance for the playlist track
            playlisttrackExists = new PlaylistTrack();

            //Load values
            playlisttrackExists.TrackId = trackid;
            playlisttrackExists.TrackNumber = tracknumber;

            //?? What about the second part of the primary key: PlayListID?
            //If the playlist exists then we know the id: playlistExists.PlaylistID
            //BUT if the playlist is NEW, we DO NOT know the id

            //In the situation of a NEW playlist, even though we have
            //   created the playlist instance (see above) it is ONLY
            //   Staged!!!!
            //This means that the actual sql record has NOT yet been created
            //This means that the IDENTITY value for the new playlist DOES NOT
            //  yet exist. The value on the playlist instance (playlistExists)
            //  is zero (0).
            //Therefore we have a serious problem

            //Solution
            //It is built into the EntityFramework software and is based using the 
            //  navigational property in PlayList pointing to it's "child"

            //Staging a typical Add in the past was to reference the entity
            //  and use the entity.Add(xxxx)
            //    _context.PlaylistTrack.Add(playlisttrackExists)
            //IF you use this statement the playlistid would be zero (0)
            //   causing your transaction to ABORT
            //Why?? pKeys cannot be zero(0) (FKey to PKey problem)

            //INSTEAD, do the staging using the "parent.navchildproperty.Add(xxxx)
            playlistExists.PlaylistTracks.Add(playlisttrackExists);

            //Staging is complete
            //Commit the work (Transaction)
            //Commiting the work needs a .SaveChanges()
            //a transaction has ONLY ONE .SaveChanges()
            //BUT what if you have discovered errors during the business process???
            //   If so, then throw all errors and DO NOT COMMIT!!!!
            if(errorlist.Count > 0)
            {
                //Throw the list of business processing error(s)
                throw new AggregateException("Unable to add new track. Check concerns:", errorlist);
            }
            else
            {
                //Consider data valid
                //Has passed business processing rules
                _context.SaveChanges();
            }
        }
        #endregion
    }
}
