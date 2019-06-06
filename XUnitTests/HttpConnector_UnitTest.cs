using System;
using Xunit;
using Xunit.Abstractions;
using Connectors;

namespace XUnitTests
{    
    public class HttpConnectorUnitTest
    {
        private readonly ITestOutputHelper Output;
        private HttpConnector Connector = new HttpConnector();

        public HttpConnectorUnitTest(ITestOutputHelper output)
        {
            Output = output;            
        }

        [Theory]
        [InlineData(500)]
        [InlineData(21000)]
        [InlineData(1999)]
        public void Intervaltest_ShouldBetweenMinAndMaxOrFireException(int value)
        {
            

            Assert.Throws<Exception>(() =>
            {
                Connector.IntervalOfUpdate = value;
                //Assert.True(Connector.IntervalOfUpdate >= Connector.MinInterval
                //    & Connector.IntervalOfUpdate <= Connector.MaxtInterval);
            });
        }
    }
}
