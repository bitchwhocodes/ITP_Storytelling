/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.IO;

namespace fs
{
	/**
	 * @brief class Utils contains helper functions for changing the coordinate system.
	 */
	public class Utils {
	
	    //! @brief transform the rotation from right-hand/left-hand to left-hand/right hand respectively
	    public static Quaternion ChangeCoordinateSystem(Quaternion rot) {
	    
	        return new Quaternion(rot.x, -rot.y, -rot.z, rot.w);
	    }
	
	    //! @brief transform the translation from right-hand/left-hand to left-hand/right hand respectively
	    public static Vector3 ChangeCoordinateSystem(Vector3 t) {
	    
	        return new Vector3(-t.x, t.y, t.z);
	    }
	
		/**
		 * Unfortunately AnimationUtility.CalculateTransformPath runs only in the Unity Editor
		 * and not in the built standalone application. Therefor we had to write our own implementation
		 * for this. If the unity editor is present, it just uses AnimationUtility.CalculateTransformPath().
		 * @param target_transform The target (end point) of the path
		 * @param root_transform The start point of the path
		 * @return The path as a string with '/' between the elements
		 */
		public static string CalculateTransformPath(Transform target_transform, Transform root_transform) {
		
	#if UNITY_EDITOR
			return AnimationUtility.CalculateTransformPath(target_transform, root_transform);
	#else
			// This is a simple replacement of AnimationUtility.CalculateTransformPath(). We have to do this
			// because the function is only available in the unity editor.
			if (target_transform == root_transform) {
				return "";
			}
			string relative_path = target_transform.name;
			Transform current_transform = target_transform.parent;
			while ((current_transform != null) && (current_transform != root_transform)) {
				relative_path = current_transform.name + "/" + relative_path;
				current_transform = current_transform.parent;
			}
			return relative_path;
	#endif
		}
	
		/**
		 * Adds all possible blendshape targets of the game object
		 * curGameObject and its sub-objects (recursively) to the
		 * list of target blendshapes.
		 */
		public static void GetGameObjectBlendshapes(GameObject curGameObject, ArrayList blendshape_infos) {
		
			if (blendshape_infos != null) blendshape_infos.Clear();

			// Iterate over game object itself and over children and add blendshapes
			Transform [] children = curGameObject.GetComponentsInChildren<Transform>();
			foreach (Transform child in children)
			{
				string transformPath = CalculateTransformPath(child, curGameObject.transform);
				SkinnedMeshRenderer meshRenderer = (SkinnedMeshRenderer)child.GetComponent(typeof(SkinnedMeshRenderer));
				if (meshRenderer != null)
				{
					if (meshRenderer.sharedMesh != null)
					{
						//Debug.Log("Number of blend shapes: " + meshRenderer.sharedMesh.blendShapeCount);
						for (int blend_shape_nr = 0; blend_shape_nr < meshRenderer.sharedMesh.blendShapeCount; blend_shape_nr++) {
							string blend_shape_name = meshRenderer.sharedMesh.GetBlendShapeName(blend_shape_nr);
							//Debug.Log("Path: '" + transformPath + "', blend shape: '" + blend_shape_name + "'");
							if (blendshape_infos != null) {
								BlendshapeInfo blendshape_info = new BlendshapeInfo();
								blendshape_info.m_path = transformPath;
								blendshape_info.m_name = blend_shape_name;
								blendshape_info.m_index = blend_shape_nr;
								blendshape_info.m_mesh_renderer = meshRenderer;
								blendshape_infos.Add (blendshape_info);
							}
						}
					}
				}
			}
		}
	
		/**
		 * Adds all possible transformation targets of the game object
		 * curGameObject and its sub-objects (recursively) to the
		 * list of target transformations.
		 */
		public static void GetGameObjectTransformations(GameObject curGameObject, ArrayList transformations) {
		
			if (transformations != null) transformations.Clear();
			
			// Iterate over game object itself and over children and add blendshapes
			Transform [] children = curGameObject.GetComponentsInChildren<Transform>();
			foreach (Transform child in children) {
			
				string transformPath = CalculateTransformPath(child, curGameObject.transform);
				if (transformations != null) {
					TransformationInformation transform_copy = new TransformationInformation();
					transform_copy.rotation = new Quaternion(child.rotation.x, child.rotation.y, child.rotation.z, child.rotation.w);
					transform_copy.localRotation = new Quaternion(child.localRotation.x, child.localRotation.y, child.localRotation.z, child.localRotation.w);
					transform_copy.position = new Vector3(child.position.x, child.position.y, child.position.z);
					transform_copy.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, child.localPosition.z);
					if (child.parent != null) {
						transform_copy.parentRotation = new Quaternion(child.parent.rotation.x, child.parent.rotation.y, child.parent.rotation.z, child.parent.rotation.w); 
						//Debug.Log("parent of child " + child.name + " is " + child.parent.name);
					} else {
						transform_copy.parentRotation = Quaternion.identity; 
						//Debug.Log("child " + child.name + " has no parent");
					}

					transform_copy.transform = child;
					transform_copy.transformPath = transformPath;
					transform_copy.transformName = child.name;
					transformations.Add(transform_copy);
				}
			}
		}
	
		/**
		 *	Evaluates the target blendshape values based on the state of a rig and a retargeting configuration.
		 *  If a blendshape is not affected by the retargeting then the value of this blendshape is null.
		 */
		public static BlendshapeValue [] EvaluateTargetBlendshapes(ClipRetargeting retargeting,
						                                             Rig rig,
						                                             RigState state,
						                                             ArrayList target_blendshapes) {
						                                             
			int n_target_blendshapes = target_blendshapes.Count;
			BlendshapeValue [] values = new BlendshapeValue[n_target_blendshapes];
			// We iterate over the targets and accumulate all sources to them
			for (int index = 0; index < n_target_blendshapes; index++) {
				double value = 0.0;
				int value_count = 0;
				for (int mapping_nr = 0; mapping_nr < retargeting.GetNumberOfBlendshapeMappings(); mapping_nr++) {
					string mapping_target = retargeting.GetBlendshapeMappingDestination(mapping_nr);
					if (!mapping_target.Equals(((BlendshapeInfo)target_blendshapes[index]).m_name)) {
						continue;
					}
					string mapping_src = retargeting.GetBlendshapeMappingSource(mapping_nr);
					int src_index = rig.ShapeIndex(mapping_src);
					if (src_index >= 0) {
						double mapping_weight = retargeting.GetBlendshapeMappingWeight(mapping_nr);
						value += state.BlendshapeCoefficient(src_index) * mapping_weight;
						value_count++;
					} else {
						Debug.Log("Could not find source blend shape '" + mapping_src);
					}
				} 
				// Apply the value for this target
				if (value_count > 0) {
					values[index] = new BlendshapeValue(value);
				}
			}
			return values;
		}
			
		/**
		 *	Evaluates the target transformation based on the state of a rig and a retargeting configuration.
		 *  If a transform is not affected by the retargeting then the value of the transform is null.
		 */
		public static TransformationValue [] EvaluateTargetTransformations(ClipRetargeting retargeting,
		                                                       Rig rig,
		                                                       RigState state,
		                                                       ArrayList target_transformations) {
		                                                       
			if (retargeting == null || rig == null || state == null || target_transformations == null) {
				Debug.LogError("cannot evaluat target transformations as one or more object is null");
				return null;
			}
			
			int n_target_transformations = target_transformations.Count;
			TransformationValue [] values = new TransformationValue[n_target_transformations];
			// We iterate over the target transformations and accumulate all sources to them
			for (int target_nr = 0; target_nr < target_transformations.Count; target_nr++) {
				
				// get original rotation and translation from tpose
				TransformationInformation tpose_joint = target_transformations[target_nr] as TransformationInformation;
				if (tpose_joint == null) {
					Debug.LogError("joint " + target_nr + " is null");
					continue;
				}
				Vector3 tpose_translation               = tpose_joint.localPosition;
				Quaternion tpose_local_rotation         = tpose_joint.localRotation;
				Quaternion tpose_parent_global_rotation = tpose_joint.parentRotation;
				
				Quaternion fs_joint_rotation_local_from_t_pose = Quaternion.identity;
				Vector3 fs_joint_translation_local_from_t_pose = new Vector3(0, 0, 0);
				
				// Sum the translation to apply
				int value_count_trans = 0;
				for (int mapping_nr = 0; mapping_nr < retargeting.GetNumberOfTranslationMappings(); mapping_nr++) {
					string mapping_target = retargeting.GetTranslationMappingDestination (mapping_nr);
					if (!mapping_target.Equals(tpose_joint.transformName)) {
						continue;
					}
					string mapping_src = retargeting.GetTranslationMappingSource (mapping_nr);
					int src_index = rig.BoneIndex (mapping_src);
					if (src_index >= 0) {
						double mapping_weight = retargeting.GetTranslationMappingWeight (mapping_nr);
						fs_joint_translation_local_from_t_pose += state.BoneTranslation (src_index) * (float)mapping_weight;
						value_count_trans++;
					} else {
						Debug.Log ("Could not find source index for '" + mapping_src + "'");
					}
				} 
				
				// Convert translation to global translation
				//Vector3 unity_joint_translation_local = fs_joint_translation_local_from_t_pose + tpose_translation;
				Vector3 unity_joint_translation_local = 
					Quaternion.Inverse(tpose_parent_global_rotation) * fs_joint_translation_local_from_t_pose
					+ tpose_translation;
						
				// Sum the rotations to apply
				int value_count_rot = 0;
				for (int mapping_nr = 0; mapping_nr < retargeting.GetNumberOfRotationMappings(); mapping_nr++) {
					string mapping_target = retargeting.GetRotationMappingDestination (mapping_nr);
					if (!mapping_target.Equals (tpose_joint.transformName)) {
						continue;
					}
					string mapping_src = retargeting.GetRotationMappingSource (mapping_nr);
					int src_index = rig.BoneIndex (mapping_src);
					if (src_index >= 0) {
						double mapping_weight = retargeting.GetRotationMappingWeight (mapping_nr);
						
						// use slerp for weighting
						fs_joint_rotation_local_from_t_pose = Quaternion.Slerp (Quaternion.identity, state.BoneRotation (src_index), (float)mapping_weight);
						// TODO: here we should accumulate if there are more than one sources (like with blendshapes)
						value_count_rot++;
					} else {
						Debug.Log ("Could not find source rotation for '" + mapping_src);
					}
				} 
				
				// Convert to local unity rotation
				Quaternion unity_joint_rotation_local = 
						Quaternion.Inverse(tpose_parent_global_rotation) 
					 		* fs_joint_rotation_local_from_t_pose 
							* tpose_parent_global_rotation
							* tpose_local_rotation; // The initial local rotation;

				if (value_count_trans > 0 || value_count_rot > 0) {
					values[target_nr] = new TransformationValue(unity_joint_rotation_local, unity_joint_translation_local);
				}
			}
			
			return values;
		}


		//! @returns The directory of path if it exists, "" otherwise.
		public static string GetExistingDirectoryName(string path) {
		
			string directory = "";
			if (path != "") 
			{
				directory = Path.GetDirectoryName(path);
				if (!Directory.Exists(directory)) directory = "";
			}
			return directory;
		}

		//! @returns True if the path points to a file in the assets (does not check if the file/directory exists).
		public static bool inAssets(string path) {
		
			return path.StartsWith(Application.dataPath);
		}

		//! @returns The assets path of a path, assuming the input is 
		public static string AssetPath(string path) {
		
			if (path.StartsWith (Application.dataPath)) {
				return "Assets/" + path.Remove (0, Application.dataPath.Length + 1);
			} else if (path.StartsWith ("Assets/")) {
				return path;
			} else {
				Debug.LogError ("path " + path + " is not in the assets directory");
				return path;
			}
		}
	}

}
