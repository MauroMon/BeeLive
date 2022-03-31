using BeeLive.Hive.Service;
using Microsoft.AspNetCore.Mvc;

namespace BeeLive.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiveController : ControllerBase
    {
        private IHiveService hiveService;

        public HiveController(IHiveService hiveService)
        {
            this.hiveService = hiveService;
        }

        [HttpGet("{id}")]
        public Task<Hive.TransferModels.HiveDto> GetHive(int id)
        {
            return hiveService.GetHive(id);
        }
    }
}
