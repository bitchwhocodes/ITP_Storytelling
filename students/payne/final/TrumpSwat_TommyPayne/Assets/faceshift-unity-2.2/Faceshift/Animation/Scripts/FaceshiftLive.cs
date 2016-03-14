/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace fs {
	
	public class FaceshiftLive : CharacterRetargeting, NetworkConnectionCallback {
		
		// public variables to access network
		public string hostName = "127.0.0.1";
		public int port = 33433;
		
		// Init variables ----------------------------
		private Mutex m_mutex;
		private NetworkConnection m_connection;
		private DataParser m_data_parser;
		private TrackData m_current_track;
		private Rig m_rig;
	
		void Start () {
			m_mutex = new Mutex();
			
			m_data_parser = new DataParser();
			m_current_track = new TrackData();
			
			m_connection = new NetworkConnection(this);
				if( !m_connection.Start(hostName,port,true) ) {
				Debug.LogError("could not access faceshift over the network on " + hostName + ":" + port + " using the TCP/IP protocol");
			}
	
			// get the blendshapes from fs studio
			AskForBlendshapeNames ();
	
			Init();
		}
		
		void Update () {
		
			bool new_data = false;
			m_mutex.WaitOne();
			// check if there are new blendshape names, and if so update
			if (NewBlendShapeNamesArrived()) {
				m_rig = Rig.GetRigFromBlendShapeNames(GetBlendShapeNames());
			}
	
			// get most recent tracking data
			while (m_data_parser.CountAvailableTracks() > 0) {
				m_current_track = m_data_parser.Dequeue();
				new_data = true;
			}
	
			// check that we have a rig (set up from blendshape names) with the same number of blendshapes as what we receive
			if (m_rig != null && m_current_track != null) {
				int n_track_coefficients = m_current_track.n_coefficients();
				int n_rig_coefficients = m_rig.NumShapes();
				if (n_track_coefficients != n_rig_coefficients) {
					Debug.LogWarning("number of coefficients of rig and tracking state have changed: " + n_track_coefficients + " vs " + n_rig_coefficients);
					// clear the current rig as it is not usable with the data coming from faceshift
					m_rig = null;
					m_current_track = null;
					// get again the blendshapes from fs studio
					AskForBlendshapeNames();
				}
			}
			m_mutex.ReleaseMutex();
	
			if (m_rig != null && m_current_track != null && m_Retargeting != null) {
				if (new_data && m_current_track.TrackSuccess) {
					// create a rig state
					RigState state = new RigState(m_rig);
					state.SetTimestamp(m_current_track.TimeStamp);
					state.SetBoneTranslation(0, m_current_track.HeadTranslation());
					state.SetBoneRotation(0, m_current_track.HeadRotation());
					state.SetBoneRotation(1, m_current_track.LeftEyeRotation());
					state.SetBoneRotation(2, m_current_track.RightEyeRotation());
					for (int i = 0; i < m_rig.NumShapes(); i++) {
						state.SetBlendshapeCoefficient(i, m_current_track.Coefficient[i]);
					}
					
					base.UpdateAnimation(m_rig, state);
				}
			}
		}
		
		void OnDestroy () {
		
			if (m_connection != null) m_connection.Stop();
			m_connection = null;
		}
		
		// Callback when new data arrives from faceshift studio
		public void OnNewData(byte[] data) {
		
			m_mutex.WaitOne();
			m_data_parser.ExtractData(data, false, 0.0);
			m_mutex.ReleaseMutex();
		}
		
		//! Asks faceshift studio to send the current blendshape names
		private bool AskForBlendshapeNames() {
		
			m_mutex.WaitOne();
			// send command to faceshift studio to get the blendshape names
			bool result = m_connection.SendCommand(44744);
			m_mutex.ReleaseMutex();
			return result;
		}
		
		//! @returns True if new blendshape names have arrived.
		private bool NewBlendShapeNamesArrived() {
			return m_data_parser.newBlendShapeNames;
		}
		
		//! @returns the List of blendshape names that are streamed from faceshift studio.
		private string[] GetBlendShapeNames() {
		
			m_mutex.WaitOne();
			string [] targetArray = null;
			if (m_data_parser.fsBlendShapeNames != null) {
				targetArray = new string[m_data_parser.fsBlendShapeNames.Length];
				m_data_parser.fsBlendShapeNames.CopyTo(targetArray, 0);
			}
			m_data_parser.newBlendShapeNames = false;
			m_mutex.ReleaseMutex();
			return targetArray;
		}
	}
	
}