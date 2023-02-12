using System;

namespace HereticalSolutions.MVVM
{
    public interface IObservableProperty<T>
    {
        T Value { get; set; }
        
        object ObjectValue { get; set; }
        
        Action<T> OnValueChanged { get; set; }
        
        void RemoveAllListeners();

        void Cleanup();
    }
}