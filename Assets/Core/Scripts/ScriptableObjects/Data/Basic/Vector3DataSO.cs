using UnityEngine;

namespace CGL.Data
{
	[CreateAssetMenu(fileName = "Vector3Data", menuName = "CGL/Data/Basic/Vector3")]
	public class Vector3DataSO : BaseSO
	{
		public Vector3 value = Vector3.zero;
	}
}
