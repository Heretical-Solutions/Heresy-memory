using UnityEngine;
using UnityEngine.UI;

namespace HereticalSolutions.MVVM.Mono
{
    public class ButtonComponent : ViewComponent
    {
        [SerializeField]
        protected string commandID;

        [SerializeField]
        private Button button;

        protected override void Awake()
        {
            view = new ButtonView(baseViewModel.ViewModel, commandID, button);
            
            base.Awake();
        }
    }
}