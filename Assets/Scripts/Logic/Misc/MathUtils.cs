using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Nexus.Logic.Misc
{
	public class MathUtils
	{
		public static string AccurateFloat(float input, int maxDotLength = 2)
		{
			float temp = input;
			for (int i = 0; i <= maxDotLength; i++)
			{
				if (Mathf.Abs(temp - Mathf.Round(temp)) < 0.01)
				{
					return FixedFloat(input, i);
				}
				temp = temp * 10;
			}
			return FixedFloat(input, maxDotLength);
		}

		public static string FixedFloat(float input, int dotLength)
		{
			if (dotLength == 0)
			{
				return Mathf.RoundToInt(input).ToString();
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < dotLength; i++)
				{
					input = input * 10;
				}
				sb.Append(Mathf.RoundToInt(input));

				for (int i = 0; i < dotLength + 1; i++)
				{
					if (sb.Length <= i || sb[sb.Length - i - 1] == '-')
					{
						sb.Insert(sb.Length - i, '0');
					}
				}

				sb.Insert(sb.Length - dotLength, '.');
				return sb.ToString();
			}

		}
	}
}