using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using MagicaCapsuleCollider1 = MagicaCloth.MagicaCapsuleCollider;
using Magica1Axis = MagicaCloth.MagicaCapsuleCollider.Axis;
using Magica2Axis = MagicaCloth2.MagicaCapsuleCollider.Direction;
using MagicaCapsuleCollider2 = MagicaCloth2.MagicaCapsuleCollider;

public class MagicaConverterWindow : EditorWindow
{
	private GameObject selectedObject;
	private Label selectedGameObjectLabel;
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
        myButton.clicked += OnButtonClick;
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

	private void OnButtonClick(){
		MagicaCapsuleCollider1[] components = selectedObject.GetComponentsInChildren<MagicaCapsuleCollider1>(true);
			foreach(MagicaCapsuleCollider1 component in components){
				GameObject parentObject = component.gameObject;
				float length= component.Length;
				float startRadius = component.StartRadius;
				float endRadius = component.EndRadius;
				float average = (startRadius+endRadius)/2f;
				float newLength = (length>0.015f)? (length+average)*2f : length*2f;
				if(newLength>2){
					Debug.LogWarning("Collider too long value will be clamped");
				}
				Magica1Axis axis = component.AxisMode;
				MagicaCapsuleCollider2 addedCollider = parentObject.AddComponent<MagicaCapsuleCollider2>();
				addedCollider.radiusSeparation = true;
				addedCollider.SetSize(endRadius,startRadius,Mathf.Clamp(newLength,0f,2f));
				addedCollider.center = new Vector3(component.Center.x,component.Center.y,component.Center.z);
				switch (axis)
				{
					case Magica1Axis.X:
						if(startRadius>endRadius && length > (startRadius-average)){
							addedCollider.center.x += -(startRadius-average);
						}else if(endRadius>startRadius && length > (endRadius - average) ){
							addedCollider.center.x += endRadius - average;
						}
						addedCollider.direction = Magica2Axis.X;
						break;
					case Magica1Axis.Y:
						if(startRadius>endRadius && length > (startRadius-average)){
							addedCollider.center.y += -(startRadius-average);
						}else if(endRadius>startRadius && length > (endRadius - average)){
							addedCollider.center.y += endRadius - average;
						}
						addedCollider.direction = Magica2Axis.Y;
						break;
					case Magica1Axis.Z:
						if(startRadius>endRadius && length > (startRadius-average)){
							addedCollider.center.z += -(startRadius-average);
						}else if(endRadius>startRadius && length > (endRadius - average)){
							addedCollider.center.z += endRadius - average;
						}					
						addedCollider.direction = Magica2Axis.Z;
						break;
					default:
						Debug.LogError("Error converting axis, defaulting to X");
						addedCollider.direction = Magica2Axis.X;
						break;
				}
			}
	}
}