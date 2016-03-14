/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
using System.Collections;

namespace fs {

	public class CharacterRetargeting : MonoBehaviour {
		
		public TextAsset m_RetargetingAsset;
		public TextAsset m_TPoseAsset;
	
		protected ClipRetargeting m_Retargeting = null; 
		protected TPose m_TPose = null;
	
		//! Caches a list of possible target blend shapes (in the connected game object)
		protected ArrayList m_GameObjectBlendshapes = new ArrayList();
		
		//! Caches a list of possible target transfromations (in the connected game object)
		protected ArrayList m_GameObjectTransformations = new ArrayList();
	
		public void Init() {
			LoadRetargetingFromAsset(m_RetargetingAsset);
			LoadTPoseFromAsset(m_TPoseAsset);
			UpdateBlendshapesAndTransformations();
		}
	
		public ClipRetargeting Retargeting() {
			return m_Retargeting;
		}
		
		public TPose TPose() {
			return m_TPose;
		}
	
		public bool LoadRetargetingFromAsset(TextAsset asset) {
			
			if (asset == null) return false;
			
			ClipRetargeting retargeting = ClipRetargeting.Load(asset.bytes, /*fromAsset=*/true);
			if (retargeting == null || retargeting.IsEmpty()) {
				ResetRetargeting();
				return false;
			}
			
			m_Retargeting = retargeting;
			m_RetargetingAsset = asset;
			
			return true;
		}
		
		public bool LoadTPoseFromAsset(TextAsset asset) {
			
			if (asset == null) return false;
			
			TPose tpose = new TPose();
			if (!tpose.LoadFromBytes(asset.bytes)) {
				ResetTPose();
				return false;
			}
			
			m_TPose = tpose;
			m_TPoseAsset = asset;
			
			return true;
		}
		
		public void ResetRetargeting() {
			m_Retargeting = null;
			m_RetargetingAsset = null;
		}
		
		public void ResetTPose() {
			m_TPose = null;
			m_TPoseAsset = null;
		}
	
		public void UpdateBlendshapesAndTransformations() {
			// Clear the cache
			m_GameObjectBlendshapes.Clear();
			m_GameObjectTransformations.Clear();
			
			// get the transformations
			Utils.GetGameObjectBlendshapes(gameObject, m_GameObjectBlendshapes);
			Utils.GetGameObjectTransformations(gameObject, m_GameObjectTransformations);
		}
	
		/// <summary>
		/// Applies the joint transformations given an influence factor
		/// </summary>
		/// <returns><c>true</c>, if transformations could be applied, <c>false</c> otherwise.</returns>
		/// <param name="transformationsToSet">The set of transformations to apply</param>
		/// <param name="influence">The factor to apply the transformations. 0.0 means that they are not used at all and the current
		/// animations are not changed. 1.0 means that they are fully used.</param>
		protected bool ApplyTransformations(TransformationValue [] transformationsToSet, float influence) {
			
			if (transformationsToSet.Length != m_GameObjectTransformations.Count) {
				return false;
			}
			
			for (int index = 0; index < m_GameObjectTransformations.Count; index++) {
				// Apply the value for this target
				if (transformationsToSet[index] != null) {
					TransformationInformation joint = m_GameObjectTransformations[index] as TransformationInformation;
					joint.transform.localRotation = Quaternion.Slerp(joint.transform.localRotation,
																	transformationsToSet[index].m_rotation, influence);
					joint.transform.localPosition = (1.0f - influence) * joint.transform.localPosition +
						influence * transformationsToSet[index].m_translation;
				}
			}
			return true;
		}
	
	
		/// <summary>
		/// Applies the blendshapes given an influence factor
		/// </summary>
		/// <returns><c>true</c>, if blendshapes could be applied, <c>false</c> otherwise.</returns>
		/// <param name="transformationsToSet">The set of blendshapes to apply</param>
		/// <param name="influence">The factor to apply the blendshapes. 0.0 means that they are not used at all and the current
		/// animations are not changed. 1.0 means that they are fully used.</param>
		protected bool ApplyBlendshapes(BlendshapeValue [] blendshapesToSetToSet, float influence) {
			
			if (blendshapesToSetToSet.Length != m_GameObjectBlendshapes.Count) {
				return false;
			}
			
			for (int index = 0; index < m_GameObjectBlendshapes.Count; index++) {
				// Apply the value for this target
				if (blendshapesToSetToSet[index] != null) {
					BlendshapeInfo bs_info = m_GameObjectBlendshapes[index] as BlendshapeInfo;
					float originalValue = bs_info.m_mesh_renderer.GetBlendShapeWeight(bs_info.m_index);
					float newValue = (float)blendshapesToSetToSet[index].m_value;
					bs_info.m_mesh_renderer.SetBlendShapeWeight(bs_info.m_index, influence * newValue + (1.0f - influence) * originalValue);
				}
			}
			return true;
		}
		
		/// <summary>
		/// Performs faceshift live tracking animation given a rig and a state.
		/// </summary>
		public void UpdateAnimation(Rig rig, RigState state) {
			
			if (rig == null || state == null) return;
			
			// evaluate joint transformations
			TransformationValue [] transformationsTracking = null;
			if (m_TPose != null && m_TPose.m_joints.Count == m_GameObjectTransformations.Count) {
				// evaluate using tpose
				transformationsTracking = Utils.EvaluateTargetTransformations(m_Retargeting, rig, state, m_TPose.m_joints);
			} else {
				// evaluate using state from start of application
				transformationsTracking = Utils.EvaluateTargetTransformations(m_Retargeting, rig, state, m_GameObjectTransformations);
			}
	
			if (transformationsTracking != null) {
				if (!ApplyTransformations(transformationsTracking, 1.0f)) {
					Debug.LogError("Cannot apply tracking transformations as evaluated shape size is incorrect");
				}
			}
	
			if (state != null) {
				BlendshapeValue [] blendshapesTracking = Utils.EvaluateTargetBlendshapes(m_Retargeting, rig, state, m_GameObjectBlendshapes);
				if (!ApplyBlendshapes(blendshapesTracking, 1.0f)) {
					Debug.LogError("Cannot apply tracking blendshapes as the number of blendshapes is not the same");
				}
			}
		}
	}

}
