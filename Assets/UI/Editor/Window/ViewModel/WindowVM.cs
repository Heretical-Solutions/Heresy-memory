using HereticalSolutions.MVVM;
using HereticalSolutions.MVVM.Observable;
using HereticalSolutions.MVVM.ViewModel;
using UnityEngine;

namespace HereticalSolutions.Collections.Tests
{
    public class WindowVM : AViewModel
    {
        private IObservableProperty<bool> isRunning;
        private IObservableProperty<string> isRunningText;
        private IObservableProperty<Color> isRunningColor;

        /// <summary>
        /// Initialize view model
        /// </summary>
        public void Initialize(WindowModel model)
        {
            isRunning = model.IsRunning;

            isRunningText = new ObservableProperty<string>();

            isRunningColor = new ObservableProperty<Color>();


            isRunning.OnValueChanged += OnIsRunningChanged;
            
            OnIsRunningChanged(isRunning.Value);
            
            
            PublishObservable("Is Running", isRunning);
            
            PublishObservable("Is Running Text", isRunningText);
            
            PublishObservable("Is Running Color", isRunningColor);
            
            
            base.Initialize();
        }

        private void OnIsRunningChanged(bool newValue)
        {
            isRunningText.Value = newValue ? "STOP" : "START";

            isRunningColor.Value = newValue ? Color.green : Color.grey;
        }

        public override void Cleanup()
        {
            base.Cleanup();

            if (isRunning != null)
            {
                isRunning.OnValueChanged -= OnIsRunningChanged;
            }

            TearDownObservable(ref isRunning);
            
            TearDownObservable(ref isRunningText);
            
            TearDownObservable(ref isRunningColor);
        }
    }
}