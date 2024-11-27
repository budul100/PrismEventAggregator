using System;
using System.Threading;
using EventAggregator.Interfaces;

namespace EventAggregator.Subscriptions
{
    ///<summary>
    /// Extends <see cref="EventSubscription{TPayload}"/> to invoke the <see cref="EventSubscription{TPayload}.Action"/> delegate
    /// in a specific <see cref="SynchronizationContext"/>.
    ///</summary>
    /// <typeparam name="TPayload">The type to use for the generic <see cref="Action{TPayload}"/> and <see cref="Predicate{TPayload}"/> types.</typeparam>
    public class DispatcherEventSubscription<TPayload>
        : EventSubscription<TPayload>
    {
        #region Private Fields

        private readonly SynchronizationContext context;

        #endregion Private Fields

        #region Public Constructors

        ///<summary>
        /// Extends <see cref="EventSubscription{TPayload}"/> to invoke the <see cref="EventSubscription{TPayload}.Action"/> delegate
        /// in a specific <see cref="SynchronizationContext"/>.
        ///</summary>
        ///<remarks>
        /// Creates a new instance of <see cref="BackgroundEventSubscription{TPayload}"/>.
        ///</remarks>
        ///<param name="actionReference">A reference to a delegate of type <see cref="Action{TPayload}"/>.</param>
        ///<param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        ///<param name="context">The synchronization context to use for UI thread dispatching.</param>
        ///<exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        ///<exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="Action{TPayload}"/>,
        ///or the target of <paramref name="filterReference"/> is not of type <see cref="Predicate{TPayload}"/>.</exception>
        public DispatcherEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference,
            SynchronizationContext context)
            : base(actionReference, filterReference)
        {
            this.context = context;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Invokes the specified <see cref="Action{TPayload}"/> asynchronously in the specified <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            context.Post(
                d: (o) => action((TPayload)o),
                state: argument);
        }

        #endregion Public Methods
    }
}