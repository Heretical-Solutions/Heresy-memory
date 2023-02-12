using System;
using HereticalSolutions.MVVM.View;
using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class LabelView : AView
    {
        protected string propertyID;
        
        protected Label label;
        
        protected IObservableProperty<string> textProperty = null;
        
        public LabelView(
            IViewModel viewModel,
            string propertyID,
            Label label)
            : base(viewModel)
        {
            this.propertyID = propertyID;

            this.label = label;
        }
        
        public override void Initialize()
        {
            if (!viewModel.GetObservable<string>(propertyID, out textProperty))
                //Debug.LogError(
                throw new Exception($"[LabelView] Could not obtain property \"{propertyID}\" from ViewModel \"{viewModel.GetType()}\"");

            textProperty.OnValueChanged += OnTextChanged;

            OnTextChanged(textProperty.Value);

            base.Initialize();
        }
        
        protected void OnTextChanged(string newValue)
        {
            label.text = string.IsNullOrEmpty(newValue)
                ? string.Empty
                : newValue;
        }
        
        public override void Cleanup()
        {
            base.Cleanup();

            if (textProperty != null)
            {
                textProperty.OnValueChanged -= OnTextChanged;

                textProperty = null;
            }
        }
    }
}