using UnityEngine;

namespace CGL.Utility
{
	public class DebugLabel : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Text to display in the debug label.")]
		private string labelText = "text";

		[SerializeField]
		[Tooltip("Size of the label box in pixels.")]
		private Vector2 boxSize = new Vector2(200, 40);

		[SerializeField]
		[Tooltip("Color of the label box.")]
		private Color color = Color.black;

		[SerializeField]
		[Tooltip("Offset from the game object position in world space.")]
		private Vector3 offset = new Vector3(0, 1, 0);

		private GUIStyle style;

		public string LabelText { get => labelText; set => labelText = value; }

		private void OnGUI()
		{
			// convert world position to screen position
			Vector3 worldPosition = transform.position + offset;
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

			// don't draw if behind the camera
			if (screenPosition.z < 0) return;

			// flip y axis (GUI origin is top-left, screen origin is bottom-left)
			screenPosition.y = Screen.height - screenPosition.y;

			// center the box on the screen position
			Rect rect = new Rect(
				screenPosition.x - boxSize.x / 2,
				screenPosition.y - boxSize.y / 2,
				boxSize.x,
				boxSize.y);

			// initialize style
			if (style == null)
			{
				style = new GUIStyle(GUI.skin.box);
				style.normal.textColor = Color.white;
				style.normal.background = MakeTexture(2, 2, color);
				style.alignment = TextAnchor.MiddleCenter;
				style.fontSize = 14;
			}

			GUI.Box(rect, labelText, style);
		}

		// creates a solid color texture for the background
		private Texture2D MakeTexture(int width, int height, Color color)
		{
			Color[] pixels = new Color[width * height];
			for (int i = 0; i < pixels.Length; i++)
				pixels[i] = color;

			Texture2D texture = new Texture2D(width, height);
			texture.SetPixels(pixels);
			texture.Apply();
			return texture;
		}
	}
}