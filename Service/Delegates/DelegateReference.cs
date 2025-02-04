using System;
using System.Reflection;
using EventAggregator.Interfaces;

namespace EventAggregator.Delegates
{
    /// <summary>
    /// Represents a reference to a <see cref="Delegate"/> that may contain a
    /// <see cref="WeakReference"/> to the target. This class is used
    /// internally by the Prism Library.
    /// </summary>
    public class DelegateReference
        : IDelegateReference
    {
        #region Private Fields

        private readonly Delegate _delegate;
        private readonly Type _delegateType;
        private readonly MethodInfo _method;
        private readonly WeakReference _weakReference;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateReference"/>.
        /// </summary>
        /// <param name="delegat">The original <see cref="Delegate"/> to create a reference for.</param>
        /// <param name="keepReferenceAlive">If <see langword="false" /> the class will create a weak reference to the delegate, allowing it to be garbage collected. Otherwise it will keep a strong reference to the target.</param>
        /// <exception cref="ArgumentNullException">If the passed <paramref name="delegat"/> is not assignable to <see cref="Delegate"/>.</exception>
        public DelegateReference(Delegate delegat, bool keepReferenceAlive)
        {
            if (delegat is null)
            {
                throw new ArgumentNullException(nameof(delegat));
            }

            if (keepReferenceAlive)
            {
                _delegate = delegat;
            }
            else
            {
                _weakReference = new WeakReference(delegat.Target);
                _method = delegat.GetMethodInfo();
                _delegateType = delegat.GetType();
            }
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the <see cref="Delegate" /> (the target) referenced by the current <see cref="DelegateReference"/> object.
        /// </summary>
        /// <value><see langword="null"/> if the object referenced by the current <see cref="DelegateReference"/> object has been garbage collected; otherwise, a reference to the <see cref="Delegate"/> referenced by the current <see cref="DelegateReference"/> object.</value>
        public Delegate Target => _delegate ?? TryGetDelegate();

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Checks if the <see cref="Delegate" /> (the target) referenced by the current <see cref="DelegateReference"/> object are equal to another <see cref="Delegate" />.
        /// This is equivalent with comparing <see cref="Target"/> with <paramref name="delegate"/>, only more efficient.
        /// </summary>
        /// <param name="delegate">The other delegate to compare with.</param>
        /// <returns>True if the target referenced by the current object are equal to <paramref name="delegate"/>.</returns>
        public bool TargetEquals(Delegate @delegate)
        {
            if (_delegate != null)
            {
                return _delegate == @delegate;
            }

            if (@delegate == null)
            {
                return !_method.IsStatic && !_weakReference.IsAlive;
            }

            return _weakReference.Target == @delegate.Target
                && Equals(_method, @delegate.GetMethodInfo());
        }

        #endregion Public Methods

        #region Private Methods

        private Delegate TryGetDelegate()
        {
            if (_method.IsStatic)
            {
                return _method.CreateDelegate(_delegateType, null);
            }

            var target = _weakReference.Target;

            if (target != null)
            {
                return _method.CreateDelegate(_delegateType, target);
            }

            return null;
        }

        #endregion Private Methods
    }
}