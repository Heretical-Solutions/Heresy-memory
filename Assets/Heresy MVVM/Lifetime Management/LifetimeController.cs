using System;

namespace HereticalSolutions.MVVM.LifetimeManagement
{
    public static class LifetimeController
    {
        /// <summary>
        /// Attach target's OnInitialized to parent's OnInitialized, 
        /// target's OnCleanedUp to parent's OnCleanedUp 
        /// and target's TearDown to parent's OnTornDown
        /// </summary>
        /// <param name="target">Target lifetimable</param>
        /// <param name="parent">Target's parent lifetimeable</param>
        public static void SyncLifetimes(
            ILifetimeable target,
            ILifetimeable parent)
        {
            if (parent == null)
                return;
            
            parent.OnInitialized += target.Initialize;
            parent.OnCleanedUp += target.Cleanup;

            Action desyncDelegate = null;

            desyncDelegate = () =>
            {
                parent.OnInitialized -= target.Initialize;
                parent.OnCleanedUp -= target.Cleanup;

                parent.OnTornDown -= desyncDelegate;

                target.TearDown();
            };

            parent.OnTornDown += desyncDelegate;
        }

        /// <summary>
        /// Attach target's custom initialization delegate to parent's OnInitialized, 
        /// target's OnCleanedUp to parent's OnCleanedUp 
        /// and target's TearDown to parent's OnTornDown
        /// </summary>
        /// <param name="target">Target lifetimable</param>
        /// <param name="parent">Target's parent lifetimeable</param>
        /// <param name="initializationDelegate">Target's custom initialization delegate</param>
        public static void SyncLifetimes(
            ILifetimeable target,
            ILifetimeable parent,
            Action initializationDelegate)
        {
            if (parent == null)
                return;
            
            parent.OnInitialized += initializationDelegate;
            parent.OnCleanedUp += target.Cleanup;

            Action desyncDelegate = null;

            desyncDelegate = () =>
            {
                parent.OnInitialized -= initializationDelegate;
                parent.OnCleanedUp -= target.Cleanup;

                parent.OnTornDown -= desyncDelegate;

                target.TearDown();
            };

            parent.OnTornDown += desyncDelegate;
        }
    }
}