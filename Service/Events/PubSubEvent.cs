using System;
using System.Linq;
using EventAggregator.Delegates;
using EventAggregator.Enums;
using EventAggregator.Interfaces;
using EventAggregator.Subscriptions;

namespace EventAggregator.Events
{
    /// <summary>
    /// Defines a class that manages publication and subscription to events.
    /// </summary>
    public class PubSubEvent
        : EventBase
    {
        #region Public Methods

        /// <summary>
        /// Returns <see langword="true"/> if there is a subscriber matching <see cref="Action"/>.
        /// </summary>
        /// <param name="subscriber">The <see cref="Action"/> used when subscribing to the event.</param>
        /// <returns><see langword="true"/> if there is an <see cref="Action"/> that matches; otherwise <see langword="false"/>.</returns>
        public virtual bool Contains(Action subscriber)
        {
            IEventSubscription eventSubscription;

            lock (Subscriptions)
            {
                eventSubscription = Subscriptions.Cast<EventSubscription>()
                    .FirstOrDefault(evt => evt.Action == subscriber);
            }

            return eventSubscription != null;
        }

        /// <summary>
        /// Publishes the <see cref="PubSubEvent"/>.
        /// </summary>
        public virtual void Publish()
        {
            InternalPublish();
        }

        /// <summary>
        /// Subscribes a delegate to an event that will be published on the <see cref="ThreadOption.PublisherThread"/>.
        /// <see cref="PubSubEvent"/> will maintain a <see cref="WeakReference"/> to the target of the supplied <paramref name="action"/> delegate.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        /// <remarks>
        /// The PubSubEvent collection is thread-safe.
        /// </remarks>
        public SubscriptionToken Subscribe(Action action)
        {
            return Subscribe(
                action: action,
                threadOption: ThreadOption.PublisherThread);
        }

        /// <summary>
        /// Subscribes a delegate to an event.
        /// PubSubEvent will maintain a <see cref="WeakReference"/> to the Target of the supplied <paramref name="action"/> delegate.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is raised.</param>
        /// <param name="threadOption">Specifies on which thread to receive the delegate callback.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        /// <remarks>
        /// The PubSubEvent collection is thread-safe.
        /// </remarks>
        public SubscriptionToken Subscribe(Action action, ThreadOption threadOption)
        {
            return Subscribe(
                action: action,
                threadOption: threadOption,
                keepSubscriberReferenceAlive: false);
        }

        /// <summary>
        /// Subscribes a delegate to an event that will be published on the <see cref="ThreadOption.PublisherThread"/>.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <param name="keepSubscriberReferenceAlive">When <see langword="true"/>, the <see cref="PubSubEvent"/> keeps a reference to the subscriber so it does not get garbage collected.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        /// <remarks>
        /// If <paramref name="keepSubscriberReferenceAlive"/> is set to <see langword="false" />, <see cref="PubSubEvent"/> will maintain a <see cref="WeakReference"/> to the Target of the supplied <paramref name="action"/> delegate.
        /// If not using a WeakReference (<paramref name="keepSubscriberReferenceAlive"/> is <see langword="true" />), the user must explicitly call Unsubscribe for the event when disposing the subscriber in order to avoid memory leaks or unexpected behavior.
        /// <para/>
        /// The PubSubEvent collection is thread-safe.
        /// </remarks>
        public SubscriptionToken Subscribe(Action action, bool keepSubscriberReferenceAlive)
        {
            return Subscribe(
                action: action,
                threadOption: ThreadOption.PublisherThread,
                keepSubscriberReferenceAlive: keepSubscriberReferenceAlive);
        }

        /// <summary>
        /// Subscribes a delegate to an event.
        /// </summary>
        /// <param name="action">The delegate that gets executed when the event is published.</param>
        /// <param name="threadOption">Specifies on which thread to receive the delegate callback.</param>
        /// <param name="keepSubscriberReferenceAlive">When <see langword="true"/>, the <see cref="PubSubEvent"/> keeps a reference to the subscriber so it does not get garbage collected.</param>
        /// <returns>A <see cref="SubscriptionToken"/> that uniquely identifies the added subscription.</returns>
        /// <remarks>
        /// If <paramref name="keepSubscriberReferenceAlive"/> is set to <see langword="false" />, <see cref="PubSubEvent"/> will maintain a <see cref="WeakReference"/> to the Target of the supplied <paramref name="action"/> delegate.
        /// If not using a WeakReference (<paramref name="keepSubscriberReferenceAlive"/> is <see langword="true" />), the user must explicitly call Unsubscribe for the event when disposing the subscriber in order to avoid memory leaks or unexpected behavior.
        /// <para/>
        /// The PubSubEvent collection is thread-safe.
        /// </remarks>
        public virtual SubscriptionToken Subscribe(Action action, ThreadOption threadOption, bool keepSubscriberReferenceAlive)
        {
            var actionReference = new DelegateReference(
                delegat: action,
                keepReferenceAlive: keepSubscriberReferenceAlive);

            EventSubscription subscription;

            switch (threadOption)
            {
                case ThreadOption.PublisherThread:
                    subscription = new EventSubscription(actionReference);
                    break;

                case ThreadOption.BackgroundThread:
                    subscription = new BackgroundEventSubscription(actionReference);
                    break;

                case ThreadOption.UIThread:
                    if (SynchronizationContext == null)
                    {
                        throw new InvalidOperationException(
                            "To use the UIThread option for subscribing, the EventAggregator must be constructed on the UI thread.");
                    }

                    subscription = new DispatcherEventSubscription(
                        actionReference: actionReference,
                        context: SynchronizationContext);
                    break;

                default:
                    subscription = new EventSubscription(actionReference);
                    break;
            }

            return InternalSubscribe(subscription);
        }

        /// <summary>
        /// Removes the first subscriber matching <see cref="Action"/> from the subscribers' list.
        /// </summary>
        /// <param name="subscriber">The <see cref="Action"/> used when subscribing to the event.</param>
        public virtual void Unsubscribe(Action subscriber)
        {
            lock (Subscriptions)
            {
                var eventSubscription = Subscriptions
                    .Cast<EventSubscription>()
                    .FirstOrDefault(evt => evt.Action == subscriber);

                if (eventSubscription != null)
                {
                    Subscriptions.Remove(eventSubscription);
                }
            }
        }

        #endregion Public Methods
    }
}