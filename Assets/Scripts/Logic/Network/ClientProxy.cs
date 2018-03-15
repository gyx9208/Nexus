using Nexus.Logic.Base;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Nexus.Logic.Network
{
	public class ClientProxy : NetProxy
	{
		private NetworkConnection _Conn;

		public ClientProxy()
		{
			HeartBeatStart();
		}

		public override void Update()
		{
			base.Update();

			HeartBeatUpdate();
		}

		private bool IsConnected()
		{
			return _Conn != null;
		}

		public override void OnConnect(NetworkConnection conn)
		{
			base.OnConnect(conn);
			_Conn = conn;

			conn.RegisterHandler(MsgID.MSG_BASE, OnReceiveMsg);
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

		#region heartbeat
		CommonPing _Ping;

		private void HeartBeatStart()
		{
			_Ping = new CommonPing(GlobalConfig.GetGlobalParam().PingGap);
		}

		private void HeartBeatUpdate()
		{
			if (IsConnected())
				if (Time.time - _Ping.LastSendTime > _Ping.SendGap)
				{
					MsgBase msg = NetMsgPool.GetBase<MsgHeartBeat>(MsgID.MSG_HEART_BEAT);
					MsgHeartBeat hb = (MsgHeartBeat)msg.Message;
					hb.ClientTimeStamp = Time.time;
					_Conn.Send(MsgID.MSG_BASE, msg);

					_Ping.RequestPing(hb.ClientTimeStamp);
					NetMsgPool.ReturnBase(msg);
				}
		}

		private void OnReceiveHeartBeat(NetworkMessage netMsg, MsgBase msgBase)
		{
			var heartbeat = (MsgHeartBeat)msgBase.Message;

			_Ping.ReceivePing(heartbeat.ClientTimeStamp, (Time.time - heartbeat.ClientTimeStamp) / 2);
		}
		#endregion
	}
}
