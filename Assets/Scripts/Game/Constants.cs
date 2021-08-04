using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class Constants
	{
		// Angle in degrees.
		public const float START_SPAWN_ANGLE = 270;
		public const float RADIUS = 5;

		public const int CARD_COUNT_IN_DISTRIBUTION = 7;
		public const float CARD_INDENT = 0.2f;

		// Cards values.
		public const int SKIP_TURN_VALUE = 10;
		public const int PLUS_2_VALUE = 11;
		public const int CHANGE_TURN_ORDER_VALUE = 12;
		public const int CHANGE_COLOR_VALUE = 13;
		public const int CHANGE_COLOR_PLUS_4_VALUE = 14;

		// Game objects pathes.
		public const string UNO_BUTTON_PATH = "Prefabs/Uno_Button";
		public const string UNO_POP_UP_PATH = "Prefabs/Uno_PopUp";
		public const string TURN_ORDER_ROTATION_PATH = "Prefabs/Effects/Rotation";
	}
}
