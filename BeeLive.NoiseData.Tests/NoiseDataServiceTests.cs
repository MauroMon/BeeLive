using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Core.Repositories;
using BeeLive.NoiseData.Core.Settings;
using BeeLive.NoiseData.Service;
using BeeLive.NoiseData.TransferModels;
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
        private IOptions<NoiseDataSettings> settings;
        private Mock<INoiseDataRepository> repository;

        public NoiseDataServiceTests()
        {
            repository = new Mock<INoiseDataRepository>();
            repository.Setup(n => n.AddAsync(It.IsAny<Core.Entities.NoiseData>())).Returns(Task.CompletedTask);
            settings = Options.Create(new NoiseDataSettings());
            service = new NoiseDataService(repository.Object, settings, NullLogger<INoiseDataService>.Instance);
        }

        [Fact]
        public async void NegativeDecibelNotAdmitted()
        {
            await service.Invoking(async y => await y.InsertNoiseDataAsync(new TransferModels.NoiseDataDto() { Decibel = -1, HiveId = 1 })).Should().ThrowAsync<ArgumentOutOfRangeException>();
        }

        [Fact]
        public async void NotEnoughValuesSetWarningToFalse()
        {
            int hiveId = 1;
            repository.Setup(r => r.GetAverage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), hiveId)).ReturnsAsync(new Core.Entities.NoiseDataAvg() { Count = 1, Average = 0 });
            settings.Value.MinRequiredValues = 2;
            NoiseDataDto noiseDataDto = new NoiseDataDto()
            {
                HiveId = hiveId,
                Decibel = 10
            };
            await service.InsertNoiseDataAsync(noiseDataDto);
            //check that noise is inserted with warning = false
            repository.Verify(r => r.AddAsync(It.Is<Core.Entities.NoiseData>(n => n.Warning == false)));
        }

        [Theory]
        [InlineData(10, false)]
        [InlineData(35, false)]
        [InlineData(36, false)]
        [InlineData(40, true)]
        public async Task CalculateWarningNoiseAsync(decimal decibel, bool expctedValue)
        {
            int hiveId = 1;
            repository.Setup(r => r.GetAverage(It.IsAny<DateTime>(), It.IsAny<DateTime>(), hiveId)).ReturnsAsync(new Core.Entities.NoiseDataAvg() { Count = 10, Average = 30 });
            settings.Value.MinRequiredValues = 2;
            settings.Value.WarningNoiseIncreasePercentage = 20;
            NoiseDataDto noiseDataDto = new NoiseDataDto()
            {
                HiveId = hiveId,
                Decibel = decibel
            };
            //note that 30+ 20% = 36 
            await service.InsertNoiseDataAsync(noiseDataDto);
            //check that noise is inserted with warning = false
            repository.Verify(r => r.AddAsync(It.Is<Core.Entities.NoiseData>(n => n.Warning == expctedValue)));
        }
    }
}