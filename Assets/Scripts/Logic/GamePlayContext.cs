using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nexus.Logic.Controller;
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
		private PlayerGestureInput _PlayerGestureInput;
		public GamePlayUI(GamePlayContext context)
		{
			_Context = context;
			_PlayerGestureInput = new PlayerGestureInput();
			_InGameUI = UGUITools.AddUI<MonoInGameUI>(MonoInGameUI.PATH);
			_InGameUI.Regist(_PlayerGestureInput);
			_PlayerGestureInput.Regist(_Context);
		}
	}
}
