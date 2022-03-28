using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Service;
using FluentAssertions;
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
            noiseDataRepository.Setup(n => n.AddAsync(It.IsAny<BeeLive.Core.Entities.NoiseData>())).Returns(Task.CompletedTask);
            service = new NoiseDataService(noiseDataRepository.Object);
        }

        [Fact]
        public async void NegativeDecibelNotAdmitted()
        {
          await  service.Invoking(async y => await y.InsertNoiseData(new TransferModels.NoiseDataDto() { Decibel = -1, HiveId = 1 })).Should().ThrowAsync<ArgumentOutOfRangeException>();
        }
    }
}