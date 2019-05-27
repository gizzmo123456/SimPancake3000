using System;
using AMS_Helpers;
using UnityEditor;
using UnityEngine;


[Serializable]
[CustomPropertyDrawer(typeof(MinMax))]
public class MinMaxDrawer : PropertyDrawer
{

	public override float GetPropertyHeight(SerializedProperty property, GUIContent lable)
	{
		return 35f;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent lable)
	{

		//Start the property

		EditorGUI.BeginProperty(position, lable, property);

		position.y += (position.height / 2) - 7;

		//Draw Lable!
		position = EditorGUI.PrefixLabel(position, lable);

		position.y -= (position.height / 2) - 7;

		//Add indent
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		//Calculate Rects
		Rect aRect = new Rect(position.x, position.y, (position.width / 3) - 2, 15);
		EditorGUI.LabelField(aRect, "Min");
		aRect.x += (position.width / 3) + 1;
		EditorGUI.LabelField(aRect, "Max");
		aRect.x += (position.width / 3) + 1;
		EditorGUI.LabelField(aRect, "Current");
		aRect.x = position.x;           //reset x
		aRect.y += 16;  //drop one line
		EditorGUI.PropertyField(aRect, property.FindPropertyRelative("min"), GUIContent.none); //Vector3Field(vectRect, "min/max/current", v);
		aRect.x += (position.width / 3) + 1;
		EditorGUI.PropertyField(aRect, property.FindPropertyRelative("max"), GUIContent.none); //Vector3Field(vectRect, "min/max/current", v);
		aRect.x += (position.width / 3) + 1;
		EditorGUI.PropertyField(aRect, property.FindPropertyRelative("current"), GUIContent.none); //Vector3Field(vectRect, "min/max/current", v);
		aRect.x += (position.width / 3) + 1;

		//Set indent back
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();

	}

}