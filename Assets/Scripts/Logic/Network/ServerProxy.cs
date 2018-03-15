using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System;

namespace Nexus.Logic.Network
{
	public class ServerProxy : NetProxy
	{
		Dictionary<int, NetworkConnection> _ConnDic = new Dictionary<int, NetworkConnection>();

		public ServerProxy()
		{
			NetworkServer.RegisterHandler(MsgID.MSG_BASE, OnReceiveMsg);
		}

		public override void Update()
		{
			base.Update();
		}

		public override void OnConnect(NetworkConnection conn)
		{
			base.OnConnect(conn);
			if (_ConnDic.ContainsKey(conn.connectionId))
			{
				Debug.LogError("Something must error, should not connect same client");
			}
			_ConnDic[conn.connectionId] = conn;
		}

		private void OnReceiveMsg(NetworkMessage netMsg)
		{
			var baseMsg = NetMsgPool.Get<MsgBase>(MsgID.MSG_BASE);
			netMsg.ReadMessage(baseMsg);

			switch (baseMsg.ID)
			{
				case MsgID.MSG_HEART_BEAT:
					OnReceiveHeartBeat(netMsg, baseMsg);
					break;
			}
			NetMsgPool.ReturnBase(baseMsg);
		}

		private void OnReceiveHeartBeat(NetworkMessage netMsg, MsgBase msgBase)
		{
			var heartbeat = (MsgHeartBeat)msgBase.Message;
			heartbeat.ServerTimeStamp = Time.time;

			netMsg.conn.Send(MsgID.MSG_BASE, msgBase);
		}
	}
}
