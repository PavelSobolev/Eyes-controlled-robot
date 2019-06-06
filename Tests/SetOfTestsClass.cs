using System;
using Xunit;
using Xunit.Abstractions;
using Sobolev.Capstone.Connectors;
using System.Net.NetworkInformation;
using Renci;
using Renci.SshNet;
using System.Net.Sockets;
using Sobolev.Capstone.Streams;

namespace Sobolev.Capstone.EBRDriverTests
{
    #region HttpConnector Unit Tests
    public class HttpConnectorUnitTest
    {
        private readonly ITestOutputHelper Output;
        //private HttpConnector Connector;
        private Random Rnd;

        public HttpConnectorUnitTest(ITestOutputHelper output)
        {
            //Connector = HttpConnector.ConnectorObject;
            Rnd = new Random();
            Output = output;
        }

        [Fact]
        public void Intervaltest_ShouldBeLessThanMaxOrFireException()
        {
            int testValue = Rnd.Next(HttpConnector.MaxtInterval, int.MaxValue);
            Output.WriteLine($"Test value is {testValue}.");
            Assert.Throws<Exception>(() =>
            {
                HttpConnector.IntervalOfUpdate = testValue;
            });
        }

        [Fact]
        public void Intervaltest_ShouldBeGreaterThanMinOrFireException()
        {
            int testValue = Rnd.Next(0, HttpConnector.MinInterval);
            Output.WriteLine($"Test value is {testValue}.");

            Assert.Throws<Exception>(() =>
            {
                HttpConnector.IntervalOfUpdate = testValue;
            });
        }


        [Theory]
        [InlineData(5000)]
        public void Intervaltest_ShouldBetweenMinAndMax(int value)
        {

            HttpConnector.IntervalOfUpdate = value;
            Assert.True(HttpConnector.IntervalOfUpdate >= HttpConnector.MinInterval
                    & HttpConnector.IntervalOfUpdate <= HttpConnector.MaxtInterval);
        }

        [Fact]
        public void IsRaspberryAvailable_Test()
        {
            if (HttpConnector.IsRaspberryAvailable)
            {
                Assert.True(new Ping().Send(HttpConnector.RaspberryIPAdderss).Status == IPStatus.Success);
            }
            else
            {
                Assert.Equal(string.Empty, HttpConnector.RaspberryIPAdderss);
            }
        }

        [Fact]
        public async void IsRaspberryAvailable_Async_Test()
        {
            string factualIP = await HttpConnector.RaspberryIPAdderssAsync();

            if (HttpConnector.IsRaspberryAvailable)
            {
                Assert.True(new Ping().Send(factualIP).Status == IPStatus.Success);
            }
            else
            {
                Assert.Equal(string.Empty, factualIP);
            }
        }
    }
    #endregion

    #region SshConnector Unit Tests

    public class SshConnectorUnitTest
    {
        private readonly ITestOutputHelper Output;

        public SshConnectorUnitTest(ITestOutputHelper output)
        {            
            Output = output;           
        }

        [Fact]
        public void SshInitTest_ShouldReturnStatus123()
        {
            //HttpConnector cn = HttpConnector.ConnectorObject;

            if (HttpConnector.IsRaspberryAvailable)
            {
                SshConnector.Connect(HttpConnector.RaspberryIPAdderss);
                Assert.True(SshConnector.SshReceiver.IsConnected);
            }
            else
            {
                Assert.Throws<SocketException>(() => 
                    SshConnector.Connect(HttpConnector.RaspberryIPAdderss));
            }
        }
    }

    #endregion

    #region MindStream class tests

    public class MindStreamClassUnitTest
    {
        private readonly ITestOutputHelper Output;
        //private static readonly string JSONBrainData_Correct = "{\"eSense\":{\"attention\":47,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}";

        private BrainWaveEventArgs BrainDataPacket = new BrainWaveEventArgs();

        public MindStreamClassUnitTest(ITestOutputHelper output)
        {
            Output = output;
        }

        [Theory]
        [InlineData("{\"eSense\":{\"attention\":47,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        public void AttentionLevelTest(string JSONData)
        {
            BrainDataPacket.GetDataFromJSONString(JSONData);
            Assert.Equal(47, BrainDataPacket.AttentionLevel);
        }

        [Theory]
        [InlineData("{\"eSense\":{\"attention\":-47,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        [InlineData("{\"eSense\":{\"attention\":-4764565654654565465654,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        [InlineData("{\"eSense\":{\"attention\":gfgfgdfgdf,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        [InlineData("{\"eSense\":{\"attention\":,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        public void AttentionLevelCorrect(string JSONData)
        {
            BrainDataPacket.GetDataFromJSONString(JSONData);
            Assert.True(0 == BrainDataPacket.AttentionLevel);
        }

        [Theory]
        [InlineData("{\"eSense\":{\"attention\":50,\"meditation\":-51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        [InlineData("{\"eSense\":{\"attention\":50,\"meditation\":514553446656563435435435435343434},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        [InlineData("{\"eSense\":{\"attention\":50,\"meditation\":dfdsfsdfsdfsd},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        [InlineData("{\"eSense\":{\"attention\":50,\"meditation\":200},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        public void MeditationLevelCorrect(string JSONData)
        {
            BrainDataPacket.GetDataFromJSONString(JSONData);
            Assert.True(0 == BrainDataPacket.MeditationLevel);
        }

        [Theory]
        [InlineData("{\"eSense\":{\"attention\":47,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        public void MediatationLevelTest(string JSONData)
        {
            BrainDataPacket.GetDataFromJSONString(JSONData);
            Assert.Equal(51, BrainDataPacket.MeditationLevel);
        }

        [Theory]
        [InlineData("{\"eSense\":{\"attention\":47,\"meditation\":51},\"eegPower\":{\"delta\":33226,\"theta\":14816,\"lowAlpha\":368,\"highAlpha\":10967,\"lowBeta\":7298,\"highBeta\":10824,\"lowGamma\":4020,\"highGamma\":1347},\"poorSignalLevel\":56}")]
        public void PoorSignalLevelTest(string JSONData)
        {
            BrainDataPacket.GetDataFromJSONString(JSONData);
            Assert.Equal(56, BrainDataPacket.PoorSignalLevel);
        }



    }

    #endregion
}
