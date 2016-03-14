/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace fs {

	/**
	 * @brief class RigState contains the pose, timestamp, blendshape coefficients and bone rotations of a Rig.
	 */
	public class RigState {
	
		//! true, if the rigid and non-rigid tracking was successful
		private bool m_tracking_successful;
	
	    //! timestamp in ms
	    private double m_timestamp;
	
	    //! blendshape values (from 0 to 1)
	    private double[] m_blendshape_coefficients;
	
	    //! bone values (in left hand coordinate system)
	    private Vector3[] m_bone_translations;
	
	    //! bone values (in left hand coordinate system)
	    private Quaternion[] m_bone_rotations;
	
	    /**
	     * @brief Create track state with shapes and bones.
	     * @param[in] rig The rig for which to create the state.
	     */
	    public RigState(Rig rig) {
	    
	        m_blendshape_coefficients = new double[rig.NumShapes()];
	        m_bone_translations = new Vector3[rig.NumBones()];
	        m_bone_rotations = new Quaternion[rig.NumBones()];
	    }
	    
	    //! @brief returns true, if the rigid and the non-rigid alignment of this frame was successful
	    public bool TrackingSuccessful() { return m_tracking_successful; }
	    
		/**
		 * @brief set if the rigid and the non-rigid alignment of this frame was successful
		 * @param[in] successful Set this to true, if the frame contains usable tracking data and false, otherwise
		 */
		public void SetTrackingSuccessful(bool successful) { m_tracking_successful = successful; }
	
		//! private empty constructor for duplicate
		private RigState() {}
		
		//! duplicate this state (deep copy)
		public RigState Duplicate() {
		
			RigState rig_state = new RigState();
			rig_state.m_tracking_successful = m_tracking_successful;
			rig_state.m_timestamp = m_timestamp;
			rig_state.m_blendshape_coefficients = (double[]) m_blendshape_coefficients.Clone();
			rig_state.m_bone_rotations = (Quaternion[])m_bone_rotations.Clone();
			rig_state.m_bone_translations = (Vector3[])m_bone_translations.Clone();
			return rig_state;
		}
	    /**
	     * @brief Get the timestamp of this state.
	     * @Return The timestamp value of this state.
	     */
	    public double Timestamp() { return m_timestamp; }
	
	    /**
	     * @brief Set the timestamp of this state.
	     * @param[in] value    The new timestamp value.
	     */
	    public void SetTimestamp(double value) { m_timestamp = value; }
	
	    /**
	     * @brief Get the blendshape coefficient.
	     * @param[in] index   Blendshape coefficient index (no check for validity of index).
	     * @return Blendshape coefficient at index.
	     */
	    public double BlendshapeCoefficient(int index) { return m_blendshape_coefficients[index]; }
	
	    /**
	     * @brief Set the blendshape coefficient.
	     * @param[in] index       Blendshape coefficient index (no check for validity of index).
	     * @param[in] value    New blendshape coefficient value.
	     */
	    public void SetBlendshapeCoefficient(int index, double value) { m_blendshape_coefficients[index] = value; }
	
	    /**
	     * @brief Get the translation of a bone.
	     * @param[in] index   Bone coefficient index (no check for validity of index).
	     */
	    public Vector3 BoneTranslation(int index) { return m_bone_translations[index]; }
	
	    /**
	     * @brief Set the translation of a bone.
	     * @param[in] index   Bone index (no check for validity of index).
	     * @param[in] value   New bone translation vector.
	     */
	    public void SetBoneTranslation(int index, Vector3 value) { m_bone_translations[index] = value; }
	
	    /**
	     * @brief Get the quaternion of a bone.
	     * @param[in] index   Bone coefficient index (no check for validity of index).
	     */
	    public Quaternion BoneRotation(int index) { return m_bone_rotations[index]; }
	
	    /**
	     * @brief Set the quaternion of a bone.
	     * @param[in] index           Bone index (no check for validity of index).
	     * @param[in] value    New bone quaternion.
	     */
	    public void SetBoneRotation(int index, Quaternion q) { m_bone_rotations[index] = q; }
	}
}

