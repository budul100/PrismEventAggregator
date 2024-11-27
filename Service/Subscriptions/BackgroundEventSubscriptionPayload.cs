using System;
using System.Threading.Tasks;
using EventAggregator.Interfaces;

namespace EventAggregator.Subscriptions
{
    /// <summary>
    /// Extends <see cref="EventSubscription{TPayload}"/> to invoke the <see cref="EventSubscription{TPayload}.Action"/> delegate in a background thread.
    /// </summary>
    /// <typeparam name="TPayload">The type to use for the generic <see cref="Action{TPayload}"/> and <see cref="Predicate{TPayload}"/> types.</typeparam>
    public class BackgroundEventSubscription<TPayload>
        : EventSubscription<TPayload>
    {
        #region Public Constructors

        /// <summary>
        /// Extends <see cref="EventSubscription{TPayload}"/> to invoke the <see cref="EventSubscription{TPayload}.Action"/> delegate in a background thread.
        /// </summary>
        /// <remarks>
        /// Creates a new instance of <see cref="BackgroundEventSubscription{TPayload}"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="Action{TPayload}"/>,
        /// or the target of <paramref name="filterReference"/> is not of type <see cref="Predicate{TPayload}"/>.</exception>
        /// <param name="actionReference">A reference to a delegate of type <see cref="Action{TPayload}"/>.</param>
        /// <param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        public BackgroundEventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
            : base(actionReference, filterReference)
        { }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Invokes the specified <see cref="Action{TPayload}"/> in an asynchronous thread by using a <see cref="ThreadPool"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        public override void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            //ThreadPool.QueueUserWorkItem( (o) => action(argument) );
            Task.Run(() => action(argument));
        }

        #endregion Public Methods
    }
}