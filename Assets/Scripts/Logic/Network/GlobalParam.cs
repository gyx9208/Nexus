using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nexus.Logic.Network
{
	[CreateAssetMenu(menuName ="Nexus/Multi Player/Global Param", order = 1)]
	public class GlobalParam : ScriptableObject
	{
		public bool IsServer = false;

		public string ServerIP = "127.0.0.1";
		public int ServerPort = 1019;
	}

	public class GlobalConfig
	{
		private const string GLOBAL_PARAM_PATH = "Data/Multiplayer/GlobalParam";

		private static GlobalParam _globalParam;
		public static GlobalParam GetGlobalParam()
		{
			if (_globalParam == null)
				_globalParam = Resources.Load(GLOBAL_PARAM_PATH) as GlobalParam;
			return _globalParam;
		}
	}
}
