using Nexus.Logic.Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nexus.UI.Network
{
	public class MonoNetworkInfoUI : MonoBehaviour
	{
		public const string Path = "UI/Network/NetworkInfoUI";
		public Text PingText, FPSText;

		private float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;  
		private float m_UpdateShowDeltaTime = 1f;//更新帧率的时间间隔;  
		private int m_FrameUpdate = 0;//帧数;  
		private float m_FPS = 0;

		// Use this for initialization
		void Start()
		{
			m_LastUpdateShowTime = Time.realtimeSinceStartup;
		}

		// Update is called once per frame
		void Update()
		{
			m_FrameUpdate++;
			if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
			{
				m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
				m_FrameUpdate = 0;
				m_LastUpdateShowTime = Time.realtimeSinceStartup;
				FPSText.text = MathUtils.AccurateFloat(m_FPS, 0);
			}
		}

		public void UpdatePing(float ping)
		{
			PingText.text = MathUtils.AccurateFloat(ping*1000,0) + "ms";
		}
	}
}
