using System;
using HereticalSolutions.MVVM.View;
using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ToggleButtonView : AView
    {
        protected string propertyID;
        
        protected Button button;

        protected IObservableProperty<bool> boolProperty = null;
        
        public ToggleButtonView(
            IViewModel viewModel,
            string propertyID,
            Button button)
            : base(viewModel)
        {
            this.propertyID = propertyID;

            this.button = button;
        }

        public override void Initialize()
        {
            if (!viewModel.GetObservable<bool>(propertyID, out boolProperty))
                //Debug.LogError(
                throw new Exception($"[ToggleButtonView] Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\"");
            
            /*
            boolProperty.OnValueChanged += OnBoolChanged;

            OnBoolChanged(boolProperty.Value);
            */
            
            button.RegisterCallback<ClickEvent>(OnButtonClicked);

            base.Initialize();
        }

        /*
        protected virtual void OnBoolChanged(bool newValue)
        {
        }
        */
        
        protected void OnButtonClicked(ClickEvent @event)
        {
            boolProperty.Value = !boolProperty.Value;
        }

        public override void Cleanup()
        {
            base.Cleanup();

            if (boolProperty != null)
            {
                //boolProperty.OnValueChanged -= OnBoolChanged;

                boolProperty = null;
            }
            
            button.UnregisterCallback<ClickEvent>(OnButtonClicked);
        }
    }
}