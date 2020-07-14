using System;
using Xunit;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace tests
{
    using System.Collections.Generic;
    using PactNet.Mocks.MockHttpService.Models;

    // This class is responsible for setting up a shared
    // mock server for Pact used by all the tests.
    // XUnit can use a Class Fixture for this.
    // See: https://goo.gl/hSq4nv
    public class ConsumerPactClassFixture : IDisposable
    {
        public IPactBuilder PactBuilder { get; private set; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort { get { return 9222; } }
        public string MockProviderServiceBaseUri { get { return String.Format("http://localhost:{0}", MockServerPort); } }

        public ConsumerPactClassFixture()
        {
            // Using Spec version 2.0.0 more details at https://goo.gl/UrBSRc
            var pactConfig = new PactConfig
            {
                SpecificationVersion = "2.0.0",
                PactDir = @"..\..\..\..\..\pacts",
                LogDir = @".\pact_logs"
            };

            PactBuilder = new PactBuilder(pactConfig);

            PactBuilder.ServiceConsumer("Consumer")
                .HasPactWith("Provider");

            MockProviderService = PactBuilder.MockService(MockServerPort);
            MockProviderService.ClearInteractions(); //NOTE: Clears any previously registered interactions before the test is run
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // This will save the pact file once finished.
                    PactBuilder.Build();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        public void ArrangeWithQueryInvalidDatetime(string expectedBodyMessage)
        {
            MockProviderService.Given("There is data")
                .UponReceiving("A invalid GET request for Date Validation with invalid date parameter")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/provider",
                    Query = $"validDateTime=lolz"
                })
                .WillRespondWith(new ProviderServiceResponse {
                    Status = 400,
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new
                    {
                        message = expectedBodyMessage
                    }
                });
        }

        public void ArrangeWithQueryValidDatetime(string validDateTimeValue, object expectedBodyMessage)
        {
            MockProviderService.Given("There is data")
                .UponReceiving("A valid GET request for Date Validation")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/provider",
                    Query = $"validDateTime={validDateTimeValue}"
                })
                .WillRespondWith(new ProviderServiceResponse {
                    Status = 200,
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = expectedBodyMessage
                });
        }

    }
}