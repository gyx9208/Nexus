using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Nexus.Logic.Proto
{
	public enum MsgId : short
	{
		MSG_HEART_BEAT=0,
	}

	public class MsgHeartBeat : MessageBase
	{
		public float ClientTimeStamp;
		public float ServerTimeStamp;
	}
}
