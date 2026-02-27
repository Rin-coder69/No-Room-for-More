using UnityEngine;

namespace CGL.Data
{
	[CreateAssetMenu(fileName = "TransformData", menuName = "CGL/Data/Basic/Transform")]
	public class TransformDataSO : BaseSO
	{
		public Transform value = null;
	}
}
