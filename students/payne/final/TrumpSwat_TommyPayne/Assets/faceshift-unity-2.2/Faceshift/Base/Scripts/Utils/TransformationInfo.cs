/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace fs {

	/**
 	 * @brief class TransformationInformation contains the information identifying a single transform of a mesh. 
 	 */
	public class TransformationInformation  {
	
		//! the global rotation of the transform
		public Quaternion rotation;

		//! the local rotation of the transform
		public Quaternion localRotation;

		//! the position of the transform
		public Vector3 position;

		//! the local position of the transform
		public Vector3 localPosition;

		//! the rotation of the parent if there is a parent or the identity if there is no parent.
		public Quaternion parentRotation;

		//! the path of the transform within the game object.
		public string transformPath;

		//! the name of the transform.
		public string transformName;

		//! the actual transform
		public Transform transform;

		//! empty constructor
		public TransformationInformation() {
			rotation = Quaternion.identity;
			localRotation = Quaternion.identity;
			position = Vector3.zero;
			localPosition = Vector3.zero;
			parentRotation = Quaternion.identity;
			transformPath = "";
			transformName = "";
		}
	}

	/**
	 * @brief class TransformationValue wraps the values for the local rotation and translation of a transformation.
	 */
	public class TransformationValue {
	
		//! constructor setting both rotation and translation
		public TransformationValue(Quaternion rotation, Vector3 translation) {
			m_rotation = rotation;
			m_translation = translation;
		}

		//! the local rotation of the transform
		public Quaternion m_rotation;

		//! the local translation of the transform
		public Vector3 m_translation;
	}
}