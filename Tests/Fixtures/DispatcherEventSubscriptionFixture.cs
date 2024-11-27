using System;
using System.Threading;
using EventAggregator.Interfaces;
using EventAggregator.Subscriptions;

namespace EventAggregatorTests.Fixtures
{
    public class DispatcherEventSubscriptionFixture
    {
        #region Public Methods

        [Fact]
        public void ShouldCallInvokeOnDispatcher()
        {
            DispatcherEventSubscription<object> eventSubscription = null;

            IDelegateReference actionDelegateReference = new MockDelegateReference()
            {
                Target = (Action<object>)(_ =>
                {
                    return;
                })
            };

            IDelegateReference filterDelegateReference = new MockDelegateReference
            {
                Target = (Predicate<object>)(_ => true)
            };
            var mockSyncContext = new MockSynchronizationContext();

            eventSubscription = new DispatcherEventSubscription<object>(actionDelegateReference, filterDelegateReference, mockSyncContext);

            eventSubscription.GetExecutionStrategy().Invoke([]);

            Assert.True(mockSyncContext.InvokeCalled);
        }

        [Fact]
        public void ShouldCallInvokeOnDispatcherNonGeneric()
        {
            DispatcherEventSubscription eventSubscription = null;

            IDelegateReference actionDelegateReference = new MockDelegateReference()
            {
                Target = () =>
                { }
            };

            var mockSyncContext = new MockSynchronizationContext();

            eventSubscription = new DispatcherEventSubscription(actionDelegateReference, mockSyncContext);

            eventSubscription.GetExecutionStrategy().Invoke([]);

            Assert.True(mockSyncContext.InvokeCalled);
        }

        [Fact]
        public void ShouldPassParametersCorrectly()
        {
            IDelegateReference actionDelegateReference = new MockDelegateReference()
            {
                Target =
                    (Action<object>)(_ =>
                    {
                        return;
                    })
            };
            IDelegateReference filterDelegateReference = new MockDelegateReference
            {
                Target = (Predicate<object>)(_ => true)
            };

            var mockSyncContext = new MockSynchronizationContext();

            var eventSubscription = new DispatcherEventSubscription<object>(actionDelegateReference, filterDelegateReference, mockSyncContext);

            var executionStrategy = eventSubscription.GetExecutionStrategy();
            Assert.NotNull(executionStrategy);

            var argument1 = new object();

            executionStrategy.Invoke([argument1]);

            Assert.Same(argument1, mockSyncContext.InvokeArg);
        }

        #endregion Public Methods
    }

    internal class MockSynchronizationContext
        : SynchronizationContext
    {
        #region Public Fields

        public object InvokeArg;
        public bool InvokeCalled;

        #endregion Public Fields

        #region Public Methods

        public override void Post(SendOrPostCallback d, object state)
        {
            InvokeCalled = true;
            InvokeArg = state;
        }

        #endregion Public Methods
    }
}