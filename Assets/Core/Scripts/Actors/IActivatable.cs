using UnityEngine;

namespace CGL.Actor
{
	public interface IActivatable
	{
		void OnActivate();
		void OnDeactivate();
	}
}