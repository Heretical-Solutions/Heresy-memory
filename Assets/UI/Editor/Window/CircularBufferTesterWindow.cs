using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using HereticalSolutions.MVVM.UIToolkit;

namespace HereticalSolutions.Collections.Tests
{
    public class CircularBufferTesterWindow : EditorWindow
    {
        private WindowModel model;
        
        private WindowVM vm;
        
        [MenuItem("Tools/Testers/Circular Buffer")]
        public static void Initialize()
        {
            //Debug.Log("Initialize start");
            
            CircularBufferTesterWindow window = GetWindow<CircularBufferTesterWindow>();

            window.titleContent = new GUIContent("Circular Buffer");
            
            //Debug.Log("Initialize stop");
        }

        public void CreateGUI()
        {
            //Debug.Log("CreateGUI start");
            
            
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            /*
            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("Hello World! From C#");
            
            root.Add(label);
            */

            //Debug.Log("CreateGUI stop");
            
            //return;
            
            /*
            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/UI/Editor/Visual Trees/CircularBufferTesterWindow.uxml");
            
            VisualElement labelFromUXML = visualTree.Instantiate();
            
            root.Add(labelFromUXML);
            */
            
            /*
            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/UI/Editor/Stylesheets/CircularBufferTesterWindow.uss");
            
            VisualElement labelWithStyle = new Label("Hello World! With Style");
            labelWithStyle.styleSheets.Add(styleSheet);
            root.Add(labelWithStyle);
            */

            model = WindowFactory.BuildWindowModel();

            vm = WindowFactory.BuildWindowVM();
            
            WindowFactory.BuildSimulationWindow(root, vm);
            
            vm.Initialize(model);
        }
        
        void OnDestroy()
        {
            vm?.TearDown();
        }
    }
}