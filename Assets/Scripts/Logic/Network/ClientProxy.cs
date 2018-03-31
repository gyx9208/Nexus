using Nexus.Logic.Base;
using Nexus.UI;
using Nexus.UI.Network;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Nexus.Logic.Network
{
	public class ClientProxy : NetProxy
	{
		private NetworkConnection _Conn;

		private MonoNetworkInfoUI _NetworkInfoUI;

		public ClientProxy()
		{
			GamePlayContext context = new GamePlayContext(true);

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

		protected void SendMessage(short msgID, MessageBase content) 
		{
			MsgBase msg = NetMsgPool.Get<MsgBase>(MsgID.MSG_BASE);
			msg.ID = msgID;
			msg.Message = content;
			_Conn.Send(MsgID.MSG_BASE, msg);
			NetMsgPool.ReturnBase(msg);
		}

		#region heartbeat
		CommonPing _Ping;

		private void HeartBeatStart()
		{
			_Ping = new CommonPing(GlobalConfig.GetGlobalParam().PingGap);
			_NetworkInfoUI = UGUITools.AddUI<MonoNetworkInfoUI>(MonoNetworkInfoUI.Path);
		}

		private void HeartBeatUpdate()
		{
			if (IsConnected())
				if (Time.time - _Ping.LastSendTime > _Ping.SendGap)
				{
					MsgHeartBeat hb = NetMsgPool.Get<MsgHeartBeat>(MsgID.MSG_HEART_BEAT);
					hb.ClientTimeStamp = Time.time;
					_Ping.RequestPing(hb.ClientTimeStamp);
					SendMessage(MsgID.MSG_HEART_BEAT, hb);
				}
			_NetworkInfoUI.UpdatePing(_Ping.Ping);
		}

		private void OnReceiveHeartBeat(NetworkMessage netMsg, MsgBase msgBase)
		{
			var heartbeat = (MsgHeartBeat)msgBase.Message;

			_Ping.ReceivePing(heartbeat.ClientTimeStamp, (Time.time - heartbeat.ClientTimeStamp) / 2);
		}
		#endregion
	}
}
