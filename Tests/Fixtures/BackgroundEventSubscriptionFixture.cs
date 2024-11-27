using System;
using System.Threading;
using EventAggregator.Interfaces;
using EventAggregator.Subscriptions;

namespace EventAggregatorTests.Fixtures
{
    public class BackgroundEventSubscriptionFixture
    {
        #region Public Methods

        [Fact]
        public void ShouldReceiveDelegateOnDifferentThread()
        {
            var completeEvent = new ManualResetEvent(false);
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            SynchronizationContext calledSyncContext = null;
            Action<object> action = delegate
            {
                calledSyncContext = SynchronizationContext.Current;
                completeEvent.Set();
            };

            IDelegateReference actionDelegateReference = new MockDelegateReference() { Target = action };
            IDelegateReference filterDelegateReference = new MockDelegateReference() { Target = (Predicate<object>)delegate { return true; } };

            var eventSubscription = new BackgroundEventSubscription<object>(actionDelegateReference, filterDelegateReference);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            publishAction.Invoke(null);

            completeEvent.WaitOne(5000);

            Assert.NotEqual(SynchronizationContext.Current, calledSyncContext);
        }

        [Fact]
        public void ShouldReceiveDelegateOnDifferentThreadNonGeneric()
        {
            var completeEvent = new ManualResetEvent(false);
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            SynchronizationContext calledSyncContext = null;
            Action action = delegate
            {
                calledSyncContext = SynchronizationContext.Current;
                completeEvent.Set();
            };

            IDelegateReference actionDelegateReference = new MockDelegateReference() { Target = action };

            var eventSubscription = new BackgroundEventSubscription(actionDelegateReference);

            var publishAction = eventSubscription.GetExecutionStrategy();

            Assert.NotNull(publishAction);

            publishAction.Invoke(null);

            completeEvent.WaitOne(5000);

            Assert.NotEqual(SynchronizationContext.Current, calledSyncContext);
        }

        #endregion Public Methods
    }
}