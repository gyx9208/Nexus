using Nexus.Logic.Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Nexus.Logic.Network
{
	public class NexusNetworkManager : NetworkManager
	{
		#region Native
		private NetProxy _Proxy;
		private void Start()
		{
			Application.targetFrameRate = 200;
			UnityEngine.Network.sendRate = 100;

			var config = GlobalConfig.GetGlobalParam();
			if (config.IsServer)
			{
				networkPort = config.ServerPort;
				StartServer();
			}
			else
			{
				networkAddress = NetworkConstants.GetIPAddress(config.ServerIP);
				networkPort = config.ServerPort;
				StartClient();
			}
		}

		private void Update()
		{
			_Proxy.Update();
		}
		#endregion

		#region Client
		public override void OnStartClient(NetworkClient client)
		{
			base.OnStartClient(client);
			_Proxy = new ClientProxy();
			Debug.Log("OnStartClient");
		}

		public override void OnStopClient()
		{
			base.OnStopClient();
			Debug.Log("OnStopClient");
		}

		public override void OnClientConnect(NetworkConnection conn)
		{
			base.OnClientConnect(conn);
			_Proxy.OnConnect(conn);
			Debug.Log("OnClientConnect");
		}

		public override void OnClientDisconnect(NetworkConnection conn)
		{
			base.OnClientDisconnect(conn);
			Debug.Log("OnClientDisconnect");
		}

		public override void OnClientError(NetworkConnection conn, int errorCode)
		{
			base.OnClientError(conn, errorCode);
			Debug.Log("OnClientDisconnect");
		}

		public override void OnClientNotReady(NetworkConnection conn)
		{
			base.OnClientNotReady(conn);
			Debug.Log("OnClientNotReady");
		}

		public override void OnClientSceneChanged(NetworkConnection conn)
		{
			base.OnClientSceneChanged(conn);
			Debug.Log("OnClientSceneChanged");
		}
		#endregion

		#region Server
		public override void OnStartServer()
		{
			base.OnStartServer();
			_Proxy = new ServerProxy();
			Debug.Log("OnStartServer");
		}

		public override void OnStopServer()
		{
			base.OnStopServer();
			Debug.Log("OnStopServer");
		}

		public override void OnServerConnect(NetworkConnection conn)
		{
			base.OnServerConnect(conn);
			Debug.Log("OnServerConnect");
		}

		public override void OnServerDisconnect(NetworkConnection conn)
		{
			base.OnServerDisconnect(conn);
			Debug.Log("OnServerDisconnect");
		}
		#endregion

		#region Misc
		private void OnConnectedToServer()
		{
			Debug.Log("OnConnectedToServer");
		}

		private void OnFailedToConnect(NetworkConnectionError error)
		{
			Debug.Log("OnFailedToConnect");
		}

		private void OnPlayerConnected(NetworkPlayer player)
		{
			Debug.Log("OnPlayerConnected");
		}

		private void OnFailedToConnectToMasterServer(NetworkConnectionError error)
		{
			Debug.Log("OnFailedToConnectToMasterServer");
		}

		public override void OnDropConnection(bool success, string extendedInfo)
		{
			Debug.Log("OnDropConnection");
			base.OnDropConnection(success, extendedInfo);
		}
		#endregion
	}
}