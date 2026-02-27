using UnityEngine;

namespace CGL.Data
{
	[CreateAssetMenu(fileName = "Vector2Data", menuName = "CGL/Data/Basic/Vector2")]
	public class Vector2DataSO : BaseSO
	{
		public Vector2 value = Vector2.zero;
	}
}
