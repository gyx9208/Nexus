using Nexus.Logic.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Nexus.Logic.Network
{
	public class NetMsgPool
	{
		private static NetMsgPool _Inst;
		private NestedCommonPool<short, MessageBase> _MsgPool;

		private NetMsgPool()
		{
			_MsgPool = new NestedCommonPool<short, MessageBase>();
		}

		private static NetMsgPool Inst
		{
			get
			{
				if (_Inst == null)
					_Inst = new NetMsgPool();
				return _Inst;
			}
		}

		public static MsgBase GetBase<T>(short key) where T : MessageBase, new()
		{
			var ret = Inst._MsgPool.Get<MsgBase>(MsgID.MSG_BASE);
			ret.ID = key;
			ret.Message = Get<T>(key);

			return ret;
		}

		public static T Get<T>(short key) where T:MessageBase, new()
		{
			return Inst._MsgPool.Get<T>(key);
		}

		public static void ReturnBase(MsgBase msg)
		{
			Inst._MsgPool.Return(msg.ID, msg.Message);
			msg.Message = null;
			Inst._MsgPool.Return(MsgID.MSG_BASE, msg);
		}

		public static void Return(short key, MessageBase msg)
		{
			Inst._MsgPool.Return(key, msg);
		}

		public static void Destroy()
		{
			if (_Inst != null)
				_Inst._MsgPool.Destroy();
			_Inst = null;
		}
	}

	public class MsgID
	{
		public const short MSG_BASE = 100;
		public const short MSG_HEART_BEAT = 101;
	}

	public class MsgBase : MessageBase
	{
		public short ID;
		public MessageBase Message;

		public MsgBase()
		{

		}

		public MsgBase(short id)
		{
			ID = id;
			CreateMessage();
		}

		private void CreateMessage()
		{
			switch (ID)
			{
				case MsgID.MSG_HEART_BEAT:
					Message = NetMsgPool.Get<MsgHeartBeat>(ID);
					break;
			}
		}

		public override void Deserialize(NetworkReader reader)
		{
			ID = reader.ReadInt16();
			CreateMessage();
			Message.Deserialize(reader);
		}

		public override void Serialize(NetworkWriter writer)
		{
			writer.Write(ID);
			Message.Serialize(writer);
		}
	}

	public class MsgHeartBeat : MessageBase
	{
		public float ClientTimeStamp;
		public float ServerTimeStamp;
	}
}
