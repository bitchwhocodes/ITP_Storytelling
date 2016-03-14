/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace fs
{
	/**
	 * @brief class TrackData contains the tracking data corresponding to a specific timestamp.
	 */
	public class TrackData {
	
	    private double          m_timestamp;
	    private bool            m_trackSuccess;
	    private Quaternion      m_headRotation;
	    private Vector3         m_headTranslation;
	    private float[]         m_coefficients;
	    private float           m_leftEyeTheta;
	    private float           m_leftEyePhi;
	    private float           m_rightEyeTheta;
	    private float           m_rightEyePhi;
	    private List<Vector3>   m_markersPositions;
	    private int             m_presence;
	
	    //! constructor
	    public TrackData(double timestamp,
	                     bool trackSuccess,
	                     Quaternion headRotation,
	                     Vector3 headTranslation,
	                     float[] coefficients,
	                     float leftEyeTheta,
	                     float leftEyePhi,
	                     float rightEyeTheta,
	                     float rightEyePhi,
	                     List<Vector3> markersPositions,
	                     int presence) {
	                     
	        m_timestamp = timestamp;
	        m_trackSuccess = trackSuccess;
	        m_headRotation = headRotation;
	        m_headTranslation = headTranslation;
	        m_coefficients = coefficients;
	        m_leftEyeTheta = leftEyeTheta;
	        m_leftEyePhi = leftEyePhi;
	        m_rightEyeTheta = rightEyeTheta;
	        m_rightEyePhi = rightEyePhi;
	        m_markersPositions = markersPositions;
	        m_presence = presence;
	
	    }
	
	    public TrackData(TrackData track) {
	        setFromTrack(track);
	    }
	
	    //! or create an "empty" trackData
	    public TrackData() {
	    
	        m_timestamp = 0.0;
	        m_trackSuccess = false;
	        m_headRotation = Quaternion.identity;
	        m_headTranslation = Vector3.zero;
	        m_coefficients = new float[48]; // 48 may change according to the faceshift version
	        m_leftEyeTheta = 0.0f;
	        m_leftEyePhi = 0.0f;
	        m_rightEyeTheta = 0.0f;
	        m_rightEyePhi = 0.0f;
	        m_markersPositions = new List<Vector3>();
	        m_presence = 0;
	
	    }
	
	    public void setFromTrack(TrackData track) {
	    
	        m_timestamp = track.m_timestamp;
	        m_trackSuccess = track.m_trackSuccess;
	        m_headRotation = track.m_headRotation;
	        m_headTranslation = track.m_headTranslation;
	        m_coefficients = track.m_coefficients;
	        m_leftEyeTheta = track.m_leftEyeTheta;
	        m_leftEyePhi = track.m_leftEyePhi;
	        m_rightEyeTheta = track.m_rightEyeTheta;
	        m_rightEyePhi = track.m_rightEyePhi;
	        m_markersPositions = track.m_markersPositions;
	        m_presence = track.m_presence;
	    }
	
	    //! @return The timestamp
	    public double TimeStamp {
	    
	        get { return m_timestamp; }
	    }
	
	    //! @return The success flag
	    public bool TrackSuccess {
	    
	        get { return m_trackSuccess; }
	    }
	
	    public Quaternion HeadRotation() {
	    
	        // we need to switch the coordinate system from right-hand (faceshift) to left-hand (unity)
	        return Utils.ChangeCoordinateSystem( m_headRotation );
	    }
	
	    public Quaternion HeadRotationMirrored() {
	    
	        // nothing to do as the change in coordinate system mirrors the motion
	        return m_headRotation;
	    }
	
	    public Vector3 HeadTranslation() {
	    
	        // we need to switch the coordinate system from right-hand (faceshift) to left-hand (unity)
	        return Utils.ChangeCoordinateSystem( m_headTranslation );
	    }
	
	    public Vector3 HeadTranslationMirrored() {
	    
	        // nothing to do as the change in coordinate system mirrors the motion
	        return m_headTranslation;
	    }
	
	    public int n_coefficients() { return m_coefficients.Length; }
	    
	    public float[] Coefficient {
	    
	        get { return m_coefficients; }
	    }
	
	    public float LeftEyeTheta()         { return m_leftEyeTheta; }
	    public float LeftEyeThetaMirrored() { return m_leftEyeTheta; }
	
	    public float LeftEyePhi() {
	    
	        // we need to switch the coordinate system from right-hand (faceshift) to left-hand (unity)
	        return -m_leftEyePhi;
	    }
	
	    public float LeftEyePhiMirrored() {
	    
	        // nothing to do as the change in coordinate system mirrors the motion
	        return m_leftEyePhi;
	    }
	
	    public Quaternion LeftEyeRotation() {
	    
	        return Quaternion.Euler(LeftEyeTheta(), LeftEyePhi(), 0);
	    }
	
	    public Quaternion LeftEyeRotationMirrored() {
	    
	        return Quaternion.Euler(LeftEyeThetaMirrored(), LeftEyePhiMirrored(), 0);
	    }
	
	    public float RightEyeTheta()         { return m_rightEyeTheta; }
	    public float RightEyeThetaMirrored() { return m_rightEyeTheta; }
	
	
	    public float RightEyePhi() {
	    
	        // we need to switch the coordinate system from right-hand (faceshift) to left-hand (unity)
	        return -m_rightEyePhi;
	    }
	
	    public float RightEyePhiMirrored() {
	    
	        // nothing to do as the change in coordinate system mirrors the motion
	        return m_rightEyePhi;
	    }
	
	    public Quaternion RightEyeRotation() {
	    
	        return Quaternion.Euler(RightEyeTheta(), RightEyePhi(), 0);
	    }
	
	    public Quaternion RightEyeRotationMirrored() {
	    
	        return Quaternion.Euler(RightEyeThetaMirrored(), RightEyePhiMirrored(), 0);
	    }
	
	    public List<Vector3> MarkersPositions {
	    
	        get { return m_markersPositions; }
	    }
	
	    public int Presence {
	    
	        get { return m_presence; }
	    }
	}

}
