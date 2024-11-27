using System;
using System.Threading;
using EventAggregator.Interfaces;

namespace EventAggregator.Subscriptions
{
    public class DispatcherEventSubscription
        : EventSubscription
    {
        #region Private Fields

        private readonly SynchronizationContext context;

        #endregion Private Fields

        #region Public Constructors

        ///<summary>
        /// Extends <see cref="EventSubscription"/> to invoke the <see cref="EventSubscription.Action"/> delegate
        /// in a specific <see cref="SynchronizationContext"/>.
        ///</summary>
        ///<remarks>
        /// Creates a new instance of <see cref="BackgroundEventSubscription"/>.
        ///</remarks>
        ///<param name="actionReference">A reference to a delegate of type <see cref="Action{TPayload}"/>.</param>
        ///<param name="context">The synchronization context to use for UI thread dispatching.</param>
        ///<exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        ///<exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="Action{TPayload}"/>.</exception>
        public DispatcherEventSubscription(IDelegateReference actionReference, SynchronizationContext context)
            : base(actionReference)
        {
            this.context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Invokes the specified <see cref="Action{TPayload}"/> asynchronously in the specified <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public override void InvokeAction(Action action)
        {
            context.Post(
                d: (_) => action(),
                state: null);
        }

        #endregion Public Methods
    }
}