using HereticalSolutions.MVVM.LifetimeManagement;
using UnityEngine;

namespace HereticalSolutions.MVVM.Mono
{
    public abstract class ViewModelComponent : MonoBehaviour
    {
        #region View Model
        
        protected IViewModel viewModel;

        public IViewModel ViewModel { get => viewModel; }

        #endregion
        
        #region Hierarchy

        [SerializeField]
        protected ViewModelComponent parentVM;
        
        /// <summary>
        /// Reference to parent VM component
        /// </summary>
        public ViewModelComponent Parent { get; protected set; }

        #endregion

        #region Unity callbacks
        
        protected virtual void Awake()
        {
            if (parentVM != null)
                Parent = parentVM;
            
            viewModel.SetUp();
        }

        protected virtual void Start()
        {
            //Sync with parent VM if there is one
            if (Parent != null)
            {
                LifetimeController.SyncLifetimes(viewModel, Parent.ViewModel);
                
                //OnEnable is performed BEFORE Start, therefore the root VM would already be initialized
                if (Parent.ViewModel.IsInitialized)
                    InitializeViewModel();
            }
        }

        protected virtual void OnEnable()
        {
            //Do not initialize if non-root VM. The parent may not be initialized yet
            //Rather perform SyncLifetimes at Start and wait for parent to call Initialize()
            if (Parent != null)
                return;
            
            InitializeViewModel();
        }

        protected virtual void InitializeViewModel()
        {
            viewModel.Initialize();
        }

        protected void OnDisable()
        {
            viewModel.Cleanup();
        }

        /// <summary>
        /// Perform tear down logic upon gameobject destruction
        /// </summary>
        protected virtual void OnDestroy()
        {
            viewModel.TearDown();
        }
        
        #endregion
    }
}