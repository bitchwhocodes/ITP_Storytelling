/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace fs {

	public class DataParser {
	
	    private ByteArrayCrawler m_crawler;
	    private bool m_valid = true;
	
	    private Queue<TrackData> m_queueOfTrackData;
	    public string[] fsBlendShapeNames = null;
	    public bool newBlendShapeNames = false;
	
	    public DataParser() {
	    
	        m_queueOfTrackData = new Queue<TrackData>();
	        m_crawler = new ByteArrayCrawler();
	    }
	
	    void Clear() {
	    
	        m_queueOfTrackData.Clear();
	        m_crawler.Clear();
	        m_valid = true;
	        newBlendShapeNames = false;
	        fsBlendShapeNames = null;
	    }
	
	    public bool Valid() {
	    
	        return m_valid;
	    }
	
		public void ExtractData(byte[] data, bool manuallySetTimestamp, double timestamp) {
		
	        m_crawler.Append(data);
	
	        while (m_crawler.DataAvailable() && m_valid) {
	
	            // initialise variables
	            // when magic number == 33433, we get
	            // BLOCK 101
				if (!manuallySetTimestamp) {
		            timestamp = 0.0;
		        }
	            bool trackSuccess = false;
	            // BLOCK 102
	            Quaternion headRotation = Quaternion.identity;
	            Vector3 headTranslation = Vector3.zero;
	            // BLOCK 103
	            float[] coefficients = new float[0];
	            // BLOCK 104
	            float leftEyeTheta = 0.0f;
	            float leftEyePhi = 0.0f;
	            float rightEyeTheta = 0.0f;
	            float rightEyePhi = 0.0f;
	            // BLOCK 105
	            List<Vector3> markersPositions = new List<Vector3>();
	            markersPositions.Capacity = 14;
	
	            // when magic number == 55355, we get
	            int presence = 2;
	
	            ushort magicNumber = m_crawler.ExtractUshort();
	            /*ushort version = */m_crawler.ExtractUshort();
	            uint messageSize = m_crawler.ExtractUint();
	            int pos = m_crawler.Pos();
	
	            if (messageSize < 0) m_valid = false;
	
	            if (magicNumber == 55355) {
	                presence = m_crawler.ExtractInt32();
	                Debug.Log("presence message received: " + presence);
	            } else if (magicNumber == 33633) {
	                // Receiving blend shape names
	                ushort numberOfBlendShapes = m_crawler.ExtractUshort();
	                fsBlendShapeNames = new string[numberOfBlendShapes];
	
	                for (ushort j = 0; j < numberOfBlendShapes; j++) {
	                    fsBlendShapeNames[j] = m_crawler.ExtractString();
	                }
	
	                newBlendShapeNames = true;
	            } else if (magicNumber == 33433) {
	                ushort nbBlocks = m_crawler.ExtractUshort();
	
	                for (int i = 0; i < nbBlocks; i++) {
	                    ushort blockID = m_crawler.ExtractUshort();
	                    /*ushort blockVersion =*/ m_crawler.ExtractUshort();
	                    uint blockSize = m_crawler.ExtractUint();
	                    int pos_start = m_crawler.Pos();
	
	                    switch (blockID) {
	                        case 101: {
	                            double timestampFromData = m_crawler.ExtractDouble();
								if (!manuallySetTimestamp) {
									timestamp = timestampFromData;
								}
	                            trackSuccess = m_crawler.ExtractBool();
	                        } break;
	
	                        case 102: {
	                            headRotation = m_crawler.ExtractQuaternion();
	                            headTranslation = m_crawler.ExtractVector3();
	                        } break;
	
	                        case 103: {
	                            uint nbCoefficients = m_crawler.ExtractUint();
	                            coefficients = m_crawler.ExtractCoefficients(nbCoefficients);
	                        } break;
	
	                        case 104: {
	                            leftEyeTheta = m_crawler.ExtractFloat();
	                            leftEyePhi = m_crawler.ExtractFloat();
	                            rightEyeTheta = m_crawler.ExtractFloat();
	                            rightEyePhi = m_crawler.ExtractFloat();
	                        } break;
	
	                        case 105: {
	                            ushort nbMarkers = m_crawler.ExtractUshort();
	                            markersPositions.AddRange( m_crawler.ExtractMarkersPositions(nbMarkers) );
	                        } break;
	
	                        case 110: {
	                            presence = m_crawler.ExtractUshort();
	                        } break;
	
	                        default: {
	                            //Debug.LogWarning("Unknown block " + blockID);
	                            // do not make message invalid as it may just be an unknown block
	                            m_crawler.Seek(m_crawler.Pos() + (int)blockSize);
	                        } break;
	                    }
	
	                    int pos_end = m_crawler.Pos();
	
	                    if (pos_end - pos_start != blockSize) m_valid = false;
	                }
	
		            TrackData trackdata = new TrackData(    timestamp,
		                                                    trackSuccess,
		                                                    headRotation,
		                                                    headTranslation,
		                                                    coefficients,
		                                                    leftEyeTheta,
		                                                    leftEyePhi,
		                                                    rightEyeTheta,
		                                                    rightEyePhi,
		                                                    markersPositions,
		                                                    presence);
	
		            m_queueOfTrackData.Enqueue(trackdata);
	            } else {
	                Debug.Log("unknown message block " + magicNumber);
	                // step to next block
	                m_crawler.Seek(pos + (int)messageSize);
	            }
	
	            if (!m_valid || m_crawler.Pos() != pos + (int)messageSize) {
	                Debug.LogError("invalid data received");
	                m_valid = false;
	            }
	
	        }
	
	        // get rid of unnecessary data in crawler
	        m_crawler.Trim();
	    }
	
	    public TrackData Dequeue() {
	    
	        if (CountAvailableTracks() < 1) {
	            throw new Exception("DataParser#Dequeue -> attempting dequeuing on empty queue");
	        }
	
	        return m_queueOfTrackData.Dequeue();
	
	    }
	
	    public int CountAvailableTracks() {
	    
	        return m_queueOfTrackData.Count;
	    }
	}

}
