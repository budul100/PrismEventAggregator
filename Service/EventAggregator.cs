using System;
using System.Collections.Generic;
using System.Threading;
using EventAggregator.Events;
using EventAggregator.Interfaces;

namespace EventAggregator
{
    /// <summary>
    /// Implements <see cref="IEventAggregator"/>.
    /// </summary>
    public class EventAggregator
        : IEventAggregator
    {
        #region Private Fields

        private readonly Dictionary<Type, EventBase> events = new Dictionary<Type, EventBase>();

        // Captures the sync context for the UI thread when constructed on the UI thread
        // in a platform agnostic way so it can be used for UI thread dispatching
        private readonly SynchronizationContext syncContext = SynchronizationContext.Current;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Gets the single instance of the event managed by this EventAggregator. Multiple calls to this method with the same <typeparamref name="TEventType"/> returns the same event instance.
        /// </summary>
        /// <typeparam name="TEventType">The type of event to get. This must inherit from <see cref="EventBase"/>.</typeparam>
        /// <returns>A singleton instance of an event object of type <typeparamref name="TEventType"/>.</returns>
        public TEventType GetEvent<TEventType>()
            where TEventType : EventBase, new()
        {
            lock (events)
            {
                if (!events.TryGetValue(
                    key: typeof(TEventType),
                    value: out var existingEvent))
                {
                    var newEvent = new TEventType
                    {
                        SynchronizationContext = syncContext
                    };

                    events[typeof(TEventType)] = newEvent;

                    return newEvent;
                }
                else
                {
                    return (TEventType)existingEvent;
                }
            }
        }

        #endregion Public Methods
    }
}