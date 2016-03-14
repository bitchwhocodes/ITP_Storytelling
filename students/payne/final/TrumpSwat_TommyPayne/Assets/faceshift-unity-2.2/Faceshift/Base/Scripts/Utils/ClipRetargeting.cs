/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fs {

	/**
	 * @brief class Mapping contains a mapping of the retargeting
	 */
	class Mapping : IComparable<Mapping> {
	
	    public string source { get; set; }
	    public string destination { get; set; }
	    public double weight { get; set; }
	
	    public Mapping(string source_i, string destination_i, double weight_i) {
	        source = source_i;
	        destination = destination_i;
	        weight = weight_i;
	    }
	
	    public override bool Equals(System.Object mapping) {
	        return ((Mapping)mapping).source.Equals(source) && ((Mapping)mapping).destination.Equals(destination);
	    }
	
	    public bool Equals(Mapping mapping) {
	        return mapping.source.Equals(source) && mapping.destination.Equals(destination);
	    }
	
	    public override int GetHashCode() {
	        return source.GetHashCode() ^ destination.GetHashCode();
	    }
	
	    /**
	     * @brief Used for sorting the list: first according the source. If the source is the same then according to the destination
	     * @param other The other mapping we want to compare with.
	     */
	    public int CompareTo(Mapping other) {
	    
	        if (this.source.Equals(other.source)) {
	            return other.destination.CompareTo(this.destination);
	        }
	
	        return other.source.CompareTo(this.source);
	    }
	}
	
	/**
	 * @brief class ClipRetargeting stores the retargeting from a Rig to a unity game object.
	 */
	public class ClipRetargeting {
	
	    //! @brief Mapping of src blendshape values to target blendshape values.
	    private List<Mapping> m_blendshape_mapping;
	
	    //! @brief Mapping of src joint rotations to target joint rotations.
	    private List<Mapping> m_rotation_mapping;
	
	    //! @brief Mapping of src translation to target translation.
	    private List<Mapping> m_translation_mapping;
	
	    //! @brief Create an empty retargeting object.
	    public ClipRetargeting()
	    {
	        m_blendshape_mapping = new List<Mapping>();
	        m_rotation_mapping = new List<Mapping>();
	        m_translation_mapping = new List<Mapping>();
	    }
	
	
		//! @returns True if there mapping is empty.
		public bool IsEmpty() {
				return (GetNumberOfBlendshapeMappings () == 0 &&
						GetNumberOfRotationMappings () == 0 &&
						GetNumberOfTranslationMappings () == 0);
		}
	
	    /**
	     * @brief Set the blendshape mapping from src to target such that target_blendshape_value_contribution = src_blendshape_value * weight
	     * @param[in] src      The src blendshape.
	     * @param[in] target   The target blendshape.
	     * @param[in] weight   The weighting.
	     */
	    public void AddBlendshapeMapping(string src, string target, double weight) {
	    
	        Mapping new_item = new Mapping(src, target, weight);
	
	        if (!m_blendshape_mapping.Contains(new_item)) {
	            m_blendshape_mapping.Add(new_item);
	        }
	    }
	
	    /**
	     * @brief Set the rotation mapping from src to target such that target_joint_rotation = src_joint_rotation * weight
	     * @param[in] src      The src joint.
	     * @param[in] target   The target joint.
	     * @param[in] weight   The weighting.
	     */
	    public void AddRotationMapping(string src, string target, double weight) {
	    
	        Mapping new_item = new Mapping(src, target, weight);
	
	        if (!m_rotation_mapping.Contains(new_item)) {
	            m_rotation_mapping.Add(new_item);
	        }
	    }
	
	    /**
	     * @brief Set the translation mapping from src to target such that target_translation = src_translation * weight
	     * @param[in] src      The src joint.
	     * @param[in] target   The target joint.
	     * @param[in] weight   The weighting.
	     */
	    public void AddTranslationMapping(string src, string target, double weight) {
	    
	        Mapping new_item = new Mapping(src, target, weight);
	
	        if (!m_translation_mapping.Contains(new_item)) {
	            m_translation_mapping.Add(new_item);
	        }
	    }
	
	    /**
	     * @brief Updates a blendshape mapping to new values
	     * @param[in] index    The index of the blendshape mapping
	     * @param[in] src      The src blendshape.
	     * @param[in] target   The target blendshape.
	     * @param[in] weight   The weighting
	     */
	    public bool UpdateBlendshapeMapping(int index, string src, string target, double weight) {
	    
	        // Make sure we do not have duplicates:
	        if (!m_blendshape_mapping.Contains(new Mapping(src, target, weight))) {
	            if ((index >= 0) && (index < m_blendshape_mapping.Count)) {
	                m_blendshape_mapping[index].source = src;
	                m_blendshape_mapping[index].destination = target;
	                m_blendshape_mapping[index].weight = weight;
	                return true;
	            }
	        } else {
	            if ((index >= 0) && (index < m_blendshape_mapping.Count)
	                && (m_blendshape_mapping[index].source.Equals(src)) && (m_blendshape_mapping[index].destination.Equals(target))) {
	                m_blendshape_mapping[index].weight = weight;
	                return true;
	            }
	        }
	
	        return false;
	    }
	
	    /**
	     * @brief Updates a rotation mapping to new values
	     * @param[in] index    The index of the rotation mapping
	     * @param[in] src      The src rotation.
	     * @param[in] target   The target rotation.
	     * @param[in] weight   The weighting
	     */
	    public bool UpdateRotationMapping(int index, string src, string target, double weight) {
	    
	        // Make sure we do not have duplicates:
	        if (!m_rotation_mapping.Contains(new Mapping(src, target, weight))) {
	            if ((index >= 0) && (index < m_rotation_mapping.Count)) {
	                m_rotation_mapping[index].source = src;
	                m_rotation_mapping[index].destination = target;
	                m_rotation_mapping[index].weight = weight;
	                return true;
	            }
	        } else {
	            if ((index >= 0) && (index < m_rotation_mapping.Count)
	                && (m_rotation_mapping[index].source.Equals(src)) && (m_rotation_mapping[index].destination.Equals(target))) {
	                m_rotation_mapping[index].weight = weight;
	                return true;
	            }
	        }
	
	        return false;
	    }
	
	    /**
	     * @brief Updates a translation mapping to new values
	     * @param[in] index    The index of the translation mapping
	     * @param[in] src      The src translation.
	     * @param[in] target   The target translation.
	     * @param[in] weight   The weighting
	     */
	    public bool UpdateTranslationMapping(int index, string src, string target, double weight) {
	    
	        // Make sure we do not have duplicates:
	        if (!m_translation_mapping.Contains(new Mapping(src, target, weight))) {
	            if ((index >= 0) && (index < m_translation_mapping.Count)) {
	                m_translation_mapping[index].source = src;
	                m_translation_mapping[index].destination = target;
	                m_translation_mapping[index].weight = weight;
	                return true;
	            }
	        } else {
	            if ((index >= 0) && (index < m_translation_mapping.Count)
	                && (m_translation_mapping[index].source.Equals(src)) && (m_translation_mapping[index].destination.Equals(target))) {
	                m_translation_mapping[index].weight = weight;
	                return true;
	            }
	        }
	
	        return false;
	    }
	
	    //! @brief Returns the number of blend shape mappings we have stored
	    public int GetNumberOfBlendshapeMappings() {
	        return m_blendshape_mapping.Count;
	    }
	
	    //! @brief Returns the number of rotation mappings we have stored
	    public int GetNumberOfRotationMappings() {
	        return m_rotation_mapping.Count;
	    }
	
	    //! @brief Returns the number of translation mappings we have stored
	    public int GetNumberOfTranslationMappings() {
	        return m_translation_mapping.Count;
	    }
	
	    //! @brief Returns the source of blend shape mapping with the number 'index' within the list of blend shape mappings
	    public string GetBlendshapeMappingSource(int index) {
	    
	        if ((index < 0) || (index >= m_blendshape_mapping.Count)) {
	            return null;
	        }
	
	        return m_blendshape_mapping[index].source;
	    }
	
	    //! @brief Returns the source of rotation mapping with the number 'index' within the list of rotation mappings
	    public string GetRotationMappingSource(int index) {
	    
	        if (index >= m_rotation_mapping.Count) {
	            return null;
	        }
	
	        return m_rotation_mapping[index].source;
	    }
	
	    //! @brief Returns the source of translation mapping with the number 'index' within the list of translation mappings
	    public string GetTranslationMappingSource(int index) {
	    
	        if (index >= m_translation_mapping.Count) {
	            return null;
	        }
	
	        return m_translation_mapping[index].source;
	    }
	
	    //! @brief Returns the target of blend shape mapping with the number 'index' within the list of blend shape mappings
	    public string GetBlendshapeMappingDestination(int index) {
	    
	        if (index >= m_blendshape_mapping.Count) {
	            return null;
	        }
	
	        return m_blendshape_mapping[index].destination;
	    }
	
	    //! @brief Returns the target of rotation mapping with the number 'index' within the list of rotation mappings
	    public string GetRotationMappingDestination(int index) {
	    
	        if (index >= m_rotation_mapping.Count) {
	            return null;
	        }
	
	        return m_rotation_mapping[index].destination;
	    }
	
	    //! @brief Returns the target of translation mapping with the number 'index' within the list of translation mappings
	    public string GetTranslationMappingDestination(int index) {
	    
	        if (index >= m_translation_mapping.Count) {
	            return null;
	        }
	
	        return m_translation_mapping[index].destination;
	    }
	
	    //! @brief Returns the weight of blend shape mapping with the number 'index' within the list of blend shape mappings
	    public double GetBlendshapeMappingWeight(int index) {
	    
	        if (index >= m_blendshape_mapping.Count) {
	            return 0.0;
	        }
	
	        return m_blendshape_mapping[index].weight;
	    }
	
	    //! @brief Returns the weight of rotation mapping with the number 'index' within the list of rotation mappings
	    public double GetRotationMappingWeight(int index) {
	    
	        if (index >= m_rotation_mapping.Count) {
	            return 0.0;
	        }
	
	        return m_rotation_mapping[index].weight;
	    }
	
	    //! @brief Returns the weight of translation mapping with the number 'index' within the list of translation mappings
	    public double GetTranslationMappingWeight(int index) {
	    
	        if (index >= m_translation_mapping.Count) {
	            return 0.0;
	        }
	
	        return m_translation_mapping[index].weight;
	    }
	
		//! @brief Removes the blend shape mapping at index.
		public void RemoveBlendshapeMapping(int index) {
		
			if ((index < 0) || (index >= m_blendshape_mapping.Count)) return;
			m_blendshape_mapping.RemoveAt (index);
		}
	
	    //! @brief Removes the blend shape mapping with the given source and destination
	    public void RemoveBlendshapeMapping(string source, string destination) {
	        m_blendshape_mapping.Remove(new Mapping(source, destination, 0.0));
	    }
	
		//! @brief Removes the rotation mapping at index.
		public void RemoveRotationMapping(int index) {
		
			if ((index < 0) || (index >= m_rotation_mapping.Count)) return;
			m_rotation_mapping.RemoveAt (index);
		}
	
	    //! @brief Removes the rotation mapping with the given source and destination
	    public void RemoveRotationMapping(string source, string destination) {
	        m_rotation_mapping.Remove(new Mapping(source, destination, 0.0));
	    }
	
		//! @brief Removes the rotation mapping at index.
		public void RemoveTranslationMapping(int index) {
			if ((index < 0) || (index >= m_translation_mapping.Count)) return;
			m_translation_mapping.RemoveAt (index);
		}
	
	    //! @brief Removes the translation mapping with the given source and destination
	    public void RemoveTranslationMapping(string source, string destination) {
	        m_translation_mapping.Remove(new Mapping(source, destination, 0.0));
	    }
	
	    /**
	     * @brief Reads an fst file and returns a clip retargeting constructed from the content of the fst file.
	     * @param[in] filename The filename of the fst file.
	     * @return A clip retargeting with the retargeting info imported from the fst file, null if there was some error during loading.
	     */
	    public static ClipRetargeting Load(string filename) {
	    
			if (!File.Exists (filename)) return null;
	
	        // open file
	        FileStream fileReader = File.OpenRead(filename);
	        int size = (int) fileReader.Length;
	
	        // read the data
	        byte [] data = new byte[size];
	        int size_read = fileReader.Read(data, 0, size);
	
	        if (size != size_read) return null;
			return Load(data, /*fromAsset=*/false);
	    }
	
		/**
	     * @brief Reads an fst file and returns a clip retargeting constructed from the content of the fst file.
	     * @param[in] filename The filename of the fst file.
	     * @return A clip retargeting with the retargeting info imported from the fst file, null if there was some error during loading.
	     */
		public static ClipRetargeting Load(byte [] data, bool fromAsset) {
				
			string text = System.Text.Encoding.UTF8.GetString(data);
			
			ClipRetargeting retargeting = new ClipRetargeting();
			
			string[] lines = text.Split('\n');
			
			for (int i = 0; i < lines.Length; i++) {
				string[] elements = lines[i].Split('=');
				
				if (elements.Length > 1) {
					string label = elements[0].Trim().ToLower();
					
					switch (label) {
					case "name": break;
					case "scale": break;
					case "bs":
					case "joint":
					case "rotation":
					case "translation": {
						// format: bs/joint = src_name = target_name [= weight]
						bool is_bs = (label == "bs");
						double weight = 1;
						string src = "";
						string target = "";
						
						if (elements.Length > 2) {
							// we require src and target for a mapping
							src = elements[1].Trim();
							target = elements[2].Trim();
							
							if (elements.Length > 3) {
								weight = Convert.ToDouble(elements[3].Trim ());
							}
							
							if (is_bs) {
								// automatically map to the 100 scale
								if (!fromAsset) weight *= 100;
								retargeting.AddBlendshapeMapping(src, target, weight);
							} else {
								// automatically rename fs joint names
								if (src == "jointNeck" || src == "neck") src = "Neck";
								if (src == "jointEyeLeft" || src == "eyeLeft") src = "EyeLeft";
								if (src == "jointEyeRight" || src == "eyeRight") src = "EyeRight";
	
								if (label.Equals("translation")) {
									retargeting.AddTranslationMapping(src, target, weight);
								} else {
									retargeting.AddRotationMapping(src, target, weight);
								}
							}
						}
					} break;
						
					default : {
					} break;
					}
				}
			}
			
			return retargeting;
		}
	
	    //! @brief Save retargeting to file.
	    public bool Save(string filename, Rig sourceRig) {
	
	        StreamWriter stream = File.CreateText(filename);
	
	        //////////////
	        /// Blend shapes
	        // Get list of all blend shapes
	        List<string> possible_source_blend_shapes = null;
	
	        if (sourceRig != null) {
	            // We also store the blend shapes that are not connected
	            possible_source_blend_shapes = new List<string>();
	
	            for (int i = 0; i < sourceRig.NumShapes(); i++) {
	                possible_source_blend_shapes.Add(sourceRig.ShapeName(i));
	            }
	        }
	
	        // write blendshapes
	        foreach (Mapping entry in m_blendshape_mapping) {
	            stream.WriteLine("bs = " + entry.source + " = " + entry.destination + " = " + entry.weight);
	            possible_source_blend_shapes.Remove(entry.source);
	        }
	
	        // write not mapped blendshapes
	        foreach (string entry in possible_source_blend_shapes) {
	            stream.WriteLine("bs = " + entry);
	        }
	
	        //////////////
	        /// Rotations
	        // Get list of all rotations
	        List<string> possible_source_rotations = null;
	
	        if (sourceRig != null) {
	            // We also store the rotations that are not connected
	            possible_source_rotations = new List<string>();
	
	            for (int i = 0; i < sourceRig.NumBones(); i++) {
	                possible_source_rotations.Add(sourceRig.BoneName(i));
	            }
	        }
	
	        // write rotations
	        foreach (Mapping entry in m_rotation_mapping) {
	            stream.WriteLine("rotation = " + entry.source + " = " + entry.destination + " = " + entry.weight);
	            possible_source_rotations.Remove(entry.source);
	        }
	
	        // write not mapped rotations
	        foreach (string entry in possible_source_rotations) {
	            stream.WriteLine("rotation = " + entry);
	        }
	
	        //////////////
	        /// Translations
	        // Get list of all translations
	        List<string> possible_source_translations = null;
	
	        if (sourceRig != null) {
	            // We also store the translations that are not connected
	            possible_source_translations = new List<string>();
	
	            for (int i = 0; i < sourceRig.NumBones(); i++) {
	                possible_source_translations.Add(sourceRig.BoneName(i));
	            }
	        }
	
	        // write translations
	        foreach (Mapping entry in m_translation_mapping) {
	            stream.WriteLine("translation = " + entry.source + " = " + entry.destination + " = " + entry.weight);
	            possible_source_translations.Remove(entry.source);
	        }
	
	        // write not mapped translations
	        foreach (string entry in possible_source_translations) {
	            stream.WriteLine("translation = " + entry);
	        }
	
	        stream.Close();
	
	        return true;
	    }
	}
}

