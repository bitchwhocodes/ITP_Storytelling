/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace fs {

	/**
	 * @brief class Rig contains the names and indices of the blendshapes and bones of a rig.
	 */
	public class Rig {
	
	    //! @brief Blendshape names of rig.
	    private List<string> m_shapes;
	
	    //! @brief Bone names of rig.
	    private List<string> m_bones;
	
	    //! @brief Create an empty track rig.
	    public Rig() {
	        m_shapes = new List<string>();
	        m_bones = new List<string>();
	    }
	    
		//! @returns a rig build from a set of blendshape names
		static public Rig GetRigFromBlendShapeNames(string [] blendshape_names) {
			Rig rig = new Rig();
			rig.AddBone("Neck");
			rig.AddBone("EyeLeft");
			rig.AddBone("EyeRight");
			for (int i = 0; i < blendshape_names.Length; i++) {
				rig.AddShape(blendshape_names[i]);
			}
			return rig;
		}
	
		//! duplicate this rig (deep copy)
		public Rig Duplicate() {
		
			Rig rig = new Rig ();
			for (int i = 0; i < NumShapes(); i++) rig.AddShape(ShapeName(i));
			for (int i = 0; i < NumBones() ; i++) rig.AddBone(BoneName(i));
			return rig;
		}
	
	    public void Clear() {
	    
	        m_shapes.Clear();
	        m_bones.Clear();
	    }
	
		public bool IsEmpty() {
			return m_shapes.Count == 0 && m_bones.Count == 0;
		}
		
	    /**
	     * @brief Get the number of blendshapes.
	     * @return The number of blendshapes.
	     */
	    public int NumShapes() { return m_shapes.Count; }
	
	    /**
	     * @brief Add new shape.
	     * @param[in] shape_name   Name of shape.
	     * @return True if shape could be added, False if shape is already in rig.
	     */
	    public bool AddShape(string shape_name) {
	    
	        if (m_shapes.Contains(shape_name)) return false;
	
	        m_shapes.Add(shape_name);
	        return true;
	    }
	
	    /**
	     * @brief Returns index of shape.
	     * @param[in] shape_name Name of shape.
	     * @return Index of shape, -1 if shape not in rig.
	     */
	    public int ShapeIndex(string shape_name) {
	    
	        return m_shapes.LastIndexOf(shape_name);
	    }
	
	    /**
	     * @brief Returns the name of shape, given the index.
	     * @param[in] index Index of the shape
	     * @return Name of shape, null if shape not in rig.
	     */
	    public string ShapeName(int index) {
	    
	        if ((index < 0) || (index >= m_shapes.Count)) {
	            return null;
	        }
	
	        return m_shapes[index];
	    }
	
	    //! @brief Returns the shape names as an array
	    public string[] GetShapeNames() {
	        return m_shapes.ToArray();
	    }
	
	    //! @brief Returns the bone names as an array
	    public string[] GetBoneNames() {
	        return m_bones.ToArray();
	    }
	
	    /**
	     * @brief Get the number of bones.
	     * @return The number of bones.
	     */
	    public int NumBones() { return m_bones.Count; }
	
	    /**
	     * @brief Add new bone.
	     * @param[in] bone_name    Name of Bone.
	     * @return True if bone could be added, False if bone is already in rig.
	     */
	    public bool AddBone(string bone_name) {
	    
	        if (m_bones.Contains(bone_name)) return false;
	
	        m_bones.Add(bone_name);
	        return true;
	    }
	
	    /**
	     * @brief Returns index of bone.
	     * @param[in] bone_name Name of bone.
	     * @return Index of bone, -1 if bone not in rig.
	     */
	    public int BoneIndex(string bone_name) {
	        return m_bones.LastIndexOf(bone_name);
	    }
	
	    /**
	     * @brief Returns the name of a bone, given the index.
	     * @param[in] index Index of the bone
	     * @return Name of bone, null if bone not in rig.
	     */
	    public string BoneName(int index) {
	    
	        if ((index < 0) || (index >= m_bones.Count)) {
	            return null;
	        }
	
	        return m_bones[index];
	    }
	
	    /**
	     * @brief Stores the rig into an fst file
	     * @param[in] path The path to the fst file
	     */
	    public bool SaveToFST(string path) {
	    
	        StreamWriter stream;
	
	        try {
	            stream = File.CreateText(path);
	        } catch (Exception) {
	            return false;
	        }
	
	        // write blendshapes
	        foreach (string entry in m_shapes) {
	            stream.WriteLine("bs = " + entry);
	        }
	
	        // write joints
	        foreach (string entry in m_bones) {
	            stream.WriteLine("joint = " + entry);
	        }
	
	        stream.Close();
	
	        return true;
	    }
	
	    /**
	     * @brief Loads the rig from an fst file
	     * @param path [in] The path to the fst file
	     */
	    public bool LoadFromFST(string path) {
	    
	        // open file
	        FileStream fileReader;
	
	        try {
	            fileReader = File.OpenRead(path);
	        } catch (Exception) {
	            return false;
	        }
	
	        int size = (int) fileReader.Length;
	
	        // read the data
	        byte [] data = new byte[size];
	        int size_read = fileReader.Read(data, 0, size);
	
	        if (size != size_read) return false;
	
			return LoadFromFST (data);
	    }
	
		/**
	     * @brief Loads the rig from an fst file
	     * @param data [in] The fst file data.
	     */
		public bool LoadFromFST(byte [] data) {
		
			string text = System.Text.Encoding.UTF8.GetString(data);
			
			string[] lines = text.Split('\n');
			
			Clear();
			
			for (int i = 0; i < lines.Length; i++) {
				string line = lines[i].Trim ();
				if (line.Length == 0) continue;
				if (line[0] == '#') continue;
	
				string[] elements = lines[i].Split('=');
				
				if (elements.Length > 1) {
					string label = elements[0].Trim().ToLower();
					
					switch (label) {
					case "name":  break;
					case "scale":  break;
						
					case "bs":
					case "joint":
					case "rotation":
					case "translation": {
						// format: bs/joint = src_name = target_name [= weight]
						bool is_bs = (label == "bs");
						string src = "";
						
						if (elements.Length > 1) {
							src = elements[1].Trim ();
						}
						
						if (src != "") {
							if (is_bs) {
								AddShape(src);
							} else {
								if (src == "jointNeck" || src == "neck") src = "Neck";
								if (src == "jointEyeLeft" || src == "eyeLeft") src = "EyeLeft";
								if (src == "jointEyeRight" || src == "eyeRight") src = "EyeRight";
								AddBone(src);
							}
						}
					} break;
						
					default : {
					} break;
					}
				}
			}
			
			return (NumBones() > 0 || NumShapes() > 0);
		}
	
	    public override bool Equals(System.Object other) {
	        return Equals((Rig)other);
	    }
	
	    public bool Equals(Rig other) {
	    
	        if ((m_shapes.Count != other.m_shapes.Count) || (m_bones.Count != other.m_bones.Count)) {
	            return false;
	        }
	
	        // Since we do not have duplicates and we only reach here if we have the same
	        // number of bones and shapes in both rigs, we can check only in one direction with Contains() tests.
	        // The order of the entries does not have to be the same
	        for (int i = 0; i < m_shapes.Count; i++) {
	            if (!m_shapes.Contains(other.m_shapes[i])) {
	                return false;
	            }
	        }
	
	        for (int i = 0; i < m_bones.Count; i++) {
	            if (!m_bones.Contains(other.m_bones[i])) {
	                return false;
	            }
	        }
	
	        return true;
	    }
	
	    public override int GetHashCode() {
	        return m_bones.GetHashCode() ^ m_shapes.GetHashCode();
	    }
	}
}

