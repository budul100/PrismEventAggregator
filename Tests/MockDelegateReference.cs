using System;
using EventAggregator.Interfaces;

namespace EventAggregatorTests
{
    internal class MockDelegateReference
        : IDelegateReference
    {
        #region Public Constructors

        public MockDelegateReference()
        { }

        public MockDelegateReference(Delegate target)
        {
            Target = target;
        }

        #endregion Public Constructors

        #region Public Properties

        public Delegate Target { get; set; }

        #endregion Public Properties
    }
}