using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nexus.Logic.Base
{
	public class CommonPing
	{
		#region define
		struct DropInfo
		{
			public float TimeStamp;
			public bool Droped;
		}

		public const int CAPBILITY = 20;

		private Queue<float> _RecentPings;
		private DropInfo[] _DropInfo;
		private float _Ping, _DropRate, _Variance, _LastReceivedTime, _LastSendTime, _SendGap;
		private int _DropInfoIndex;
		#endregion

		#region common
		public CommonPing(float sendGap)
		{
			_RecentPings = new Queue<float>(CAPBILITY);
			_DropInfo = new DropInfo[CAPBILITY];
			_SendGap = sendGap;
		}

		public void Initialize(float time)
		{
			Clear();
			_LastReceivedTime = time;
			_LastSendTime = time;
		}

		public void Clear()
		{
			_RecentPings.Clear();
			_Ping = 0;
			_DropRate = 0;
			_Variance = 0;
			_DropInfo = new DropInfo[CAPBILITY];
			_DropInfoIndex = 0;
			_LastReceivedTime = 0;
			_LastSendTime = 0;
		}
		#endregion

		#region get
		public float SendGap
		{
			get
			{
				return _SendGap;
			}
		}

		public float Ping
		{
			get
			{
				return _Ping;
			}
		}
		public float DropRate
		{
			get
			{
				return _DropRate;
			}
		}

		public float Variance
		{
			get
			{
				return _Variance;
			}
		}

		public float LastReceivedTime
		{
			get
			{
				return _LastReceivedTime;
			}
		}

		public float LastSendTime
		{
			get
			{
				return _LastSendTime;
			}
		}
		#endregion

		#region issues
		public void RequestPing(float timeStamp)
		{
			_DropInfo[_DropInfoIndex].TimeStamp = timeStamp;
			_DropInfo[_DropInfoIndex].Droped = true;
			_DropInfoIndex = GetNextIndex(_DropInfoIndex, CAPBILITY);

			_LastSendTime = timeStamp;
			if (timeStamp - _LastReceivedTime > _SendGap * 2)
			{
				EnPing(_Ping + (timeStamp - _LastReceivedTime - _SendGap) / 2);
			}
		}

		/// <summary>
		/// 收到ping返回包
		/// </summary>
		/// <param name="timeStamp">发送时间</param>
		/// <param name="ping">收到时间减去发送时间 除以2</param>
		public void ReceivePing(float timeStamp, float ping)
		{
			if (timeStamp < _LastReceivedTime)
			{//不按顺序到来的ping包一律当作丢包处理
				return;
			}

			_LastReceivedTime = timeStamp;
			EnPing(ping);

			int dropCount = 0;
			for (int i = 0; i < _DropInfo.Length; i++)
			{
				if (Mathf.Approximately(_DropInfo[i].TimeStamp, timeStamp))
				{
					_DropInfo[i].Droped = false;
				}
				if (_DropInfo[i].Droped)
				{
					dropCount++;
				}
			}
			_DropRate = (float)dropCount / CAPBILITY;
		}

		private void EnPing(float ping)
		{
			//Debug.Log("Enping: " + ping);
			float pingSum = ping + _Ping * _RecentPings.Count;

			if (_RecentPings.Count == CAPBILITY)
			{
				pingSum -= _RecentPings.Dequeue();
			}
			_RecentPings.Enqueue(ping);
			_Ping = pingSum / _RecentPings.Count;
		}

		private int GetNextIndex(int index, int count)
		{
			if (index + 1 < count)
			{
				return index;
			}
			else
			{
				return 0;
			}
		}
		#endregion
	}
}
