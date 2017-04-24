using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class UChromaKey_DS_unlit_editor : MaterialEditor {

	private UnityEngine.Object[] materials;

	public override void Awake ()
	{
		base.Awake();
		materials = new UnityEngine.Object[1];
		materials[0] = serializedObject.targetObject;
	}

	public override void OnInspectorGUI ()
	{
		Vector2 shift = Vector2.zero;
		Vector2 multiplier = Vector2.one;
		bool flipH = false;
		bool flipV = false;
		serializedObject.Update ();
		materials[0] = serializedObject.targetObject;
		var theShader = serializedObject.FindProperty ("m_Shader"); 
		if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null)
		{
			EditorGUI.BeginChangeCheck();

			foreach(MaterialProperty mProp in GetMaterialProperties(materials))
			{
				if (mProp.name == "_uvDefX")
					break;
				ShaderProperty(mProp,mProp.displayName);
				if (mProp.name == "_uvShiftMulti")
				{
					mProp.vectorValue = new Vector4 (Mathf.Clamp01(mProp.vectorValue.x),
					                                 Mathf.Clamp01(mProp.vectorValue.y),
					                                 Mathf.Clamp01(mProp.vectorValue.z),
					                                 Mathf.Clamp01(mProp.vectorValue.w));
					shift = new Vector2(mProp.vectorValue.x,mProp.vectorValue.y);
					multiplier = new Vector2(mProp.vectorValue.z,mProp.vectorValue.w);
				}
				else if (mProp.name == "_flipHorizontal")
					flipH = mProp.floatValue == 0;
				else if (mProp.name == "_flipVertical")
					flipV = mProp.floatValue == 0;

				GUILayout.Space(4);
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				for (int i = 0; i < materials.Length; i++)
					UChromaKey.SetShiftAndMultiplier((Material)materials[i],shift,multiplier,flipH,flipV);
				PropertiesChanged ();
			}
		}
	}
}
