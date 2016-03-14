/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using System.Collections;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace fs
{
	/**
	 * The TPose class contains the tpose configuration of a set of joints/transformations of a game object.
	 * The TPose is used to define the relative rotations of a joint with respect to the tpose.
	 */
	public class TPose {
	
		const int TPOSE_FILE_VERSION = 0;
		
		public ArrayList m_joints;
		
		public TPose() {
			m_joints = new ArrayList();
		}
	
		//! @brief Saves the tpose to file.
		public bool SaveToFile(string filename_with_path) {
		
			// open file
			using (FileStream file_stream = new FileStream(filename_with_path, FileMode.Create)) {
				using (StreamWriter file_writer = new StreamWriter(file_stream, System.Text.Encoding.UTF8)) {
					// first row contains number of joints
					int number_of_joints = m_joints.Count;
					file_writer.WriteLine(TPOSE_FILE_VERSION + "," + number_of_joints);
					foreach (TransformationInformation joint in m_joints) {
						// We have 2 lines per joint
						file_writer.Write(joint.rotation.x + "," + joint.rotation.y + "," + joint.rotation.z + "," + joint.rotation.w);
						file_writer.Write("," + joint.localRotation.x + "," + joint.localRotation.y + "," + joint.localRotation.z + "," + joint.localRotation.w);
						file_writer.Write("," + joint.position.x + "," + joint.position.y + "," + joint.position.z);
						file_writer.Write("," + joint.localPosition.x + "," + joint.localPosition.y + "," + joint.localPosition.z);
						file_writer.WriteLine("," + joint.parentRotation.x + "," + joint.parentRotation.y + "," + joint.parentRotation.z + "," + joint.parentRotation.w);
						file_writer.WriteLine(joint.transformPath + "," + joint.transformName);
					}
				}
			}
			return true;
		}
		
		//! @brief Loads the tpose from file.
		//! @returns True if load was successful, false otherwise.
		public bool LoadFromFile(string filename_with_path) {
		
			// open file
			FileStream fileReader = File.OpenRead(filename_with_path);
			if (fileReader == null) {
				return false;
			}
			int size = (int) fileReader.Length;
			
			// read the data
			byte [] data = new byte[size];
			int size_read = fileReader.Read(data,0,size);
			if (size != size_read) {
				return false;
			}
	
			return LoadFromBytes (data);
		}
		
		//! @brief Loads the tpose from a byte array.
		//! @returns True if load was successful, false otherwise.
		public bool LoadFromBytes(byte [] data) {
		
			if (data == null) return false;
			
			// create 2D grid of the file
			string text = System.Text.Encoding.UTF8.GetString(data);			
			string[,] grid = SplitTPoseFile(text);
			//Debug.Log("size = " + grid.GetLength(0) + " , " + grid.GetLength (1)); 
			
			
			// first row contains version, number of joints
			int row = 0;
			int col = 0;
			Int32 version = 0;
			try {
				Convert.ToInt32(grid[row,col+0]);
			} catch (Exception) {
				return false;
			}
			if (version != TPOSE_FILE_VERSION) {
				return false;
			}
			Int32 number_of_joints = Convert.ToInt32(grid[row,col+1]);
			
			ArrayList new_joints = new ArrayList();
			for (int i = 0; i < number_of_joints; i++) {
				TransformationInformation joint = new TransformationInformation();
				int start_row = 1 + 2 * i;
				joint.rotation.x = (float)Convert.ToDouble(grid[start_row, 0]);
				joint.rotation.y = (float)Convert.ToDouble(grid[start_row, 1]);
				joint.rotation.z = (float)Convert.ToDouble(grid[start_row, 2]);
				joint.rotation.w = (float)Convert.ToDouble(grid[start_row, 3]);
				joint.localRotation.x = (float)Convert.ToDouble(grid[start_row, 4]);
				joint.localRotation.y = (float)Convert.ToDouble(grid[start_row, 5]);
				joint.localRotation.z = (float)Convert.ToDouble(grid[start_row, 6]);
				joint.localRotation.w = (float)Convert.ToDouble(grid[start_row, 7]);
				joint.position.x = (float)Convert.ToDouble(grid[start_row, 8]);
				joint.position.y = (float)Convert.ToDouble(grid[start_row, 9]);
				joint.position.z = (float)Convert.ToDouble(grid[start_row, 10]);
				joint.localPosition.x = (float)Convert.ToDouble(grid[start_row, 11]);
				joint.localPosition.y = (float)Convert.ToDouble(grid[start_row, 12]);
				joint.localPosition.z = (float)Convert.ToDouble(grid[start_row, 13]);
				joint.parentRotation.x = (float)Convert.ToDouble(grid[start_row, 14]);
				joint.parentRotation.y = (float)Convert.ToDouble(grid[start_row, 15]);
				joint.parentRotation.z = (float)Convert.ToDouble(grid[start_row, 16]);
				joint.parentRotation.w = (float)Convert.ToDouble(grid[start_row, 17]);
				joint.transformPath = grid[start_row + 1, 0];
				if (grid[start_row+1,1] != null) joint.transformName = grid[start_row+row+1, 1];
				new_joints.Add(joint);
			}
			// Apply
			m_joints = new_joints;
			return true;
		}
		
		/**
		 * splits a T-pose file into a 2D string array
		 * @pre: each line is one row in the array, and entries are separated by commas, and no other commas are in the input
		 *       besides the separators
		 */
		static public string[,] SplitTPoseFile(string tPoseText) {
		
			Regex regex = new Regex("\r\n|\n");
			string[] lines = regex.Split(tPoseText);	
			
			// finds the max width of row
			int width = 0; 
			for (int i = 0; i < lines.Length; i++) {
				string[] row = lines[i].Split(','); 
				width = Mathf.Max(width, row.Length); 
			}
			
			
			// creates new 2D string grid to output to
			string[,] outputGrid = new string[lines.Length,width]; 
			for (int row = 0; row < lines.Length; row++) {
				string[] row_elements = lines[row].Split(',');
				for (int col = 0; col < row_elements.Length; col++) {
					// remove whitespace before getting string
					outputGrid[row,col] = row_elements[col].Trim(); 
				}
			}
			
			return outputGrid; 
		}
	}

}