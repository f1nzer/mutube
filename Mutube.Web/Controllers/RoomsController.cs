using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Mutube.Database;
using Mutube.Database.Models;

namespace Mutube.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly MutubeDbContext _dbContext;

        public RoomsController(MutubeDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            return null;
        }
    }
}