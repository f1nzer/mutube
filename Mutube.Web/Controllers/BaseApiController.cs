using Microsoft.AspNetCore.Mvc;
using Mutube.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Mutube.Web.Controllers
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly MutubeDbContext _dbContext;
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public BaseApiController(MutubeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected ActionResult JsonResult(object value)
        {
            return new JsonResult(value, _jsonSerializerSettings);
        }
    }
}