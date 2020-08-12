using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GuidedPathSegrmentController))]
public class CustomPathBinding : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		GuidedPathSegrmentController script = (GuidedPathSegrmentController)target;
		if(GUILayout.Button("Bind Path to Gates"))
		{
			script.BindPathToGates();
		}
	}
}
