using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicaCapsuleCollider1 = MagicaCloth.MagicaCapsuleCollider;
using Magica1Axis = MagicaCloth.MagicaCapsuleCollider.Axis;
using Magica2Axis = MagicaCloth2.MagicaCapsuleCollider.Direction;
using MagicaCapsuleCollider2 = MagicaCloth2.MagicaCapsuleCollider;
using MagicaPlaneCollider1 = MagicaCloth.MagicaPlaneCollider;
using MagicaPlaneCollider2 = MagicaCloth2.MagicaPlaneCollider;

namespace MagicaConverter{
	public static class MG_MG2Converter
	{

		public static void convertCollider(MagicaCapsuleCollider1 mg1_collider, MagicaCapsuleCollider2 newCollider){
			float length= mg1_collider.Length;
			float startRadius = mg1_collider.StartRadius;
			float endRadius = mg1_collider.EndRadius;
			float average = (startRadius+endRadius)/2f;
			float centerMovement =0;

			if(startRadius>endRadius){
				centerMovement = -(startRadius-average);
			}else if(endRadius>startRadius){
				centerMovement = endRadius - average;
			}
			
			//radius is actually diameter, MG2 handles the radius differently.
			//In MG2 the lenght is fixed at the edge of the sphere and grows from the edge to the center point of the collider.
			//In MG the lenght is fixes at the center of the sphere and grows form the center out.
			//MG2 also doubles the lenght
			float newLength = (length+startRadius/2f+endRadius/2f)*2;
			Debug.Log($"NEW:{newLength}");
			if(newLength>2){
				Debug.LogWarning("Collider too long value will be clamped");
			}

			Magica1Axis axis = mg1_collider.AxisMode;
			newCollider.radiusSeparation = true;
			newCollider.SetSize(endRadius,startRadius,Mathf.Clamp(newLength,0f,2f));
			newCollider.center = new Vector3(mg1_collider.Center.x,mg1_collider.Center.y,mg1_collider.Center.z);

			//Because of what written earlier changing the radius in MG moved the center point of the collider depending on the axis
			switch (axis)
			{
				case Magica1Axis.X:
					newCollider.center.x += centerMovement;
					newCollider.direction = Magica2Axis.X;
					break;
				case Magica1Axis.Y:
					newCollider.center.y += centerMovement;
					newCollider.direction = Magica2Axis.Y;
					break;
				case Magica1Axis.Z:
					newCollider.center.z += centerMovement;					
					newCollider.direction = Magica2Axis.Z;
					break;
				default:
					Debug.LogError("Error converting axis, defaulting to X");
					newCollider.center.x += centerMovement;
					newCollider.direction = Magica2Axis.X;
					break;
			}
		}

		public static void convertColliders(GameObject selectedObject){
			MagicaCapsuleCollider1[] colliders = selectedObject.GetComponentsInChildren<MagicaCapsuleCollider1>(true);
			foreach(MagicaCapsuleCollider1 collider in colliders){
				GameObject parentObject = collider.gameObject;
				
				MagicaCapsuleCollider2 addedCollider = parentObject.AddComponent<MagicaCapsuleCollider2>();
				MG_MG2Converter.convertCollider(collider,addedCollider);
			}
		}

		public static void convertCollider(MagicaPlaneCollider1 mg1_collider, MagicaPlaneCollider2 newCollider){

		}
	}
}

