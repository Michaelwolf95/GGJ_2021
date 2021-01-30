using System;
using UnityEngine;

namespace MichaelWolfGames
{
	public static class InputManager
	{
		public enum InputAxis
		{
			MouseX,
			MouseY,
			Horizontal,
			Vertical
		}
		
		public enum InputButton
		{
			Use,
			Submit
		}
		
		public static float GetAxis(InputAxis axis)
		{
			string axisName = "";
#if PLATFORM_STANDALONE_WIN || PLATFORM_STANDALONE_LINUX
			axisName += "WIN_";
#elif UNITY_STANDALONE_OSX
			axisName += "OSX_";
#endif
			switch (axis)
			{
				case InputAxis.MouseX:
					axisName += "Mouse X";
					break;
				case InputAxis.MouseY:
					axisName += "Mouse Y";
					break;
				case InputAxis.Horizontal:
					axisName = "Horizontal";
					break;
				case InputAxis.Vertical:
					axisName = "Vertical";
					break;
			}
			return Input.GetAxis(axisName);
		}

		public static bool GetButton(InputButton button)
		{
			return Input.GetButton(GetButtonAlias(button));
		}
		
		public static bool GetButtonUp(InputButton button)
		{
			return Input.GetButtonUp(GetButtonAlias(button));
		}
		public static bool GetButtonDown(InputButton button)
		{
			return Input.GetButtonDown(GetButtonAlias(button));
		}

		private static string GetButtonAlias(InputButton button)
		{
			string buttonName = "";
#if PLATFORM_STANDALONE_WIN || PLATFORM_STANDALONE_LINUX
			buttonName += "WIN_";
#elif UNITY_STANDALONE_OSX
			buttonName += "OSX_";
#endif
			switch (button)
			{
				case InputButton.Use:
					buttonName += "Use";
					break;
				case InputButton.Submit:
					buttonName += "Submit";
					break;
			}

			return buttonName;
		}
	}
	
}