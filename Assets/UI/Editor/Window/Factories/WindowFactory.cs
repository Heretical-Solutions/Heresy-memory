using System;
using HereticalSolutions.MVVM.LifetimeManagement;
using HereticalSolutions.MVVM.Observable;
using HereticalSolutions.MVVM.UIToolkit;
using HereticalSolutions.MVVM.View;
using UnityEngine.UIElements;

namespace HereticalSolutions.Collections.Tests
{
    public static class WindowFactory
    {
        public static WindowModel BuildWindowModel()
        {
            return new WindowModel
            {
                IsRunning = new ObservableProperty<bool>(false)
            };
        }

        public static WindowVM BuildWindowVM()
        {
            WindowVM vm = new WindowVM();
            
            vm.SetUp();

            return vm;
        }

        public static void BuildSimulationWindow(
            VisualElement root,
            WindowVM vm)
        {
            BuildSimulationButton(root, vm);
        }

        public static void BuildSimulationButton(
            VisualElement parent,
            WindowVM windowVM)
        {
            VisualElement button = BuildSimulationButton(
                windowVM,
                out ToggleButtonView toggleView,
                out BackgroundColorView backgroundColorView,
                out LabelView labelView);
            
            
            toggleView.SetUp();
            
            LifetimeController.SyncLifetimes(toggleView, windowVM);
            
            backgroundColorView.SetUp();
            
            LifetimeController.SyncLifetimes(backgroundColorView, windowVM);
            
            labelView.SetUp();
            
            LifetimeController.SyncLifetimes(labelView, windowVM);
            
            
            parent.Add(button);
        }

        public static VisualElement BuildSimulationButton(
            WindowVM windowVM,
            out ToggleButtonView toggleView,
            out BackgroundColorView backgroundColorView,
            out LabelView labelView)
        {
            Button button = new Button();

            toggleView = new ToggleButtonView(windowVM, "Is Running", button);

            backgroundColorView = new BackgroundColorView(windowVM, "Is Running Color", button);

            Label label = new Label("");

            labelView = new LabelView(windowVM, "Is Running Text", label);
            
            button.Add(label);

            return button;
        }
    }
}