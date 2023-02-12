using System;
using HereticalSolutions.MVVM.View;
using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ToggleView : AView
    {
        protected string propertyID;
        
        protected Toggle toggle;

        protected IObservableProperty<bool> boolProperty = null;

        protected bool togglePressed = false;
        
        public ToggleView(
            IViewModel viewModel,
            string propertyID,
            Toggle toggle)
            : base(viewModel)
        {
            this.propertyID = propertyID;

            this.toggle = toggle;
        }

        public override void Initialize()
        {
            if (!viewModel.GetObservable<bool>(propertyID, out boolProperty))
                //Debug.LogError(
                throw new Exception($"[ToggleView] Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\"");

            boolProperty.OnValueChanged += OnBoolChanged;

            OnBoolChanged(boolProperty.Value);
            
            toggle.RegisterValueChangedCallback(OnToggleValueChanged);

            base.Initialize();
        }
        
        protected virtual void OnBoolChanged(bool newValue)
        {
            if (togglePressed)
                return;
            
            toggle.value = newValue;
        }
        
        protected void OnToggleValueChanged(ChangeEvent<bool> @event)
        {
            togglePressed = true;
            
            boolProperty.Value = @event.newValue;

            togglePressed = false;
        }

        public override void Cleanup()
        {
            base.Cleanup();

            togglePressed = false;
            
            if (boolProperty != null)
            {
                boolProperty.OnValueChanged -= OnBoolChanged;

                boolProperty = null;
            }
            
            toggle.UnregisterValueChangedCallback(OnToggleValueChanged);
        }
    }
}