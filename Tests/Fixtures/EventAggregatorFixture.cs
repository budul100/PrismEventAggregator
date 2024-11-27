using EventAggregator.Events;

namespace EventAggregatorTests.Fixtures
{
    public class EventAggregatorFixture
    {
        #region Public Methods

        [Fact]
        public void GetReturnsSingleInstancesOfSameEventType()
        {
            var eventAggregator = new EventAggregator.EventAggregator();
            var instance1 = eventAggregator.GetEvent<MockEventBase>();
            var instance2 = eventAggregator.GetEvent<MockEventBase>();

            Assert.Same(instance2, instance1);
        }

        #endregion Public Methods

        #region Internal Classes

        internal class MockEventBase
            : EventBase;

        #endregion Internal Classes
    }
}