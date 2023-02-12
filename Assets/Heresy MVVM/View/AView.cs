using System;

namespace HereticalSolutions.MVVM.View
{
    public abstract class AView : ILifetimeable
    {
        /*
        [SerializeField]
        protected AViewModelBase baseViewModel;
        */

        protected IViewModel viewModel;

        public AView(IViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        #region ILifetimable
        
        public bool IsSetUp { get; protected set; } = false;
        
        public bool IsInitialized { get; protected set; } = false;

        /// <summary>
        /// Initialization callback
        /// </summary>
        public Action OnInitialized { get; set; }

        /// <summary>
        /// Cleanup callback
        /// </summary>
        public Action OnCleanedUp { get; set; }

        /// <summary>
        /// Destruction callback
        /// </summary>
        public Action OnTornDown { get; set; }

        /// <summary>
        /// Self initialization
        /// </summary>
        public virtual void SetUp()
        {
        }

        /// <summary>
        /// Initialize view
        /// </summary>
        public virtual void Initialize()
        {
            if (IsInitialized)
            {
                throw new Exception($"[AView] Initializing a view that is already initialized: {this.GetType().ToBeautifulString()}");
            }

            IsInitialized = true;

            OnInitialized?.Invoke();
        }

        /// <summary>
        /// Cleanup view
        /// </summary>
        public virtual void Cleanup()
        {
            IsInitialized = false;

            OnCleanedUp?.Invoke();
        }

        /// <summary>
        /// Tear down view
        /// </summary>
        public virtual void TearDown()
        {
            IsSetUp = false;
            
            Cleanup();

            OnTornDown?.Invoke();
        }

        /*
        /// <summary>
        /// Assert variable to ensure there are no excessive subscriptions
        /// </summary>
        protected int subscriptionsCounter = 0;

        protected virtual void Start() //Awake()
        {
            Startup();

            if (baseViewModel.Initialized)
                OnInitialized();
        }
        */

        #endregion

        protected void TryObtainProperty<T>(string propertyID, out IObservableProperty<T> property)
        {
            bool propertyObtained = viewModel.GetObservable<T>(propertyID, out property);

            if (!propertyObtained)
            {
                //Debug.LogError(
                throw new Exception($"[AView] Could not obtain property {propertyID} from viewmodel {viewModel.GetType()}");
            }
        }

        /*
        /// <summary>
        /// Perform deinitialization logic upon gameobject destruction
        /// </summary>
        protected virtual void OnDestroy()
        {
            DesyncLifetimeWithVM();
            
            OnTornDown();
        }
        */
    }
}