using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Nexus.Logic.Network
{
	public class ClientProxy : NetProxy
	{
		private NetworkConnection _Conn;

		public override void Update()
		{
			base.Update();

		}

		public override void OnConnect(NetworkConnection conn)
		{
			base.OnConnect(conn);
			_Conn = conn;
		}
	}
}
