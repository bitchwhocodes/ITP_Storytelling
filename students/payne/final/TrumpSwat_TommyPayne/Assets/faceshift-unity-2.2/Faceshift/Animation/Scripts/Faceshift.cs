/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Threading;
using System.IO;

namespace fs {

	/**
	 * @brief This is the main class used for the faceshift retargeting. Simply drag this script onto your game object.
	 */
	public class Faceshift : CharacterRetargeting {
	
	    //! The source rig
		public Rig m_fs_Rig = null;
		
	    //! To make sure only one thread accesses the list at a time
	    private Mutex mutexOnTargetBlendshapeList = null;
	
	    //! The animation clip (source) we want to retarget
	    public Clip m_clip = null;
	
		//! The animation clip path.
		public string m_clip_path = "";
	
	    //! To make sure we do not warn the user of a changed rig if there was no rig before
		private bool cachedRigLoaded = false;
	
		//! Flag whether to normalize the headpose when creating animation clips
		public bool m_normalize_headpose = false;
		
	    public Faceshift()
	    {
			mutexOnTargetBlendshapeList = new Mutex();
	        m_fs_Rig = new Rig();
	        cachedRigLoaded = LoadCachedSourceRig();
	    }
	
		public void Reset() {
	
			m_clip = null;
			m_clip_path = "";
		}
	
		//! Init function initializing the retargeting, clip, and tpose (can be called any amount of times)
		new public void Init() {
			
			// check for clips
			if (m_clip_path != "" && m_clip == null) {
				LoadFSB(m_clip_path);
				// no clip, so the clip file is invalid
				if (m_clip == null) m_clip_path = "";
			}
	
			if (m_RetargetingAsset != null && (m_Retargeting == null || m_Retargeting.IsEmpty())) {
				if (!LoadRetargetingFromAsset(m_RetargetingAsset)) {
					m_RetargetingAsset = null;
				}
			}
			
			if (m_TPoseAsset != null && m_TPose == null) {
				LoadTPoseFromAsset(m_TPoseAsset);
				// no tpose, so the tpose asset is invalid
				if (m_TPose == null) m_TPoseAsset = null;
			}
		}
	
		public void ClearRetargeting() {
			m_Retargeting = new ClipRetargeting();
			m_RetargetingAsset = null;
			ClearCachedRig();
		}
	
		private void ClearCachedRig() {
			m_fs_Rig = new Rig();
		}
		
		private bool IsSourceRigDifferent(Rig newRig) {
			return !newRig.Equals(m_fs_Rig);
		}
		
	    /**
	     * @brief Loads an FST file from faceshift which contains the retargeting information
	     * @param[in] path The path to the file
	     * @param onlyRig Should only the rig be loaded (true) or also the mapping (false)
	     */
	    public bool LoadRetargetingFromFile(string path)
	    {
	        Rig new_rig = new Rig();
			if (!new_rig.LoadFromFST(path)) return false;
	
			if (IsSourceRigDifferent(new_rig)) {
	            if (cachedRigLoaded && !SourceRigChangedAskToContinue()) {
	                // The user wants to cancel
	                return false;
	            }
	
	            // Apply new rig
	            m_fs_Rig = new_rig;
	
	            // Cache new rig
	            SaveSourceRig();
	        }
	
	    	ClipRetargeting retargeting = ClipRetargeting.Load(path);
			if (retargeting != null && !retargeting.IsEmpty()) {
				m_Retargeting = retargeting;
				m_RetargetingAsset = null; // clear asset as we use a file
				return true;
			} else {
				return false;
			}   
	    }
	
		/**
	     * @brief Loads an FST retargeting file from faceshift which contains the retargeting information
	     * @param[in] path The path to the file
	     * @param onlyRig Should only the rig be loaded (true) or also the mapping (false)
	     */
		new public bool LoadRetargetingFromAsset(TextAsset asset) {
		
			Rig new_rig = new Rig();
			if (!new_rig.LoadFromFST(asset.bytes)) return false;
			
			if (IsSourceRigDifferent(new_rig)) {
				if (cachedRigLoaded && !SourceRigChangedAskToContinue()) {
					// The user wants to cancel
					return false;
				}
				
				// Apply new rig
				m_fs_Rig = new_rig;
				
				// Cache new rig
				SaveSourceRig();
			}
			 
			return base.LoadRetargetingFromAsset(asset);
		}
	
	    //! @brief Saves the retargeting (including the source rig) into a FST file
	    public void SaveRetargetingToFile(string path) {
	    
	        if (m_Retargeting.Save(path, m_fs_Rig)) {
				m_RetargetingAsset = null; // clear asset as we use file
			}
	    }
	
	#if UNITY_EDITOR
		public void SaveRetargetingToAsset(string path) {
		
			if (m_Retargeting.Save(path, m_fs_Rig)) {
				AssetDatabase.Refresh();
	
				string asset_path = Utils.AssetPath(path);
				Debug.Log ("loading asset " + asset_path);
				m_RetargetingAsset = AssetDatabase.LoadAssetAtPath(asset_path, typeof(TextAsset)) as TextAsset;
				if (m_RetargetingAsset == null) {
					Debug.LogError ("no retargeting asset at path " + asset_path);
				}
			}
		}
	#endif
	
		public void CleanupRetargeting() {
		
			int index = 0;
			while (index < m_Retargeting.GetNumberOfBlendshapeMappings()) {
				if (m_fs_Rig.ShapeIndex(m_Retargeting.GetBlendshapeMappingSource(index)) < 0 ||
					    !HasTargetBlendshapeName(m_Retargeting.GetBlendshapeMappingDestination(index))) {
						m_Retargeting.RemoveBlendshapeMapping(index);
				} else index++;
			}
			index = 0;
			while (index < m_Retargeting.GetNumberOfRotationMappings()) {
				if (m_fs_Rig.BoneIndex(m_Retargeting.GetRotationMappingSource(index)) < 0 ||
				    !HasTargetBoneName(m_Retargeting.GetRotationMappingDestination(index))) {
					m_Retargeting.RemoveRotationMapping(index);
				} else index++;
			}
			index = 0;
			while (index < m_Retargeting.GetNumberOfTranslationMappings()) {
				if (m_fs_Rig.BoneIndex(m_Retargeting.GetTranslationMappingSource(index)) < 0 ||
				    !HasTargetBoneName(m_Retargeting.GetTranslationMappingDestination(index))) {
					m_Retargeting.RemoveTranslationMapping(index);
				} else index++;
			}
		}
		
	    //! @brief Saves (caches) the faceshift rig (source rig)
	    public void SaveSourceRig() {
	    
	        m_fs_Rig.SaveToFST("Cached_fs_rig.fst");
	        cachedRigLoaded = true;
	    }
	
	    //! @brief Loads the faceshift rig (source rig) from the cache file
	    public bool LoadCachedSourceRig() {
	        return m_fs_Rig.LoadFromFST("Cached_fs_rig.fst");
	    }
	
	    //! @brief Loads a faceshift clip
	    public void LoadFSB(string path) {
	    
			if (!File.Exists (path)) {
				#if UNITY_EDITOR
				EditorUtility.DisplayDialog("Clip loading failed", "File " + path + " does exist", "Ok");
				#endif
				return;
			}
			
	        Clip new_clip = FsbReader.Read(path);
			if (new_clip == null) {
				Debug.LogError("could not read clip from file " + path);
				#if UNITY_EDITOR
				EditorUtility.DisplayDialog("Load failed", "File " + path + " does not contain a clip", "Ok");
				#endif
				return;
			}
	
	        Rig clip_rig = new_clip.Rig();
	
			if (IsSourceRigDifferent(clip_rig)) {
				if ((cachedRigLoaded) && (!SourceRigChangedAskToContinue ())) {
					// The user wants to cancel
					return;
				}
	        }
	
			// always save new rig from the fsb file to have the same order
	
			// Apply new rig
			m_fs_Rig = clip_rig;
			
			// Cache new rig
			SaveSourceRig();
	
	        m_clip = new_clip;
			m_clip_path = path;
	    }
	
	    /**
	     * @brief Shows a message popup to the user stating that the source rig changed and asks the user to continue
	     * @return true, if the user wants to continue and false otherwise
	     */
	    public bool SourceRigChangedAskToContinue() {
	    
	#if UNITY_EDITOR
			if (m_fs_Rig == null || m_fs_Rig.IsEmpty()) {
				return true;
			} else {
		        return (EditorUtility.DisplayDialog("Different source rig!",
		                                            "The file contains a different source rig. Do you want to continue?", "Yes", "No"));
	        }
	#else
	        return true;
	#endif
	    }
	
		//! Clears the tpose
		public void ClearTPose() {
			m_TPose = null;
			m_TPoseAsset = null;
		}
	
		/**
		 * Saves the current pose of the character as T-pose
		 * @param path [in] The path to the file
		 */
		public void SetTPose() {
			m_TPose = new TPose();
			Utils.GetGameObjectTransformations (gameObject, m_TPose.m_joints);
		}
	
		//! Loads the t pose of the character.
		public bool LoadTPoseFromFile(string filename_with_path) {
		
			TPose new_tpose = new TPose();
			if (new_tpose.LoadFromFile (filename_with_path)) {
				m_TPose = new_tpose;
				m_TPoseAsset = null;
				return true;
			} else {
				return false;
			}
		}
	
		/**
		 * Saves the current pose of the character as T-pose
		 * @param path [in] The path to the file
		 */
		public void SaveTPoseToFile(string filename_with_path) {
		
			if (m_TPose.SaveToFile(filename_with_path)) {
				m_TPoseAsset = null;
			}
		}
	
	#if UNITY_EDITOR
		public void SaveTPoseToAsset(string path) {
		
			if (m_TPose.SaveToFile(path)) {
				AssetDatabase.Refresh();
	
				string asset_path = Utils.AssetPath(path);
				Debug.Log ("loading asset " + asset_path);
				m_TPoseAsset = AssetDatabase.LoadAssetAtPath(asset_path, typeof(TextAsset)) as TextAsset;
				if (m_TPoseAsset == null) {
					Debug.LogError ("no tpose asset at path " + asset_path);
				}
			}
		}
	#endif
	
		public bool SaveClipAsAnim(string path) {
			return GetClipAsAnim(path) != null;
		}
		
		/**
	     * @brief Stores the current clip into a Unity .anim file
	     * Important: If you use your own character with blend shapes, you have to make sure it is
	     * set to 'legacy' animation type. You can do this by the following steps in Unity3D:
	     * 1. In the 'project' window in the Assets hierarchy, click on your fbx model
	     * 2. In the Inspector, you should see now the Import Settings of your model.
	     * 3. Select in these Import Settings in the 'Rig' tab for the 'Animation Type' the value 'Legacy'
	     */ 
		public AnimationClip GetClipAsAnim(string path) {

			bool have_t_pose = false; 
			if (m_TPose != null && m_TPose.m_joints != null) {
				if (m_TPose.m_joints.Count != m_GameObjectTransformations.Count) {
					Debug.LogError ("tpose and model do not have the same number of transformations (" + m_TPose.m_joints.Count + "!=" 
					                + m_GameObjectTransformations.Count + ")");
					return null;
				}
				have_t_pose = true;
			} else {
				Debug.LogWarning ("tpose missing");
			}
			
			AnimationClip animation_clip = new AnimationClip();
			
			// Not all of them might actually be used, since not all of them might be a target
			AnimationCurve [] translation_x_curves = new AnimationCurve[m_GameObjectTransformations.Count];
			AnimationCurve [] translation_y_curves = new AnimationCurve[m_GameObjectTransformations.Count];
			AnimationCurve [] translation_z_curves = new AnimationCurve[m_GameObjectTransformations.Count];
			AnimationCurve [] rotation_x_curves = new AnimationCurve[m_GameObjectTransformations.Count];
			AnimationCurve [] rotation_y_curves = new AnimationCurve[m_GameObjectTransformations.Count];
			AnimationCurve [] rotation_z_curves = new AnimationCurve[m_GameObjectTransformations.Count];
			AnimationCurve [] rotation_w_curves = new AnimationCurve[m_GameObjectTransformations.Count];
			AnimationCurve [] bs_curves = new AnimationCurve[m_GameObjectBlendshapes.Count];
			
			Clip clip = m_clip.Duplicate();
			if (m_normalize_headpose) clip.NormalizeHeadPoseAllClip();
			
			Debug.Log("nr of clip states: " + clip.NumStates());
			for (int frame_nr = 0; frame_nr < clip.NumStates(); frame_nr++) {
				if (!clip[frame_nr].TrackingSuccessful()) {
					Debug.Log("skipping clip state");
					continue;
				}
				
				// get frame time
				float time = (float)(clip[frame_nr].Timestamp() - clip[0].Timestamp()) * 0.001f; // time is in ms
				
				// evaluate transformation
				TransformationValue [] transformation_values = null;
				if (have_t_pose) {
					transformation_values = Utils.EvaluateTargetTransformations(m_Retargeting, clip.Rig(), clip[frame_nr], m_TPose.m_joints);
				} else {
					transformation_values = Utils.EvaluateTargetTransformations(m_Retargeting, clip.Rig(), clip[frame_nr], m_GameObjectTransformations);
				}
				
				int key_index = -1;
				if (transformation_values.Length == m_GameObjectTransformations.Count) {
					for (int index = 0; index < transformation_values.Length; index++) {
						// Apply the value for this target
						if (transformation_values[index] != null) {
							// Apply the translation value for this target
							if (translation_x_curves[index] == null) {
								translation_x_curves[index] = new AnimationCurve();
								translation_y_curves[index] = new AnimationCurve();
								translation_z_curves[index] = new AnimationCurve();
							}
							key_index = translation_x_curves[index].AddKey(time, transformation_values[index].m_translation.x);
							if (key_index < 0) Debug.LogError("Could not add key at time " + time);
							key_index = translation_y_curves[index].AddKey(time, transformation_values[index].m_translation.y);
							if (key_index < 0) Debug.LogError("Could not add key at time " + time);
							key_index = translation_z_curves[index].AddKey(time, transformation_values[index].m_translation.z);
							if (key_index < 0) Debug.LogError("Could not add key at time " + time);
							
							if (rotation_x_curves[index] == null) {
								rotation_x_curves[index] = new AnimationCurve();
								rotation_y_curves[index] = new AnimationCurve();
								rotation_z_curves[index] = new AnimationCurve();
								rotation_w_curves[index] = new AnimationCurve();
							}
							// Add to curve for the animation
							key_index = rotation_x_curves[index].AddKey(time, transformation_values[index].m_rotation.x);
							if (key_index < 0) Debug.LogError("Could not add key at time " + time);
							key_index = rotation_y_curves[index].AddKey(time, transformation_values[index].m_rotation.y);
							if (key_index < 0) Debug.LogError("Could not add key at time " + time);
							key_index = rotation_z_curves[index].AddKey(time, transformation_values[index].m_rotation.z);
							if (key_index < 0) Debug.LogError("Could not add key at time " + time);
							key_index = rotation_w_curves[index].AddKey(time, transformation_values[index].m_rotation.w);
							if (key_index < 0) Debug.LogError("Could not add key at time " + time);
						}
					}
				} else {
					Debug.LogError("Cannot create transformation as evaluated shape size is incorrect");
				}
				
				// evaluate blendshapes
				BlendshapeValue [] blendshape_values = Utils.EvaluateTargetBlendshapes(m_Retargeting, clip.Rig(), clip[frame_nr], m_GameObjectBlendshapes);
				
				if (blendshape_values.Length == m_GameObjectBlendshapes.Count) {
					for (int index = 0; index < m_GameObjectBlendshapes.Count; index++) {
						// Apply the value for this target
						if (blendshape_values[index] != null) {
							if (bs_curves[index] == null) {
								bs_curves[index] = new AnimationCurve();
							}
							bs_curves[index].AddKey(time, (float)blendshape_values[index].m_value);
						}
					}
				} else {
					Debug.LogError("Cannot create blendshapes as evaluated shape size is incorrect");
				}
			}
			
			// Set all transformation curves for all transformations that are animated
			for (int target_nr = 0; target_nr < m_GameObjectTransformations.Count; target_nr++) {
				// Extract path:
				string path_to_transformation = ((TransformationInformation)m_GameObjectTransformations[target_nr]).transformPath;
				// Apply translation curve, if there is one
				if (translation_x_curves[target_nr] != null) {
					animation_clip.SetCurve(path_to_transformation, typeof(Transform), "localPosition.x", translation_x_curves[target_nr]);
					animation_clip.SetCurve(path_to_transformation, typeof(Transform), "localPosition.y", translation_y_curves[target_nr]);
					animation_clip.SetCurve(path_to_transformation, typeof(Transform), "localPosition.z", translation_z_curves[target_nr]);
				}
				// Apply rotation curve, if there is one
				if (rotation_x_curves[target_nr] != null) {
					animation_clip.SetCurve(path_to_transformation, typeof(Transform), "localRotation.x", rotation_x_curves[target_nr]);
					animation_clip.SetCurve(path_to_transformation, typeof(Transform), "localRotation.y", rotation_y_curves[target_nr]);
					animation_clip.SetCurve(path_to_transformation, typeof(Transform), "localRotation.z", rotation_z_curves[target_nr]);
					animation_clip.SetCurve(path_to_transformation, typeof(Transform), "localRotation.w", rotation_w_curves[target_nr]);
				}
			}
			
			// Without this, there are some weird jumps (rotation) in the animation:
			animation_clip.EnsureQuaternionContinuity();
			
			// Set all blendshape curves for all blendshapes that are animated
			for (int i = 0; i < m_GameObjectBlendshapes.Count; i++) {
				if (bs_curves[i] != null) {
					BlendshapeInfo bs_info = (BlendshapeInfo)(m_GameObjectBlendshapes[i]);
					// Debug.Log("bs_curves[" + i + "].length=" + bs_curves[i].length);
					string bs_path = bs_info.m_path;
					string bs_name = bs_info.m_name;
					animation_clip.SetCurve(bs_path, typeof(SkinnedMeshRenderer), "blendShape." + bs_name, bs_curves[i]);
				}
			}
			
			Debug.Log ("Animation clip = " + animation_clip.length);
			
			// animation clip asset 
			string animation_name = Path.GetFileNameWithoutExtension(path);
			animation_clip.name = animation_name;
			AnimationEvent animation_event = new AnimationEvent();
			animation_event.functionName = "AnimationClipEventCallback";
			animation_event.time = animation_clip.length;
			#if UNITY_EDITOR
			AnimationEvent[] animation_events = { animation_event };
			AnimationUtility.SetAnimationEvents(animation_clip, animation_events);
			AssetDatabase.CreateAsset(animation_clip, path);
			#endif
			
			Debug.Log("Wrote animation with length " + (1000.0 * animation_clip.length) + " milliseconds");
			return animation_clip;
		}
		
		void AnimationClipEventCallback() {
			// Dummy event callback function because the AnimationClip needs an AnimationEvent
			// otherwise the AnimationClip created by Unity will always have its stop time set to 1 second.
			// The AnimationEvent should have a length greater or equal to the AnimationClip.
			// AnimationUtility.SetAnimationEvents needs to be used to make the event persistent.
		}
	
		/**
	     * Parses the connected game object and all sub objects for
	     * blendshapes and tranformation. Writes these into targetBlendShapes
	     * and targetTransformations, respectively.
	     **/
		public void UpdateTargetBlendshapesAndTransformations() {
			mutexOnTargetBlendshapeList.WaitOne();
			base.UpdateBlendshapesAndTransformations();
			mutexOnTargetBlendshapeList.ReleaseMutex();
		}
		
		//! @returns all target blendshape names
		public string[] TargetBlendshapeNames() {
			string[] names = new string[m_GameObjectBlendshapes.Count];
			for (int i = 0; i < m_GameObjectBlendshapes.Count; i++) {
				names[i] = ((BlendshapeInfo)(m_GameObjectBlendshapes[i])).m_name;
			}
			return names;
		}
		
		//! @returns True if the target contains the blendshape name.
		public bool HasTargetBlendshapeName(string name) {
			for (int i = 0; i < m_GameObjectBlendshapes.Count; i++) {
				if (((BlendshapeInfo)(m_GameObjectBlendshapes[i])).m_name == name) return true;
			}
			return false;
		}
		
		//! @returns all target transformation names
		public string[] TargetTransformationNames() {
			string[] names = new string[m_GameObjectTransformations.Count];
			for (int i = 0; i < m_GameObjectTransformations.Count; i++) {
				names[i] = ((TransformationInformation)(m_GameObjectTransformations[i])).transformName;
			}
			return names;
		}
		
		//! @returns True if the target contains the bone name.
		public bool HasTargetBoneName(string name) {
			for (int i = 0; i < m_GameObjectTransformations.Count; i++) {
				if (((TransformationInformation)(m_GameObjectTransformations[i])).transformName == name) return true;
			}
			return false;
		}
	}

}
