/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
using System;
using System.IO;

namespace fs {

	/**
	 * class FsbReader allows reading of fsb files that contain animation clips exported from faceshift studio.
	 */
	public class FsbReader {
	
	    /**
	     * @brief Reads an fsb file and returns a clip constructed from the the content of the fsb file.
	     * @param[in] filename  The filename of the fsb file.
	     * @return A clip with the track data imported from the fsb file, null if there was some error during loading.
	     */
	    public static Clip Read(string filename) {
	    
			if (!File.Exists (filename)) return null;
	
	        FileStream fileReader = File.OpenRead(filename); 
	        int size = (int) fileReader.Length;
				 
	        byte [] data = new byte[size];
	        int size_read = fileReader.Read(data, 0, size);
			if (size != size_read) {
				Debug.LogError("Reading file unsuccessful: read " + size_read + " bytes instead of " + size + " bytes");
				return null;
			}
			return Read(data);
	     }
	
	    /**
	     * @brief Reads an fsb file and returns a clip constructed from the the content of the fsb file.
	     * @param[in] data  The binary data from the fsb file.
	     * @return A clip with the track data imported from the fsb file, null if there was some error during loading.
	     */
	    public static Clip Read(byte[] data) {
	
	        DataParser parser = new DataParser();
	        parser.ExtractData(data, false, 0.0);
	
	        if (parser.Valid()) {
	
	            Rig rig = new Rig();
	
	            // add hardcoded bone names of fsb file
	            rig.AddBone("Neck");
	            rig.AddBone("EyeLeft");
	            rig.AddBone("EyeRight");
	
	            for (int i = 0; i < parser.fsBlendShapeNames.Length; i++) {
	                rig.AddShape(parser.fsBlendShapeNames[i]);
	            }
	
	            Clip clip = new Clip(rig);
	
	            while (parser.CountAvailableTracks() > 0) {
	                TrackData track_data = parser.Dequeue();
	
	                if (!track_data.TrackSuccess) {
	                    // do not save the state if the tracking data is not valid
	                    Debug.LogWarning("this frame doesn't contain valid tracking data");
	                    continue;
	                }
	
	                RigState state = clip.NewState(track_data.TimeStamp);
	                
					state.SetTrackingSuccessful(track_data.TrackSuccess);
	
	                if (rig.NumShapes() != track_data.n_coefficients()) {
	                    Debug.LogError("num blendshapes do not agree in file with " + rig.NumShapes() +
	                                   " in rig and " + track_data.n_coefficients() + " in state");
	                    return null;
	                }
	
	                // Assume the head translation to be joint 0 (Neck)
	                state.SetBoneTranslation(0, track_data.HeadTranslation());
	
	                // bone indices same as the order when added to the rig
	                state.SetBoneRotation(0, track_data.HeadRotation());
	                state.SetBoneRotation(1, track_data.LeftEyeRotation());
	                state.SetBoneRotation(2, track_data.RightEyeRotation());
	
	                for (int i = 0; i < track_data.n_coefficients(); i++) {
	                    state.SetBlendshapeCoefficient(i, track_data.Coefficient[i]);
	                }
	            }
	
	            return clip;
	        } else {
	            Debug.LogError("cannot parse fsb file");
	        }
	
	        return null;
	    }
	}
}

