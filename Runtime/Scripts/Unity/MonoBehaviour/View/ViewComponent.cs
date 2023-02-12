using HereticalSolutions.MVVM;
using HereticalSolutions.MVVM.LifetimeManagement;
using HereticalSolutions.MVVM.View;
using UnityEngine;

namespace HereticalSolutions.MVVM.Mono
{
    public abstract class ViewComponent : MonoBehaviour
    {
        
        [SerializeField]
        protected ViewModelComponent baseViewModel;

        protected AView view;
        
        #region Unity callbacks
        
        protected virtual void Awake()
        {
            view.SetUp();
        }

        protected virtual void Start()
        {
            LifetimeController.SyncLifetimes(view, baseViewModel.ViewModel);
            
            if (baseViewModel.ViewModel.IsInitialized)
                InitializeView();
        }

        protected virtual void InitializeView()
        {
            view.Initialize();
        }

        protected void OnDisable()
        {
            view.Cleanup();
        }

        /// <summary>
        /// Perform tear down logic upon gameobject destruction
        /// </summary>
        protected virtual void OnDestroy()
        {
            view.TearDown();
        }
        
        #endregion
    }
}