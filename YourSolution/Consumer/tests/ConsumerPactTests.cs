using System;
using Xunit;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Consumer;
using System.Collections.Generic;

namespace tests
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class ConsumerPactTests : IClassFixture<ConsumerPactClassFixture>
    {
        private IMockProviderService _mockProviderService;
        private string _mockProviderServiceBaseUri;
        private ConsumerPactClassFixture fixture;

        public ConsumerPactTests(ConsumerPactClassFixture fixture)
        {
            this.fixture = fixture;
            _mockProviderService = fixture.MockProviderService;

            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public void ItHandlesInvalidDateParam()
        {
            // Arange
            var invalidRequestMessage = "validDateTime is not a date or time";
            fixture.ArrangeWithQueryInvalidDatetime(invalidRequestMessage);

            // Act
            var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("lolz", _mockProviderServiceBaseUri).GetAwaiter().GetResult();
            var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            // Assert
            _mockProviderService.VerifyInteractions();
            Assert.Contains(invalidRequestMessage, resultBodyText);
        }

        [Fact]
        public void ItHandlesValidDateParam()
        {
            // Arange
            var validDateTime = "04/04/2018";
            var expectedResultBody = new
            {
                test = "NO",
                validDateTime = $"04-04-2018 00:00:00"
            };
            var expectedBodyMessage = JsonConvert.SerializeObject(expectedResultBody);
            fixture.ArrangeWithQueryValidDatetime(validDateTime, expectedResultBody);

            // Act
            var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi(validDateTime, _mockProviderServiceBaseUri).GetAwaiter().GetResult();
            var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            // Assert
            _mockProviderService.VerifyInteractions();
            Assert.Contains(expectedBodyMessage, resultBodyText);
        }
    }
}