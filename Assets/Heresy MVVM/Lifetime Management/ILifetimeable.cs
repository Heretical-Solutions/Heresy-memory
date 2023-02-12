using System;

namespace HereticalSolutions.MVVM
{
    public interface ILifetimeable
    {
        #region Set up
        
        void SetUp();

        bool IsSetUp { get; }

        //Action OnSetUp { get; set; }
        
        #endregion

        #region Initialize

        void Initialize();
        
        bool IsInitialized { get; }
        
        Action OnInitialized { get; set; }

        #endregion

        #region Cleanup
        void Cleanup();
        
        Action OnCleanedUp { get; set; }

        #endregion

        #region Tear down
        
        void TearDown();
        
        Action OnTornDown { get; set; }
        
        #endregion
    }
}