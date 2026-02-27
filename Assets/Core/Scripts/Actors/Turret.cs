using CGL.Inventory;
using UnityEngine;

namespace CGL.Actor
{
	public class Turret : Actor
	{
		[SerializeField]
		[Tooltip("The weapon this turret fires.")]
		private Weapon weapon;

		public void Fire() => weapon?.Use();
		public void StopFire() => weapon?.StopUse();
	}
}