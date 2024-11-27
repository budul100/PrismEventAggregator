using System;

namespace EventAggregator.Events
{
    /// <summary>
    /// Generic arguments class to pass to event handlers that need to receive data.
    /// </summary>
    /// <typeparam name="TData">The type of data to pass.</typeparam>
    public class DataEventArgs<TData>
        : EventArgs
    {
        #region Public Constructors

        /// <remarks>
        /// Initializes the DataEventArgs class.
        /// </remarks>
        /// <param name="value">Information related to the event.</param>
        public DataEventArgs(TData value)
        {
            Value = value;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the information related to the event.
        /// </summary>
        /// <value>Information related to the event.</value>
        public TData Value { get; }

        #endregion Public Properties
    }
}