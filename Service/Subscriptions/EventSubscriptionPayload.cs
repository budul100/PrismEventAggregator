﻿using System;
using EventAggregator.Events;
using EventAggregator.Interfaces;

namespace EventAggregator.Subscriptions
{
    /// <summary>
    /// Provides a way to retrieve a <see cref="Delegate"/> to execute an action depending
    /// on the value of a second filter predicate that returns true if the action should execute.
    /// </summary>
    /// <typeparam name="TPayload">The type to use for the generic <see cref="Action{TPayload}"/> and <see cref="Predicate{TPayload}"/> types.</typeparam>
    public class EventSubscription<TPayload>
        : IEventSubscription
    {
        #region Private Fields

        private readonly IDelegateReference _actionReference;
        private readonly IDelegateReference _filterReference;

        #endregion Private Fields

        #region Public Constructors

        ///<summary>
        /// Creates a new instance of <see cref="EventSubscription{TPayload}"/>.
        ///</summary>
        ///<param name="actionReference">A reference to a delegate of type <see cref="Action{TPayload}"/>.</param>
        ///<param name="filterReference">A reference to a delegate of type <see cref="Predicate{TPayload}"/>.</param>
        ///<exception cref="ArgumentNullException">When <paramref name="actionReference"/> or <see paramref="filterReference"/> are <see langword="null" />.</exception>
        ///<exception cref="ArgumentException">When the target of <paramref name="actionReference"/> is not of type <see cref="Action{TPayload}"/>,
        ///or the target of <paramref name="filterReference"/> is not of type <see cref="Predicate{TPayload}"/>.</exception>
        public EventSubscription(IDelegateReference actionReference, IDelegateReference filterReference)
        {
            if (actionReference is null)
            {
                throw new ArgumentNullException(nameof(actionReference));
            }

            if (!(actionReference.Target is Action<TPayload>))
            {
                throw new ArgumentException(
                    message: "Invalid Delegate Reference Type Exception",
                    paramName: nameof(actionReference));
            }

            if (filterReference is null)
            {
                throw new ArgumentNullException(nameof(filterReference));
            }

            if (!(filterReference.Target is Predicate<TPayload>))
            {
                throw new ArgumentException(
                    message: "Invalid Delegate Reference Type Exception",
                    paramName: nameof(filterReference));
            }

            _actionReference = actionReference;
            _filterReference = filterReference;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the target <see cref="Action{T}"/> that is referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        /// <value>An <see cref="Action{T}"/> or <see langword="null" /> if the referenced target is not alive.</value>
        public Action<TPayload> Action => (Action<TPayload>)_actionReference.Target;

        /// <summary>
        /// Gets the target <see cref="Predicate{T}"/> that is referenced by the <see cref="IDelegateReference"/>.
        /// </summary>
        /// <value>An <see cref="Predicate{T}"/> or <see langword="null" /> if the referenced target is not alive.</value>
        public Predicate<TPayload> Filter => (Predicate<TPayload>)_filterReference.Target;

        /// <summary>
        /// Gets or sets a <see cref="SubscriptionToken"/> that identifies this <see cref="IEventSubscription"/>.
        /// </summary>
        /// <value>A token that identifies this <see cref="IEventSubscription"/>.</value>
        public SubscriptionToken SubscriptionToken { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the execution strategy to publish this event.
        /// </summary>
        /// <returns>An <see cref="Action{T}"/> with the execution strategy, or <see langword="null" /> if the <see cref="IEventSubscription"/> is no longer valid.</returns>
        /// <remarks>
        /// If <see cref="Action"/> or <see cref="Filter"/> are no longer valid because they were
        /// garbage collected, this method will return <see langword="null" />.
        /// Otherwise it will return a delegate that evaluates the <see cref="Filter"/> and if it
        /// returns <see langword="true" /> will then call <see cref="InvokeAction"/>. The returned
        /// delegate holds hard references to the <see cref="Action"/> and <see cref="Filter"/> target
        /// <see cref="Delegate">delegates</see>. As long as the returned delegate is not garbage collected,
        /// the <see cref="Action"/> and <see cref="Filter"/> references delegates won't get collected either.
        /// </remarks>
        public virtual Action<object[]> GetExecutionStrategy()
        {
            var action = Action;
            var filter = Filter;

            if (action != null
                && filter != null)
            {
                return arguments =>
                {
                    TPayload argument = default;

                    if (arguments?.Length > 0 && arguments[0] != null)
                    {
                        argument = (TPayload)arguments[0];
                    }

                    if (filter(argument))
                    {
                        InvokeAction(action, argument);
                    }
                };
            }

            return null;
        }

        /// <summary>
        /// Invokes the specified <see cref="Action{TPayload}"/> synchronously when not overridden.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="argument">The payload to pass <paramref name="action"/> while invoking it.</param>
        /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if <paramref name="action"/> is null.</exception>
        public virtual void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(argument);
        }

        #endregion Public Methods
    }
}