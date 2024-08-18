using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using MagicaCapsuleCollider1 = MagicaCloth.MagicaCapsuleCollider;
using Magica1Axis = MagicaCloth.MagicaCapsuleCollider.Axis;
using Magica2Axis = MagicaCloth2.MagicaCapsuleCollider.Direction;
using MagicaCapsuleCollider2 = MagicaCloth2.MagicaCapsuleCollider;


namespace MagicaConverter{
	public class MagicaConverterWindow : EditorWindow
	{
		private enum ConversionTypes{
			None,
			Full,
			Colliders,
			Cloth,
			Bone
		}

		

		private bool PRESERVE_COMPONENTS = false;
		private GameObject selectedObject;
		private Label selectedGameObjectLabel;
		private ConversionTypes selectedOption = ConversionTypes.None;
		[MenuItem("Tools/MagicaConverter")]
		public static void ShowWindow()
		{
			// Create or show the custom editor window
			MagicaConverterWindow wnd = GetWindow<MagicaConverterWindow>();
			wnd.titleContent = new GUIContent("MagicaConverter");
		}

		public void CreateGUI()
		{
			// Get the root visual element of the window
			VisualElement root = rootVisualElement;

			Label warningLabel = new Label("The conversion is not 100% accurate\n when using different start and end radius.\n Sometime the resulting lenght is too high,\n in such cases it will be clamped to 2");
			
			// Apply styles to the label
			warningLabel.style.color = Color.yellow;
			warningLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
			warningLabel.style.backgroundColor = new Color(1, 0, 0, 0.3f); // Semi-transparent red background
			warningLabel.style.paddingLeft = 5;
			warningLabel.style.paddingRight = 5;
			warningLabel.style.paddingTop = 3;
			warningLabel.style.paddingBottom = 3;
			warningLabel.style.marginBottom = 10;

			// Add the label to the root visual element
			root.Add(warningLabel);

			EnumField enumField = new EnumField("Convertion Type", selectedOption);
			enumField.Init(selectedOption); // Initialize the EnumField with the current value

			// Handle the value change
			enumField.RegisterValueChangedCallback(evt =>
			{
				selectedOption = (ConversionTypes)evt.newValue;
				//Debug.Log("Selected option: " + selectedOption);
			});

			root.Add(enumField);

			// Create an ObjectField for GameObject
			ObjectField gameObjectField = new ObjectField("Select GameObject")
			{
				objectType = typeof(GameObject),  // Specify that we want to select GameObjects
				allowSceneObjects = true          // Allow selection of scene objects
			};

			// Add the ObjectField to the root visual element
			root.Add(gameObjectField);

			// Create a label to display the selected GameObject
			selectedGameObjectLabel = new Label("Selected GameObject: None");
			root.Add(selectedGameObjectLabel);

			// Update the label when a GameObject is selected
			gameObjectField.RegisterValueChangedCallback(evt =>
			{
				selectedObject = evt.newValue as GameObject;
			});

			Button myButton = new Button()
			{
				text = "Convert!"
			};

			root.Add(myButton);
			// Register a click event handler for the button
			myButton.clicked += convert;
		}

		public void Update(){
			if(selectedObject != null){
				string str = "\n";
				MagicaCapsuleCollider1[] components = selectedObject.GetComponentsInChildren<MagicaCapsuleCollider1>(true);
				foreach(MagicaCapsuleCollider1 component in components){
					str += component.gameObject.name + " | " + component.GetType().Name;
					str+= "\n";
				}
				selectedGameObjectLabel.text =  $"Selected GameObject: {str}";
			}else {
				selectedGameObjectLabel.text = "Selected GameObject: None";
			}
		}
		private void convert(){
			if(selectedOption == ConversionTypes.Colliders || selectedOption == ConversionTypes.Full){
				Debug.Log($"selectedOption == ConversionTypes.Colliders | {selectedOption == ConversionTypes.Colliders}");
				MG1_MG2_collider();
			}
		}
		private void MG1_MG2_collider(){
			MagicaCapsuleCollider1[] colliders = selectedObject.GetComponentsInChildren<MagicaCapsuleCollider1>(true);
			foreach(MagicaCapsuleCollider1 collider in colliders){
				GameObject parentObject = collider.gameObject;
				
				MagicaCapsuleCollider2 addedCollider = parentObject.AddComponent<MagicaCapsuleCollider2>();
				MG_MG2Converter.convertCollider(collider,addedCollider);
			}
		}
	}
}