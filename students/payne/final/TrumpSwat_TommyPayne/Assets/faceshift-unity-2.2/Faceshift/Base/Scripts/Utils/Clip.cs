/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
using System;
using System.Collections.Generic;

namespace fs {

	/**
	 * @brief class Clip contains a Rig and a list of RigStates defining an animation. The states are instances of a Rig that defines the blendshape and bone names.
	 */
	public class Clip {
	
	    //! Rig determining the blendshape and bone names.
	    private Rig m_rig;
	
	    //! List of states determining the animation curves of blendshapes and bones.
	    private List<RigState> m_states;
	
	    /**
	     * @brief Create a new clip with a specific rig.
	     * @param[in] rig     The rig of the clip.
	     */
	    public Clip(Rig rig) {
	        m_rig = rig;
	        m_states = new List<RigState>();
	    }
	
		//! duplicate this clip (deep copy)
		public Clip Duplicate() {
		
			Rig rig = m_rig.Duplicate();
			Clip clip = new Clip(rig);
			for (int i = 0; i < m_states.Count; i++) {
				clip.m_states.Add(m_states[i].Duplicate());
			}
			return clip;
		}
	
	    /**
	     * @brief Creates a new state and adds it to this clip.
	     * @param[in] timestamp    The timestamp for this new state.
	     * @return The newly created state.
	     */
	    public RigState NewState(double timestamp) {
	        RigState state = new RigState(m_rig);
	        state.SetTimestamp(timestamp);
	        m_states.Add(state);
	        return state;
	    }
	
	    //! @brief Returns the rig of this clip
	    public Rig Rig() {
	        return m_rig;
	    }
	
	    /**
	     * @brief Return number of states of the clip.
	     * @return The number of states of the clip.
	     */
	    public int NumStates() { return m_states.Count; }
	
	    //! @brief Get state. No check of index.
	    public RigState this[int index] {
	    
	        get { return m_states[index]; }
	    }
	
		/**
		 * Normalize the head pose at the beginning and end of the clip so that the head pose is identity both at the beginning and the end.
		 * This makes clips particularly suitable for looping.
		 */
		public void NormalizeHeadPoseAllClip() {
		
			if (NumStates () == 0) return;
			
			int neck_index = m_rig.BoneIndex("Neck");
			double timestamp1 = m_states[0].Timestamp();
			Quaternion rot1 = m_states[0].BoneRotation(neck_index);
			Vector3 t1 = m_states [0].BoneTranslation (neck_index);
			
			double timestamp2 = m_states[NumStates() - 1].Timestamp();
			Quaternion rot2 = m_states[NumStates() - 1].BoneRotation(neck_index);
			Vector3 t2 = m_states[NumStates() - 1].BoneTranslation(neck_index);
			
			double duration = timestamp2 - timestamp1;
			duration = ((duration > 0.0) ? duration : 1.0);
	
			for (int i = 0; i < NumStates(); i++) {
				double timestamp = m_states[i].Timestamp();
				float w1 = (float) ((timestamp2-timestamp)/duration);
				float w2 = 1.0f - w1;
				Vector3 t_delta = - w1 * t1 - w2 * t2;
				m_states[i].SetBoneTranslation(neck_index, m_states[i].BoneTranslation(neck_index) + t_delta);
				Quaternion r_delta = Quaternion.Inverse(Quaternion.Slerp(rot1, rot2, w2));
				m_states[i].SetBoneRotation(neck_index, r_delta * m_states[i].BoneRotation(neck_index));
			}
		}
	}
}

