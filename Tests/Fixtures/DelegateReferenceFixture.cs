using System;
using System.Threading.Tasks;
using EventAggregator.Delegates;

namespace EventAggregatorTests.Fixtures
{
    public class DelegateReferenceFixture
    {
        #region Public Methods

        [Fact]
        public void KeepAlivePreventsDelegateFromBeingCollected()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference(delegates.DoEvent, true);

            delegates = null;
            GC.Collect();

            Assert.NotNull(delegateReference.Target);
        }

        [Fact]
        public async Task NotKeepAliveAllowsDelegateToBeCollected()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference(delegates.DoEvent, false);

            delegates = null;
            await Task.Delay(100);
            GC.Collect();

            Assert.Null(delegateReference.Target);
        }

        [Fact]
        public async Task NotKeepAliveKeepsDelegateIfStillAlive()
        {
            var delegates = new SomeClassHandler();
            var delegateReference = new DelegateReference(delegates.DoEvent, false);

            GC.Collect();

            Assert.NotNull(delegateReference.Target);

            GC.KeepAlive(delegates);  //Makes delegates ineligible for garbage collection until this point (to prevent oompiler optimizations that may release the referenced object prematurely).
            delegates = null;
            await Task.Delay(100);
            GC.Collect();

            Assert.Null(delegateReference.Target);
        }

        [Fact]
        public async Task ShouldAllowCollectionOfOriginalDelegate()
        {
            var classHandler = new SomeClassHandler();
            var myAction = new Action<string>(classHandler.MyAction);

            var weakAction = new DelegateReference(myAction, false);

            var originalAction = new WeakReference(myAction);
            myAction = null;
            await Task.Delay(100);
            GC.Collect();
            Assert.False(originalAction.IsAlive);

            ((Action<string>)weakAction.Target)("payload");
            Assert.Equal("payload", classHandler.MyActionArg);
        }

        [Fact]
        public async Task ShouldReturnNullIfTargetNotAlive()
        {
            var handler = new SomeClassHandler();
            var weakHandlerRef = new WeakReference(handler);

            var action = new DelegateReference(handler.DoEvent, false);

            handler = null;
            await Task.Delay(100);
            GC.Collect();
            Assert.False(weakHandlerRef.IsAlive);

            Assert.Null(action.Target);
        }

        [Fact]
        public void TargetEqualsActionShouldReturnTrue()
        {
            var classHandler = new SomeClassHandler();
            var myAction = new Action<string>(classHandler.MyAction);

            var weakAction = new DelegateReference(myAction, false);

            Assert.True(weakAction.TargetEquals(new Action<string>(classHandler.MyAction)));
        }

        [Fact]
        public void TargetEqualsNullShouldReturnFalseIfTargetAlive()
        {
            var handler = new SomeClassHandler();
            var weakHandlerRef = new WeakReference(handler);

            var action = new DelegateReference(handler.DoEvent, false);

            Assert.False(action.TargetEquals(null));
            Assert.True(weakHandlerRef.IsAlive);
            GC.KeepAlive(handler);
        }

        [Fact]
        public async Task TargetEqualsNullShouldReturnTrueIfTargetNotAlive()
        {
            var handler = new SomeClassHandler();
            var weakHandlerRef = new WeakReference(handler);

            var action = new DelegateReference(handler.DoEvent, false);

            handler = null;

            // Intentional delay to encourage Garbage Collection to actually occur
            await Task.Delay(100);
            GC.Collect();
            Assert.False(weakHandlerRef.IsAlive);

            Assert.True(action.TargetEquals(null));
        }

        [Fact]
        public void TargetEqualsWorksWithStaticMethodDelegates()
        {
            var action = new DelegateReference(SomeClassHandler.StaticMethod, false);

            Assert.True(action.TargetEquals(SomeClassHandler.StaticMethod));
        }

        [Fact]
        public void TargetShouldReturnAction()
        {
            var classHandler = new SomeClassHandler();
            var myAction = new Action<string>(classHandler.MyAction);

            var weakAction = new DelegateReference(myAction, false);

            ((Action<string>)weakAction.Target)("payload");
            Assert.Equal("payload", classHandler.MyActionArg);
        }

        [Fact]
        public void WeakDelegateWorksWithStaticMethodDelegates()
        {
            var action = new DelegateReference(SomeClassHandler.StaticMethod, false);

            Assert.NotNull(action.Target);
        }

        #endregion Public Methods

        //todo: fix
        //[Fact]
        //public void NullDelegateThrows()
        //{
        //    Assert.ThrowsException<ArgumentNullException>(() =>
        //    {
        //        var action = new DelegateReference(null, true);
        //    });
        //}

        #region Public Classes

        public class SomeClassHandler
        {
            #region Public Fields

            public string MyActionArg;

            #endregion Public Fields

            #region Public Methods

            public static void StaticMethod()
            {
            }

            public void DoEvent(string value)
            {
                var myValue = value;
            }

            public void MyAction(string arg)
            {
                MyActionArg = arg;
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}