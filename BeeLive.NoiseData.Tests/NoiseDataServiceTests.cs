using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Core.Repositories;
using BeeLive.NoiseData.Core.Settings;
using BeeLive.NoiseData.Service;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BeeLive.NoiseData.Tests
{
    public class NoiseDataServiceTests
    {
        private INoiseDataService service;
        public NoiseDataServiceTests()
        {
            var noiseDataRepository = new Mock<INoiseDataRepository>();
            noiseDataRepository.Setup(n => n.AddAsync(It.IsAny<Core.Entities.NoiseData>())).Returns(Task.CompletedTask);
            var noiseDataSettings = new Mock<IOptions<NoiseDataSettings>>();
            service = new NoiseDataService(noiseDataRepository.Object, noiseDataSettings.Object, NullLogger<INoiseDataService>.Instance);
        }

        [Fact]
        public async void NegativeDecibelNotAdmitted()
        {
            await service.Invoking(async y => await y.InsertNoiseData(new TransferModels.NoiseDataDto() { Decibel = -1, HiveId = 1 })).Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}