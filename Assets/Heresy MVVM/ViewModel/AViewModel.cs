using System;
using System.Collections.Generic;

namespace HereticalSolutions.MVVM.ViewModel
{
    /// <summary>
    /// Base class for view models containing boilerplate logic
    /// </summary>
    
    public abstract class AViewModel : IViewModel
    {
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
            IsSetUp = true;
        }

        /// <summary>
        /// Initialize view model
        /// </summary>
        public virtual void Initialize()
        {
            if (IsInitialized)
            {
                throw new Exception($"[AViewModel] Initializing a view model that is already initialized: {this.GetType().ToBeautifulString()}");
            }

            IsInitialized = true;

            OnInitialized?.Invoke();
        }

        /// <summary>
        /// Clean up view model
        /// </summary>
        public virtual void Cleanup()
        {
            IsInitialized = false;

            OnCleanedUp?.Invoke();

            UnpublishObservables();
            
            UnpublishCommands();
        }

        /// <summary>
        /// Tear down view model
        /// </summary>
        public virtual void TearDown()
        {
            IsSetUp = false;
            
            Cleanup();

            OnTornDown?.Invoke();
            
            
            OnInitialized = null;
            
            OnCleanedUp = null;
            
            OnTornDown = null;
        }

        #endregion

        #region Observables

        /// <summary>
        /// Exposed observables list
        /// </summary>
        protected Dictionary<string, object> observables = new Dictionary<string, object>();

        /// <summary>
        /// Add the observable to list of exposed observables
        /// </summary>
        /// <param name="key">Identifier</param>
        /// <param name="observable">Observable</param>
        protected void PublishObservable(string key, object observable)
        {
            if (observables.ContainsKey(key))
            {
                //Debug.LogError(
                throw new Exception($"[AViewModel] Exposed observables list already has a key \"{key}\": {this.GetType().ToBeautifulString()}");
            }

            observables.Add(key, observable);
        }

        /// <summary>
        /// Try get the observable by identifier
        /// </summary>
        /// <typeparam name="T">Observable generic type</typeparam>
        /// <param name="key">Identifier</param>
        /// <param name="observable">Observable</param>
        /// <returns>Whether the observable is present in the exposed observables list</returns>
        public bool GetObservable<T>(string key, out IObservableProperty<T> observable)
        {
            object output;

            bool result = observables.TryGetValue(key, out output);

            /*
            if (!result)
            {
                //Debug.LogError(
                throw new Exception($"[AViewModel] Exposed observables list does not have a key \"{key}\": {this.GetType().ToBeautifulString()}");
            }
            */

            observable = result ? (IObservableProperty<T>)output : null;

            return result;
        }
        
        /// <summary>
        /// Try to poll observable property's value with active poll delegate
        /// </summary>
        /// <param name="observableProperty">Observable property</param>
        /// <typeparam name="T"></typeparam>
        protected void TryPollObservable<T>(IObservableProperty<T> observableProperty)
        {
            if (observableProperty != null)
                ((IValuePollable)observableProperty).PollValue();
        }

        /// <summary>
        /// Tear down the observable and clean it up
        /// </summary>
        /// <param name="observableProperty">Observable property</param>
        /// <typeparam name="T">Observable value type</typeparam>
        protected void TearDownObservable<T>(ref IObservableProperty<T> observableProperty)
        {
            if (observableProperty != null)
            {
                observableProperty.Cleanup();

                observableProperty = null;
            }
        }

        /// <summary>
        /// Cleanup the exposed observables list
        /// </summary>
        protected void UnpublishObservables()
        {
            observables.Clear();
        }
        
        #endregion
        
        #region Commands
        
        /// <summary>
        /// List of delegates that views can fire as callbacks to user actions
        /// </summary>
        /// <typeparam name="string">Key</typeparam>
        /// <typeparam name="CommandDelegate">Value</typeparam>
        protected Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>();

        /// <summary>
        /// List of delegates that views can fire as callbacks to user actions (with arguments)
        /// </summary>
        /// <typeparam name="string">Key</typeparam>
        /// <typeparam name="CommandWithArgsDelegate">Value</typeparam>
        protected Dictionary<string, CommandWithArgsDelegate> commandsWithArguments = new Dictionary<string, CommandWithArgsDelegate>();
        
        /// <summary>
        /// Add the delegate to list of exposed commands
        /// </summary>
        /// <param name="key">Identifier</param>
        /// <param name="delegate">Delegate</param>
        protected void PublishCommand(string key, CommandDelegate @delegate)
        {
            if (commands.ContainsKey(key))
            {
                //Debug.LogError(
                throw new Exception($"[AViewModel] Exposed commands list already has a key \"{key}\": {this.GetType().ToBeautifulString()}");
            }

            commands.Add(key, @delegate);
        }
        
        /// <summary>
        /// Add the delegate to list of exposed commands with arguments
        /// </summary>
        /// <param name="key">Identifier</param>
        /// <param name="delegate">Delegate</param>
        protected void PublishCommandWithArguments(string key, CommandWithArgsDelegate @delegate)
        {
            if (commandsWithArguments.ContainsKey(key))
            {
                //Debug.LogError(
                throw new Exception($"[AViewModel] Exposed commands with arguments list already has a key \"{key}\": {this.GetType().ToBeautifulString()}");
            }

            commandsWithArguments.Add(key, @delegate);
        }

        /// <summary>
        /// Try get the command by identifier
        /// </summary>
        /// <param name="key">Identifier</param>
        /// <returns>Command</returns>
        public CommandDelegate GetCommand(string key)
        {
            CommandDelegate result = null;

            if (!commands.TryGetValue(key, out result))
            {
                //Debug.LogError(
                throw new Exception($"[AViewModel] Exposed commands list does not have a key \"{key}\": {this.GetType().ToBeautifulString()}");
            }

            return result;
        }
        
        /// <summary>
        /// Try get the command with arguments by identifier
        /// </summary>
        /// <param name="key">Identifier</param>
        /// <returns>Command</returns>
        public CommandWithArgsDelegate GetCommandWithArguments(string key)
        {
            CommandWithArgsDelegate result = null;

            if (!commandsWithArguments.TryGetValue(key, out result))
            {
                //Debug.LogError(
                throw new Exception($"[AViewModel] Exposed commands with arguments list does not have a key \"{key}\": {this.GetType().ToBeautifulString()}");
            }

            return result;
        }

        /// <summary>
        /// Cleanup the exposed commands list
        /// </summary>
        protected void UnpublishCommands()
        {
            commands.Clear();
            
            commandsWithArguments.Clear();
        }

        #endregion
    }
}