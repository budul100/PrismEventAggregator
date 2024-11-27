using EventAggregator.Events;
using EventAggregator.Interfaces;
using System;

namespace EventAggregatorTests.Fixtures
{
    public class EventBaseFixture
    {
        #region Public Methods

        [Fact]
        public void CanHaveMultipleSubscribersAndRaiseCustomEvent()
        {
            var customEvent = new TestableEventBase();
            var payload = new Payload();
            object[] received1 = null;
            object[] received2 = null;

            var eventSubscription1 = new MockEventSubscription
            {
                GetPublishActionReturnValue = (object[] args) => received1 = args
            };
            var eventSubscription2 = new MockEventSubscription
            {
                GetPublishActionReturnValue = (object[] args) => received2 = args
            };

            customEvent.Subscribe(eventSubscription1);
            customEvent.Subscribe(eventSubscription2);

            customEvent.Publish(payload);

            Assert.Single(received1);
            Assert.Same(received1[0], payload);

            Assert.Single(received2);
            Assert.Same(received2[0], payload);
        }

        [Fact]
        public void CanPublishSimpleEvents()
        {
            var eventBase = new TestableEventBase();
            var eventSubscription = new MockEventSubscription();
            var eventPublished = false;
            eventSubscription.GetPublishActionReturnValue = delegate
            {
                eventPublished = true;
            };
            eventBase.Subscribe(eventSubscription);

            eventBase.Publish();

            Assert.True(eventSubscription.GetPublishActionCalled);
            Assert.True(eventPublished);
        }

        [Fact]
        public void ShouldSubscribeAndUnsubscribe()
        {
            var eventBase = new TestableEventBase();

            var eventSubscription = new MockEventSubscription();
            eventBase.Subscribe(eventSubscription);

            Assert.NotNull(eventSubscription.SubscriptionToken);
            Assert.True(eventBase.Contains(eventSubscription.SubscriptionToken));

            eventBase.Unsubscribe(eventSubscription.SubscriptionToken);

            Assert.False(eventBase.Contains(eventSubscription.SubscriptionToken));
        }

        [Fact]
        public void WhenEventSubscriptionActionIsNullPruneItFromList()
        {
            var eventBase = new TestableEventBase();

            var eventSubscription = new MockEventSubscription
            {
                GetPublishActionReturnValue = null
            };

            var token = eventBase.Subscribe(eventSubscription);

            eventBase.Publish();

            Assert.False(eventBase.Contains(token));
        }

        #endregion Public Methods

        #region Private Classes

        private class MockEventSubscription : IEventSubscription
        {
            #region Public Fields

            public bool GetPublishActionCalled;
            public Action<object[]> GetPublishActionReturnValue;

            #endregion Public Fields

            #region Public Properties

            public SubscriptionToken SubscriptionToken { get; set; }

            #endregion Public Properties

            #region Public Methods

            public Action<object[]> GetExecutionStrategy()
            {
                GetPublishActionCalled = true;
                return GetPublishActionReturnValue;
            }

            #endregion Public Methods
        }

        private class Payload;

        private class TestableEventBase
            : EventBase
        {
            #region Public Methods

            public void Publish(params object[] arguments)
            {
                base.InternalPublish(arguments);
            }

            public SubscriptionToken Subscribe(IEventSubscription subscription)
            {
                return base.InternalSubscribe(subscription);
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}