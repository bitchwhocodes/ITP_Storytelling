/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.IO;

[CanEditMultipleObjects]
[CustomEditor(typeof(fs.Faceshift))]

/** 
 * @brief class FaceshiftEditor contains the unity editor user interface.
 */ 
public class FaceshiftEditor : Editor
{
    private fs.Faceshift fs_obj;
    private bool showTranslations = true;
    private bool showRotations = true;
    private bool showBlendshapes = true;
    private Texture logoTexture;
	 
    void Awake() 
    {
        fs_obj = (fs.Faceshift)target;
        logoTexture = (Texture)Resources.Load("logo-editor");  
	}
	 
    /**
     * Indentation for structuring the GUI.
     * Since EditorGUI.indentLevel++ and -- do not work for Buttons, we do this
     * with Space()
     */
    private void startIndent()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
    }

    /**
     * Indentation for structuring the GUI.
     * Since EditorGUI.indentLevel++ and -- do not work for Buttons, we do this
     * with Space()
     */
    private void endIndent()
    {
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
	{
		// (always re)initialize the fs plugin
		fs_obj.Init();

		GUIStyle mainTitlesStyle = new GUIStyle (EditorStyles.label);
		mainTitlesStyle.fontStyle = FontStyle.Bold;

		fs_obj.UpdateTargetBlendshapesAndTransformations();

		///////////////
		/// Inform on faceshift trial
		///
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("If you don't have faceshift studio yet, get it now:"
		                           , GUILayout.Height(25), GUILayout.Width(350));

		if (GUILayout.Button ("Get faceshift studio", GUILayout.Width(150))) {
			Application.OpenURL("http://www.faceshift.com/get-faceshift/");
		}
		EditorGUILayout.EndVertical();
		EditorGUILayout.Space();

		///////////////
		/// Input
		///
		EditorGUILayout.LabelField ("Input", mainTitlesStyle);

		EditorGUILayout.BeginHorizontal ();
 
		EditorGUILayout.BeginVertical (GUILayout.Width (250), GUILayout.ExpandWidth (false));
		startIndent ();

		if (fs_obj.m_clip != null) {  
			EditorGUILayout.LabelField (Path.GetFileName (fs_obj.m_clip_path), GUILayout.Height (15), GUILayout.Width(100), GUILayout.ExpandWidth(true));
			string info = "" + fs_obj.m_clip.NumStates () + " frames";
			if (fs_obj.m_clip.NumStates () > 0) {
					double duration = fs_obj.m_clip [fs_obj.m_clip.NumStates () - 1].Timestamp () - fs_obj.m_clip [0].Timestamp ();
					info += ", duration: " + (duration / 1000).ToString("0.00") + " seconds";
			}
			EditorGUILayout.LabelField (info, GUILayout.Width (250));
		} else {
			EditorGUILayout.LabelField ("No clip loaded", GUILayout.Width (250));
		}

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Load faceshift clip (fsb file)", GUILayout.Width (250))) {
			string path = EditorUtility.OpenFilePanel ("Open faceshift fsb file", fs.Utils.GetExistingDirectoryName (fs_obj.m_clip_path), "fsb");

			if (path.Length != 0) {
				fs_obj.LoadFSB (path);
			}
		}
		if (GUILayout.Button ("Clear", GUILayout.Width(50))) {
			fs_obj.Reset();
		}
		EditorGUILayout.EndHorizontal ();

		endIndent ();
		EditorGUILayout.EndVertical ();

		// faceshift logo
		GUILayout.FlexibleSpace ();

		if (GUILayout.Button (logoTexture, EditorStyles.label)) {
			Application.OpenURL ("http://www.faceshift.com/");
		}

		EditorGUILayout.Space ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		///////////////
		/// Retargeting
		///
		EditorGUILayout.LabelField ("Retargeting", mainTitlesStyle);
		startIndent ();

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Clear Retargeting", GUILayout.Width (150))) {
			fs_obj.ClearRetargeting ();
		} 

		if (GUILayout.Button ("Import faceshift Retargeting", GUILayout.Width (200))) {
			string path = EditorUtility.OpenFilePanel("Open faceshift fst file", "", "fst");
			if (path.Length != 0) {
				fs_obj.LoadRetargetingFromFile(path);
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUI.enabled = (fs_obj.Retargeting() == null) || (!fs_obj.Retargeting().IsEmpty());
		if (GUILayout.Button ("Save Retargeting Asset", GUILayout.Width (150))) {
			string current_asset_path = AssetDatabase.GetAssetPath(fs_obj.m_RetargetingAsset);
			string directory = fs.Utils.GetExistingDirectoryName(current_asset_path);
			string filename = Path.GetFileName(current_asset_path);
			string path = EditorUtility.SaveFilePanelInProject("Save to faceshift retargeting asset", filename, "bytes", "", directory);

			if (path.Length != 0) {
				fs_obj.SaveRetargetingToAsset(path);
			}
		}
		GUI.enabled = true;
		
		EditorGUI.BeginChangeCheck();
		TextAsset new_retargeting_asset = EditorGUILayout.ObjectField (fs_obj.m_RetargetingAsset, typeof(TextAsset), false, GUILayout.ExpandWidth(true)) as TextAsset;
		if (EditorGUI.EndChangeCheck()) {
			if (new_retargeting_asset != null) {
				fs_obj.LoadRetargetingFromAsset(new_retargeting_asset);
			} else {
				fs_obj.m_RetargetingAsset = null;
				fs_obj.ClearRetargeting();
			}
		}
		
		EditorGUILayout.EndHorizontal ();
		if (GUILayout.Button ("Cleanup", GUILayout.Width (150))) {
			fs_obj.CleanupRetargeting ();
		} 
		
		EditorGUILayout.Separator ();

		////////////////////////
		/// Translations
		showTranslations = EditorGUILayout.Foldout (showTranslations, "Translations");

		if (showTranslations) {
			startIndent ();

			string [] target_transformation_names = fs_obj.TargetTransformationNames ();

			for (int i = 0; fs_obj.Retargeting() != null && i < fs_obj.Retargeting().GetNumberOfTranslationMappings(); i++) {
					////////
					/// Get selected source and target
					string mapping_source = fs_obj.Retargeting().GetTranslationMappingSource (i);

					if (mapping_source == null) {
							mapping_source = "";
					}

					string mapping_destination = fs_obj.Retargeting().GetTranslationMappingDestination (i);

					if (mapping_destination == null) {
							mapping_destination = "";
					}

					double value = fs_obj.Retargeting().GetTranslationMappingWeight (i);
					int selected_source = -1;
					string [] joint_names = fs_obj.m_fs_Rig.GetBoneNames ();

					if ((mapping_source.Length != 0) && (fs_obj.m_fs_Rig != null)) {
							for (int j = 0; j < joint_names.Length; j++) {
									if (joint_names [j].Equals (mapping_source)) {
											selected_source = j;
									}
							}
					}

					int selected_target = -1;

					if (mapping_destination.Length != 0) {
							for (int j = 0; j < target_transformation_names.Length; j++) {
									if (mapping_destination.Equals (target_transformation_names [j])) {
											selected_target = j;
									}
							}
					}

					EditorGUILayout.BeginHorizontal ();

					////////
					/// Source selection
					int selected_new_source = EditorGUILayout.Popup (selected_source, joint_names, GUILayout.Width (200));
					string mapping_source_new = "";

					if ((selected_new_source >= 0) && (selected_new_source < joint_names.Length)) {
							mapping_source_new = joint_names [selected_new_source];

							if (selected_target < 0) {
									// No target selected so far
									// Check if there is a target with the same name as the source
									for (int j = 0; j < target_transformation_names.Length; j++) {
											if (mapping_source_new.Equals (target_transformation_names [j])) {
													selected_target = j;
											}
									}
							}
					}

					////////
					/// Target selection
					int selected_new_target = EditorGUILayout.Popup (
                                  selected_target,
								  target_transformation_names,
                                  GUILayout.Width (200));

					string mapping_destination_new = "";

					if ((selected_new_target >= 0) && (selected_new_target < target_transformation_names.Length)) {
							mapping_destination_new = target_transformation_names [selected_new_target];
					}

					/////////
					/// Weight
					double value_new = EditorGUILayout.FloatField ((float)value, GUILayout.Width (70));

					/////////
					/// Remove Button
					if (GUILayout.Button ("-", GUILayout.Width (20))) {
						fs_obj.Retargeting().RemoveTranslationMapping (mapping_source, mapping_destination);
					} else if ((!mapping_source.Equals (mapping_source_new)) ||
							(!mapping_destination.Equals (mapping_destination_new)) ||
							(value != value_new)) {
						fs_obj.Retargeting().UpdateTranslationMapping (i, mapping_source_new,
                                                      mapping_destination_new, value_new);
					}

					EditorGUILayout.EndHorizontal ();
			}

			if (GUILayout.Button ("Add translation mapping", GUILayout.Width (502))) {
					fs_obj.Retargeting().AddTranslationMapping ("", "", 1.0);
			}

			endIndent ();
		}

		////////////////////////
		/// Rotations
		showRotations = EditorGUILayout.Foldout (showRotations, "Joint Rotations");

		if (showRotations) {
			startIndent ();

			string [] target_transformation_names = fs_obj.TargetTransformationNames ();

			for (int i = 0; fs_obj.Retargeting() != null && i < fs_obj.Retargeting().GetNumberOfRotationMappings(); i++) {
					////////
					/// Get selected source and target
					string mapping_source = fs_obj.Retargeting().GetRotationMappingSource (i);

					if (mapping_source == null) {
							mapping_source = "";
					}

					string mapping_destination = fs_obj.Retargeting().GetRotationMappingDestination (i);

					if (mapping_destination == null) {
							mapping_destination = "";
					}

					double value = fs_obj.Retargeting().GetRotationMappingWeight (i);
					int selected_source = -1;
					string [] joint_names = fs_obj.m_fs_Rig.GetBoneNames ();

					if (fs_obj.m_fs_Rig != null) {
							for (int j = 0; j < joint_names.Length; j++) {
									if (joint_names [j].Equals (mapping_source)) {
											selected_source = j;
									}
							}
					}

					int selected_target = -1;

					for (int j = 0; j < target_transformation_names.Length; j++) {
							if (mapping_destination.Equals (target_transformation_names [j])) {
									selected_target = j;
							}
					}

					EditorGUILayout.BeginHorizontal ();

					////////
					/// Source
					int selected_new_source = EditorGUILayout.Popup (selected_source, joint_names, GUILayout.Width (200));
					string mapping_source_new = "";

					if ((selected_new_source >= 0) && (selected_new_source < joint_names.Length)) {
							mapping_source_new = joint_names [selected_new_source];

							if (selected_target < 0) {
									// No target selected so far
									// Check if there is a target with the same name as the source
									for (int j = 0; j < target_transformation_names.Length; j++) {
											if (mapping_source_new.Equals (target_transformation_names [j])) {
													selected_target = j;
											}
									}
							}
					}

					////////
					/// Target
					int selected_new_target = EditorGUILayout.Popup (
                                  selected_target,
									target_transformation_names,
                                  GUILayout.Width (200));

					string mapping_destination_new = "";

					if ((selected_new_target >= 0) && (selected_new_target < target_transformation_names.Length)) {
							mapping_destination_new = target_transformation_names [selected_new_target];
					}

					/////////
					/// Weight
					double value_new = EditorGUILayout.FloatField ((float)value, GUILayout.Width (70));

					/////////
					/// Remove Button
					if (GUILayout.Button ("-", GUILayout.Width (20))) {
						fs_obj.Retargeting().RemoveRotationMapping (mapping_source, mapping_destination);
					} else if ((!mapping_source.Equals (mapping_source_new)) ||
							(!mapping_destination.Equals (mapping_destination_new)) ||
							(value != value_new)) {
							fs_obj.Retargeting().UpdateRotationMapping (i, mapping_source_new, mapping_destination_new, value_new);
					}

					EditorGUILayout.EndHorizontal ();
			}

			if (GUILayout.Button ("Add rotation mapping", GUILayout.Width (502))) {
					fs_obj.Retargeting().AddRotationMapping ("", "", 1.0);
			}

			endIndent ();
		}

		////////////////////////
		/// Blend Shapes
		showBlendshapes = EditorGUILayout.Foldout (showBlendshapes, "Blend shapes");

		if (showBlendshapes) {
			startIndent ();

			// get all target blendshape names
			string [] target_blendshape_names = fs_obj.TargetBlendshapeNames ();
			string [] shape_names = fs_obj.m_fs_Rig.GetShapeNames ();

			for (int i = 0; fs_obj.Retargeting() != null && i < fs_obj.Retargeting().GetNumberOfBlendshapeMappings(); i++) {
					////////
					/// Get selected source and target
					string mapping_source = fs_obj.Retargeting().GetBlendshapeMappingSource (i);

					if (mapping_source == null) {
							mapping_source = "";
					}

					string mapping_destination = fs_obj.Retargeting().GetBlendshapeMappingDestination (i);

					if (mapping_destination == null) {
							mapping_destination = "";
					}

					double value = fs_obj.Retargeting().GetBlendshapeMappingWeight (i);
					int selected_source = -1;
    
					if (fs_obj.m_fs_Rig != null) {
							for (int j = 0; j < shape_names.Length; j++) {
									if (shape_names [j].Equals (mapping_source)) {
											selected_source = j;
									}
							}
					}

					int selected_target = -1;

					for (int j = 0; j < target_blendshape_names.Length; j++) {
							if (mapping_destination.Equals (target_blendshape_names [j])) {
									selected_target = j;
							}
					}

					EditorGUILayout.BeginHorizontal ();

					////////
					/// Source
					int selected_new_source = EditorGUILayout.Popup (selected_source, shape_names, GUILayout.Width (200));
					string mapping_source_new = "";

					if ((selected_new_source >= 0) && (selected_new_source < shape_names.Length)) {
							mapping_source_new = shape_names [selected_new_source];

							if (selected_target < 0) {
									// No target selected so far
									// Check if there is a target with the same name as the source
									for (int j = 0; j < target_blendshape_names.Length; j++) {
											if ((mapping_source_new.Equals (target_blendshape_names [j]))
													|| ((target_blendshape_names [j] != null)
											// We also check for the name to just end with ".blendshapename" because with many meshes
											// like our macaw this is the case. This way, the automatic mapping still works.
													&& (target_blendshape_names [j].EndsWith ("." + mapping_source_new)))) {
													selected_target = j;
											}
									}
							}
					}

					////////
					/// Target
					int selected_new_target = EditorGUILayout.Popup (
                                  selected_target,
								  target_blendshape_names,
                                  GUILayout.Width (200));

					string mapping_destination_new = "";

					if ((selected_new_target >= 0) && (selected_new_target < target_blendshape_names.Length)) {
							mapping_destination_new = target_blendshape_names [selected_new_target];
					}

					/////////
					/// Weight
					double value_new = EditorGUILayout.FloatField ((float)value, GUILayout.Width (70));

					/////////
					/// Remove Button
					if (GUILayout.Button ("-", GUILayout.Width (20))) {
						fs_obj.Retargeting().RemoveBlendshapeMapping (mapping_source, mapping_destination);
					} else if ((!mapping_source.Equals (mapping_source_new)) ||
							(!mapping_destination.Equals (mapping_destination_new)) ||
							(value != value_new)) {
							fs_obj.Retargeting().UpdateBlendshapeMapping (i, mapping_source_new, mapping_destination_new, value_new);
					}

					EditorGUILayout.EndHorizontal ();
			}

			if (GUILayout.Button ("Add blend shape mapping", GUILayout.Width (502))) {
					fs_obj.Retargeting().AddBlendshapeMapping ("", "", 100.0);
			}

			endIndent ();
		}

		endIndent ();

		EditorGUILayout.Space ();

		////////////
		// Storing of t-pose
		//
		EditorGUILayout.LabelField ("T-Pose", mainTitlesStyle);
		startIndent ();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("Clear T-Pose", GUILayout.Width (150))) {
			fs_obj.ClearTPose();
		} 

		if (GUILayout.Button ("Set T-Pose", GUILayout.Width (150))) {
			fs_obj.SetTPose();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal ();
		GUI.enabled = (fs_obj.TPose() != null);
		if (GUILayout.Button ("Save T-Pose Asset", GUILayout.Width (150))) {
			string current_asset_path = AssetDatabase.GetAssetPath(fs_obj.m_TPoseAsset);
			string directory = fs.Utils.GetExistingDirectoryName(current_asset_path);
			string filename = Path.GetFileName(current_asset_path);
			string path = EditorUtility.SaveFilePanelInProject("Save to faceshift t-pose asset", filename, "bytes", "", directory);
			
			if (path.Length != 0) {
				fs_obj.SaveTPoseToAsset (path);
			}
		}
		GUI.enabled = true;
		EditorGUI.BeginChangeCheck();
		TextAsset new_tpose_asset = EditorGUILayout.ObjectField (fs_obj.m_TPoseAsset, typeof(TextAsset), false, GUILayout.ExpandWidth(true)) as TextAsset;
		if (EditorGUI.EndChangeCheck()) {
			if (new_tpose_asset != null) {
				fs_obj.LoadTPoseFromAsset(new_tpose_asset);
			} else {
				fs_obj.m_TPoseAsset = null;
				fs_obj.ClearTPose();
			}
		}

		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.Separator();
		endIndent ();

        EditorGUILayout.LabelField("Output", mainTitlesStyle);
        startIndent();
        GUI.enabled = (fs_obj.m_clip != null);

        if (GUILayout.Button("Create animation", GUILayout.Width(180))) {
            string path = EditorUtility.SaveFilePanelInProject("Save clip as", "", "anim", "");

            if (path.Length != 0) {
                fs_obj.SaveClipAsAnim(path);
            }
        }

        GUI.enabled = true;
        endIndent();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}
