using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nexus.UI;
using Nexus.UI.InGame;

namespace Nexus.Logic
{
	public class GamePlayContext
	{
		private bool _ShowUI;
		public GamePlayContext(bool showUI)
		{
			_ShowUI = showUI;
			if (_ShowUI)
			{
				new GamePlayUI(this);
			}
		}
	}

	public class GamePlayUI
	{
		private GamePlayContext _Context;
		private MonoInGameUI _InGameUI;
		public GamePlayUI(GamePlayContext context)
		{
			_Context = context;

			_InGameUI = UGUITools.AddUI<MonoInGameUI>(MonoInGameUI.PATH);
			
		}
	}
}
