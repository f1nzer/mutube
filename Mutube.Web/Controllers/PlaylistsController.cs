using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Mutube.Database;
using Mutube.Database.Models;
using Mutube.Web.ModelsDto;

namespace Mutube.Web.Controllers
{
    [Route("api/playlists")]
    [ApiController]
    public class PlaylistsController : BaseApiController
    {
        public PlaylistsController(MutubeDbContext dbContext) 
            : base(dbContext) { }

        [HttpGet]
        public ActionResult<Playlist> GetById(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var playlist = _dbContext.Playlists.FirstOrDefault(x => x.Id.Equals(id));

            return JsonResult(playlist);
        }

        [HttpGet]
        public ActionResult<IReadOnlyCollection<Playlist>> Find(string serchQuery)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult<Playlist> Create(PlaylistInputDto playlistInput)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult<Playlist> Edit(PlaylistInputDto playlistInput)
        {
            throw new NotImplementedException();
        }


        [HttpDelete]
        public ActionResult<Playlist> Delete(string id)
        {
            throw new NotImplementedException();
        }


        [HttpPost]
        public ActionResult<Playlist> SaveReorder()
        {
            throw new NotImplementedException();
        }


        [HttpPost]
        public ActionResult<Playlist> AddTrackToPlaylist(string playlistId, string trackUrl)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public ActionResult<Playlist> DeleteTrackFromPlaylist(string playlistId, string trackUrl)
        {
            throw new NotImplementedException();
        }
    }
}
