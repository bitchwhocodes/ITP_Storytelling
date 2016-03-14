/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace fs {

	/**
 	 * @brief class BlendshapeInfo contains the information identifying a single blendshape of a mesh. 
 	 */
	public class BlendshapeInfo {
	
		//! path to skinned mesh
		public string m_path;

		//! name of blendshape
		public string m_name;

		//! index of blendshape for the skinned mesh
		public int m_index;

		//! the skinned mesh
		public SkinnedMeshRenderer m_mesh_renderer;
	}

	/**
	 *	@brief class BlendshapeValue wraps the value of a blendshape. By wrapping the class a not-assigned is represented by the null object. 
	 */
	public class BlendshapeValue {
	
		public BlendshapeValue(double value) {
			m_value = value;
		}

		//! the blendshape value
		public double m_value = 0;
	}
}