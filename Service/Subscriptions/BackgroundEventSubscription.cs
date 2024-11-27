using System;
using System.Threading.Tasks;
using EventAggregator.Interfaces;

namespace EventAggregator.Subscriptions
{
    public class BackgroundEventSubscription
        : EventSubscription
    {
        #region Public Constructors

        /// <summary>
        /// Extends <see cref="EventSubscription"/> to invoke the <see cref="EventSubscription.Action"/> delegate in a background thread.
        /// </summary>
        /// <remarks>
        /// Creates a new instance of <see cref="BackgroundEventSubscription"/>.
        /// </remarks>
        /// <param name="actionReference">A reference to a delegate of type <see cref="Action"/>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="Action"/>.</exception>
        public BackgroundEventSubscription(IDelegateReference actionReference)
            : base(actionReference)
        { }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Invokes the specified <see cref="Action"/> in an asynchronous thread by using a <see cref="Task"/>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public override void InvokeAction(Action action)
        {
            Task.Run(action);
        }

        #endregion Public Methods
    }
}