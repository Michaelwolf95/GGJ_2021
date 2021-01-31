using System;

using UnityEngine.InputSystem;

namespace MichaelWolfGames
{
	[Serializable]
	public class EventButton
	{
		[Serializable]
		public enum ButtonState
		{
			None,
			Pressed,
			Hold,
			Released
		}
		private ButtonState _state = ButtonState.None;
		public ButtonState state
		{
			get => _state;
			private set => _state = value;
		}

		private PlayerInput inputController;
		private string buttonName;
		
		public float timeHeld { get; private set; }

		public bool isPressed => isPressedDown || isHeld;
		public bool isPressedDown => state == ButtonState.Pressed;
		public bool isHeld => state == ButtonState.Hold;
		public bool isReleased => state == ButtonState.Released;

		public EventButton(PlayerInput argInputController, string argButtonName)
		{
			buttonName = argButtonName;
			inputController = argInputController;
		}

		public void HandleUpdate(float argDeltaTime)
		{
			bool isPressed = inputController.actions[buttonName].ReadValue<float>() != 0;
			switch (state)
			{
				case ButtonState.None:
					if (isPressed)
					{
						state = ButtonState.Pressed;
						timeHeld = 0f;
					}
					break;
				case ButtonState.Pressed:
					timeHeld = 0;
					if (isPressed)
					{
						state = ButtonState.Hold;
						
					}
					else
					{
						state = ButtonState.Released;
					}
					break;
				case ButtonState.Hold:
					if (isPressed)
					{
						timeHeld += argDeltaTime;
					}
					else
					{
						state = ButtonState.Released;
						timeHeld = 0f;
					}
					break;
				case ButtonState.Released:
					timeHeld = 0f;
					if (isPressed)
					{
						state = ButtonState.Pressed;
					}
					else
					{
						state = ButtonState.None;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

	}
}