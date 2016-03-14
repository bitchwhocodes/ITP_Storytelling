/**
 * Copyright 2012-2015 faceshift AG. All rights reserved.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace fs {
	/**
	 * @brief class ByteArrayCrawler handles data extraction from a byte stream.
	 */
	public class ByteArrayCrawler {
	
	    private byte[]      m_data;
	    private int         m_index = 0;
	
	    public ByteArrayCrawler() {
	        m_data = new byte[0];
	        m_index = 0;
	    }
	
	    public void Clear() {
	        m_data = new byte[0];
	        m_index = 0;
	    }
	
	    //! append new data and trim current data
	    public void Append(byte [] data) {
	    
	        byte [] new_data = new byte[m_data.Length + data.Length - m_index];
	        Array.Copy (m_data, m_index,          new_data, 0,            m_data.Length - m_index);
	        Array.Copy (data, m_data.Length, new_data, m_data.Length - m_index, data.Length);
	        m_data = new_data;
	        m_index = 0;
	    }
	
	    public void Trim() {
	    
	        byte [] new_data = new byte[m_data.Length - m_index];
	        Array.Copy (m_data, m_index, new_data, 0, new_data.Length);
	        m_index = 0;
	        m_data = new_data;
	    }
	
	    public int Pos() {
	        return m_index;
	    }
	
	    public void Seek(int pos) {
	        m_index = pos;
	    }
	
	    public bool DataAvailable() {
	        return (m_index < m_data.Length);
	    }
	
	    public ushort ExtractUshort() {
	        ushort value = BitConverter.ToUInt16(m_data, m_index);
	        m_index += 2;
	        return value;
	    }
	
	    public double ExtractDouble() {
	        double value = BitConverter.ToDouble(m_data, m_index);
	        m_index += 8;
	        return value;
	    }
	
	    public bool ExtractBool() {
	        bool value = BitConverter.ToBoolean(m_data, m_index);
	        m_index += 1;
	        return value;
	    }
	
	    public float ExtractFloat() {
	        float value = BitConverter.ToSingle(m_data, m_index);
	        m_index += 4;
	        return value;
	    }
	
	    public uint ExtractUint() {
	        uint value = BitConverter.ToUInt32(m_data, m_index);
	        m_index += 4;
	        return value;
	    }
	
	    public int ExtractInt32() {
	        int value = BitConverter.ToInt32(m_data, m_index);
	        m_index += 4;
	        return value;
	    }
	
	    public Quaternion ExtractQuaternion() {
	        float x = ExtractFloat ();
	        float y = ExtractFloat ();
	        float z = ExtractFloat ();
	        float w = ExtractFloat();
	
	        return new Quaternion(x, y, z, w);
	    }
	
	    public Vector3 ExtractVector3() {
	        float x = ExtractFloat();
	        float y = ExtractFloat();
	        float z = ExtractFloat();
	
	        return new Vector3(x, y, z);
	    }
	
	    public float[] ExtractCoefficients(uint howMany) {
	    
	        float[] result = new float[howMany];
	        for (int i = 0; i < howMany; i++) {
	            result[i] = ExtractFloat();
	        }
	
	        return result;
	    }
	
	    public List<Vector3> ExtractMarkersPositions(uint howMany) {
	    
	        List<Vector3> result = new List<Vector3>();
	        for (int i = 0; i < howMany; i++) {
	            result.Add(ExtractVector3());
	        }
	
	        return result;
	    }
	
	    /**
	     * Used for the blend shape names, which are sent as strings over the network.
	     */
	    public string ExtractString() {
	    
	        ushort stringLength = ExtractUshort();
	        string resultString = System.Text.Encoding.UTF8.GetString(m_data, m_index, stringLength);
	        m_index += stringLength;
	        return resultString;
	    }
	}
}
