using System;
using UnityEngine.UI;
using HereticalSolutions.MVVM.View;

namespace HereticalSolutions.MVVM.Mono
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
                //Debug.LogError(
                throw new Exception($"[ButtonView] Could not obtain command \"{commandID}\" from ViewModel \"{viewModel.GetType()}\"");
            
            button.onClick.AddListener(OnButtonClicked);

            base.Initialize();
        }
        
        protected void OnButtonClicked()
        {
            onClickCommand();
        }

        public override void Cleanup()
        {
            base.Cleanup();

            button.onClick.RemoveAllListeners();
        }
    }
}