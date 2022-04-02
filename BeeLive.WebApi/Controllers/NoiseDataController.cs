using BeeLive.NoiseData.Service;
using BeeLive.NoiseData.TransferModels;
using Microsoft.AspNetCore.Mvc;

namespace BeeLive.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoiseDataController : ControllerBase
    {
        private INoiseDataService noiseDataService;

        public NoiseDataController(INoiseDataService noiseDataService)
        {
            this.noiseDataService = noiseDataService;
        }

        [HttpPost]
        public async Task AddAsync([FromBody] NoiseDataDto noiseData)
        {
            await noiseDataService.InsertNoiseDataAsync(noiseData);
        }
    }
}
