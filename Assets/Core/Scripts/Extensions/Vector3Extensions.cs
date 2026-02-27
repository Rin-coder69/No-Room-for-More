using UnityEngine;

namespace CGL.Extensions
{
	public static class Vector3Extensions
	{
		// flatten vector to XZ plane (Y = 0)
		public static Vector3 VectorXZ(this Vector3 v)
		{
			return new Vector3(v.x, 0, v.z);
		}

		// horizontal magnitude ignoring Y
		public static float MagnitudeXZ(this Vector3 v)
		{
			return new Vector3(v.x, 0, v.z).magnitude;
		}

		// horizontal distance ignoring Y
		public static float DistanceXZ(this Vector3 a, Vector3 b)
		{
			Vector3 v = b - a;
			v.y = 0;
			return v.magnitude;
		}

		// clamp vector magnitude between a min and max length
		public static Vector3 ClampMagnitude(this Vector3 v, float minLength, float maxLength)
		{
			float magnitude = v.magnitude;
			if (magnitude == 0.0f) return v;

			float length = Mathf.Clamp(magnitude, minLength, maxLength);
			return v.normalized * length;
		}

		// check if vector is approximately zero
		public static bool IsZero(this Vector3 v)
		{
			return v.sqrMagnitude < 0.0001f;
		}

		// return direction to target
		public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
		{
			return (to - from).normalized;
		}

		// return direction to target on XZ plane
		public static Vector3 DirectionToXZ(this Vector3 from, Vector3 to)
		{
			return (to - from).VectorXZ().normalized;
		}

		// set individual components
		public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
		public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
		public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);
	}
}