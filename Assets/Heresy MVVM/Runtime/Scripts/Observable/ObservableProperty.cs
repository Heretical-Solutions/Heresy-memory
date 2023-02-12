using System;
using System.Collections.Generic;

namespace HereticalSolutions.MVVM.Observable
{
    public delegate bool PollDelegate<T>(T previousValue, out T newValue);

    public class ObservableProperty<T>
        : IObservableProperty<T>,
          IPropertyModifiedNotifiable<T>,
          IValuePollable
    {
        #region Active polling
        
        private PollDelegate<T> activePollDelegate;

        public bool IsActivePollable { get; private set; }

        public void PollValue()
        {
            if (IsActivePollable
                && activePollDelegate(value, out value))
            {
                if (hasSubscribers)
                    OnValueChanged.Invoke(value);
            }
        }
        
        #endregion

        #region Model modification

        private Action<T> modifyModelDelegate;

        public bool IsModifyingModel { get; private set; }

        #endregion

        #region Value

        //I used to do OnValueChanged?.Invoke(value)\
        //Until I discovered that any kind of comparisons (even null checks) for delegates produce allocations
        private bool hasSubscribers = false;

        private Action<T> onValueChanged = null;

        public Action<T> OnValueChanged
        {
            get { return onValueChanged; }
            set
            {
                onValueChanged = value;

                hasSubscribers = onValueChanged != null;
            }
        }
        
        private T value;

        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;

                if (IsModifyingModel)
                    modifyModelDelegate(value);

                if (hasSubscribers)
                    OnValueChanged.Invoke(this.value);
            }
        }
        
        public object ObjectValue
        {
            get { return value; }
            set
            {
                this.value = (T)value;

                if (IsModifyingModel)
                    modifyModelDelegate((T)value);

                if (hasSubscribers)
                    OnValueChanged.Invoke(this.value);
            }
        }

        #endregion
        
        public ObservableProperty(
            T defaultValue = default(T),
            PollDelegate<T> activePollDelegate = null,
            Action<T> modifyModelDelegate = null)
        {
            value = defaultValue;
            this.activePollDelegate = activePollDelegate;
            this.modifyModelDelegate = modifyModelDelegate;

            IsActivePollable = activePollDelegate != null;
            IsModifyingModel = modifyModelDelegate != null;
            
            RemoveAllListeners();
        }

        public ObservableProperty(
            Func<T> pollValueDelegate,
            Action<T> modifyModelDelegate = null)
        {
            activePollDelegate =
                (T previousValue, out T result) =>
                {
                    result = pollValueDelegate();
                    
                    //Courtesy of https://stackoverflow.com/questions/488250/c-sharp-compare-two-generic-values
                    return !EqualityComparer<T>.Default.Equals(previousValue , result);
                };
            
            this.modifyModelDelegate = modifyModelDelegate;

            IsActivePollable = true;

            IsModifyingModel = modifyModelDelegate != null;
            
            RemoveAllListeners();

            activePollDelegate.Invoke(default(T), out value);
        }

        #region Cleanup
        
        public void RemoveAllListeners()
        {
            OnValueChanged = null;
        }

        public void Cleanup()
        {
            value = default(T);
            
            activePollDelegate = null;
            
            modifyModelDelegate = null;

            IsActivePollable = false;
            
            IsModifyingModel = false;
            
            RemoveAllListeners();
        }
        
        #endregion
    }
}