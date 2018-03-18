using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nexus.UI
{
	public class UGUITools
	{
		public static string CANVAS_PATH = "UGUICanvas";
		public static T AddUI<T>(string Path) where T : MonoBehaviour
		{
			var go = GameObject.Instantiate(Resources.Load(Path)) as GameObject;
			var canvas = GameObject.Find(CANVAS_PATH);
			go.transform.SetParent(canvas.transform);
			T t = go.GetComponent<T>();
			return t;
		}
	}
}
