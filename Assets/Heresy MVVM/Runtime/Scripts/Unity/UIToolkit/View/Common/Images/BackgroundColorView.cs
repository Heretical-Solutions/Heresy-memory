using System;
using HereticalSolutions.MVVM.View;
using UnityEngine;
using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class BackgroundColorView : AView
    {
        protected string propertyID;
        
        protected VisualElement visualElement;

        protected IObservableProperty<Color> colorProperty = null;

        public BackgroundColorView(
            IViewModel viewModel,
            string propertyID,
            VisualElement visualElement)
            : base(viewModel)
        {
            this.propertyID = propertyID;

            this.visualElement = visualElement;
        }
        
        public override void Initialize()
        {
            if (!viewModel.GetObservable<Color>(propertyID, out colorProperty))
                //Debug.LogError(
                throw new Exception($"[BackgroundColorView] Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\"");
            
            colorProperty.OnValueChanged += OnColorChanged;

            OnColorChanged(colorProperty.Value);

            base.Initialize();
        }
        
        protected void OnColorChanged(Color newValue)
        {
            visualElement.style.backgroundColor = newValue;
        }

        public override void Cleanup()
        {
            base.Cleanup();

            if (colorProperty != null)
            {
                colorProperty.OnValueChanged -= OnColorChanged;

                colorProperty = null;
            }
        }
    }
}