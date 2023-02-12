using System;
using HereticalSolutions.MVVM.View;
using UnityEngine.UIElements;

namespace HereticalSolutions.MVVM.UIToolkit
{
    public class ButtonView : AView
    {
        protected string commandID;
        
        protected Button button;

        protected CommandDelegate onClickCommand = null;
        
        public ButtonView(
            IViewModel viewModel,
            string commandID,
            Button button)
            : base(viewModel)
        {
            this.commandID = commandID;

            this.button = button;
        }

        public override void Initialize()
        {
            onClickCommand = viewModel.GetCommand(commandID);

            if (onClickCommand == null)
                throw new Exception($"[ButtonView] Could not obtain command \"{commandID}\" from ViewModel \"{viewModel.GetType()}\"");
            
            button.RegisterCallback<ClickEvent>(OnButtonClicked);

            base.Initialize();
        }

        protected void OnButtonClicked(ClickEvent @event)
        {
            onClickCommand();
        }

        public override void Cleanup()
        {
            base.Cleanup();

            button.UnregisterCallback<ClickEvent>(OnButtonClicked);
        }
    }
}