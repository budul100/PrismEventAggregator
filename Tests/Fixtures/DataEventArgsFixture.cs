using System;
using EventAggregator.Events;

namespace EventAggregatorTests.Fixtures
{
    public class DataEventArgsFixture
    {
        #region Public Methods

        [Fact]
        public void CanPassData()
        {
            var e = new DataEventArgs<int>(32);
            Assert.Equal(32, e.Value);
        }

        [Fact]
        public void IsEventArgs()
        {
            var dea = new DataEventArgs<string>("");
            var ea = dea as EventArgs;
            Assert.NotNull(ea);
        }

        #endregion Public Methods
    }
}