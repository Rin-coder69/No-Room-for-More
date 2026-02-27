using UnityEngine;

namespace CGL.Data
{
	[CreateAssetMenu(fileName = "GameObjectData", menuName = "CGL/Data/Basic/GameObject")]
	public class GameObjectDataSO : BaseSO
	{
		public GameObject value = null;
	}
}
