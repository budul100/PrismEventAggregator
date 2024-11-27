using System;
using System.Collections.Generic;
using System.Linq;

namespace EventAggregator.Delegates
{
    public class WeakDelegatesManager
    {
        #region Private Fields

        private readonly List<DelegateReference> _listeners = new List<DelegateReference>();

        #endregion Private Fields

        #region Public Methods

        public void AddListener(Delegate listener)
        {
            _listeners.Add(new DelegateReference(listener, false));
        }

        public void Raise(params object[] args)
        {
            _listeners.RemoveAll(listener => listener.TargetEquals(null));

            var handlers = _listeners
                .Select(listener => listener.Target)
                .Where(listener => listener != null).ToList();

            foreach (var handler in handlers)
            {
                handler.DynamicInvoke(args);
            }
        }

        public void RemoveListener(Delegate listener)
        {
            //Remove the listener, and prune collected listeners
            _listeners.RemoveAll(reference => reference.TargetEquals(null) || reference.TargetEquals(listener));
        }

        #endregion Public Methods
    }
}