/**
 *  Copyright 2012-2015 faceshift AG. All rights reserved.
 * 
 *  Script co-written by @David_Hodgetts -- http://www.demainlalune.ch/
 *  and @raphael_munoz -- http://www.aprobado.ch/
 */

using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace fs {
	
	/**
	 * @brief interface NetworkConnectionCallback provides the callback functionality for network data.
	 * @ warning The callback happens in a separate thread, so the callback object has to handle concurrency.
	 */
	public interface NetworkConnectionCallback {
		void OnNewData(byte[] data);
	}
	
	
	public class NetworkConnection {
	
		//! @brief Hostname to which to connect (only applicable for TCP/IP connections).
		private volatile string m_host_name = "127.0.0.1";
		
		//! @brief Port to which to connect (for TCP/IP) or on which to listen (for UDP)
		private volatile int m_port = 33433;
	
		//! @brief Flag whether to use TCP/IP or UDP for the connection.
		private volatile bool m_use_tcp_ip = true;
	
		//! @brief Flag whether to stop the connection.
		private volatile bool m_should_stop = false;
	
		//! @brief Flag whether connection was successful. 0 if not yet started, -1 if error on connection, 1 if successful.
		private volatile int m_status = 0;
		
		//! @brief Worker thread handling the connection.
		private Thread m_connection_thread;
		
		//! @brief Callback object which receives the data from the network.
		private NetworkConnectionCallback m_callback;
	
		//! @brief Network stream to send and receive data.
		private NetworkStream m_stream = null;
	    
		//! @brief Registers a callback with the network connection. The callback receives the data from the network.
		public NetworkConnection(NetworkConnectionCallback callback) {
				m_callback = callback;
	    }
		
		//! @brief Starts the network connection using either TCP/IP or UDP.
		//! @param host_name  Host to which to connect for TCP/IP, for UDP this is ignored.
		//! @param port		  Port to which to connect for TCP/IP, or on which to listen for UDP.
		//! @param use_tcp_ip Flag whether to use TCP/IP (true) or UDP (false)
		//! @returns True if network connection could be started, false otherwise.
		public bool Start(string host_name, int port, bool use_tcp_ip) {
		
			if (m_status != 0) {
				Stop();
			}
			
			m_status = 0;
			m_host_name = host_name;
			m_port = port;
			m_use_tcp_ip = use_tcp_ip;
			
			Debug.Log("start network connection");
			m_should_stop = false;
			// the network connection is handled in a separate thread.
			m_connection_thread = new Thread(new ThreadStart(NetworkReceiveLoop));
			m_connection_thread.Start();
			
			// wait for a maximum amount of time to see whether start was successful
			double connection_start_time = Time.realtimeSinceStartup;
			double max_wait_time = 5;
			while( m_status == 0 && (Time.realtimeSinceStartup - connection_start_time) < max_wait_time) { 
				Thread.Sleep(3); 
			}
	
			int status = m_status;
			if (status == 0) {
				// status 0 as there is a timeout - requires stop of the connection
				Stop ();
			}
			
			return (status > 0);
	    }
	
		//! Stops the network connection and waits for the network thread to finish.
	    public void Stop() {
	    
			Debug.Log("stop network connection");
			m_should_stop = true;
			m_connection_thread.Join();
			m_status = 0;
	    }
		
	    //! Sends a command to faceshift studio. Only work in TCP/IP mode.
		public bool SendCommand(ushort command) {
		
			if (m_stream == null || !m_use_tcp_ip) {
				return false;
			}
			try{
				Debug.Log("sending command " + command);
				// The data to send
				byte[] commandB = BitConverter.GetBytes(command);
				ushort versionNumber = 1;
				byte[] versionNumberB = BitConverter.GetBytes(versionNumber);
				uint payloadSize = 0;
				byte[] payloadSizeB = BitConverter.GetBytes(payloadSize);
				
				// Concatenate into one block:
				byte[] datablock = new byte[commandB.Length + versionNumberB.Length + payloadSizeB.Length];
				System.Buffer.BlockCopy(commandB, 0, datablock, 0, commandB.Length);
				System.Buffer.BlockCopy(versionNumberB, 0, datablock, commandB.Length, versionNumberB.Length);
				System.Buffer.BlockCopy(payloadSizeB, 0, datablock, commandB.Length + versionNumberB.Length, payloadSizeB.Length);
				
				// Send
				if(m_use_tcp_ip) {
					m_stream.Write(datablock, 0, datablock.Length);
				}
				
			} catch(Exception e) {
				Debug.LogError("network send exception: " + e.Message);
				Debug.LogError("network send exception: " + e.StackTrace);
				m_status = -1;
				return false;
			}
			return true;	
		}
	
		//! Loop receiving data from either TCP/IP or UDP. Passes the received payload to the callback.
		//! Receives data until Stop() is called.
		private void NetworkReceiveLoop() {
		
			try{
				if( m_use_tcp_ip )
				{
					Debug.Log ("start network receive loop");
					// TCP/IP connection
					TcpClient tcp_client = new TcpClient(m_host_name,m_port);
					m_stream = tcp_client.GetStream();
					m_status = 1;
					tcp_client.ReceiveBufferSize = 8192;
					int receive_buffer_size = tcp_client.ReceiveBufferSize;
					
					while(!m_should_stop && IsConnected(tcp_client)){
						if( m_stream.DataAvailable ) {
							byte[] bytes = new byte[receive_buffer_size];
							int bytesRead = m_stream.Read(bytes,0,receive_buffer_size);
							if( bytesRead > 0 ) {
								System.Array.Resize(ref bytes, bytesRead);
								m_callback.OnNewData(bytes);
							}
								Thread.Sleep(10);
						} else {
							Thread.Sleep(10); // sleep before checking again
						}
			        }
	
					if (!IsConnected(tcp_client)) {
						//m_callback.OnRemoteDisconnected();
						Debug.Log ("remote host closed connection");
					}
					Debug.Log ("closing the connection");
					tcp_client.Close();
				}
				else
				{
					// UDP connection
					UdpClient udp_client = new UdpClient(m_port);
					m_status = 1;
					
					while(!m_should_stop){
						
						int bytesAvailable = udp_client.Available;
						if( bytesAvailable > 0 ) {
							IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 33433);
							byte[] bytes = udp_client.Receive(ref groupEP);
							m_callback.OnNewData(bytes);
						}
						else {
							Thread.Sleep(10); // sleep before checking again
						}
			        }
		 
					udp_client.Close();
				}
				
			
			} catch(Exception e) {
	        	Debug.LogError("network receive exception: " + e.Message);
				Debug.LogError("network receive exception stack trace: " + e.StackTrace);
				m_status = -1;
	        }
	    }
	
		//! Checks whether the tcpclient is still connected to the remote host.
		bool IsConnected(TcpClient tcp) {
		
			// Detect if client disconnected
			if( tcp.Client.Poll( 0, SelectMode.SelectRead ) )
			{
				byte[] buff = new byte[1];
				if( tcp.Client.Receive( buff, SocketFlags.Peek ) == 0 )
				{
					// Client disconnected
					return false;
				}
			}
			return true;
		}
	}
	
}