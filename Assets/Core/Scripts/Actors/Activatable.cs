using CGL.Events;
using UnityEngine;

namespace CGL.Actor
{
	public abstract class Activatable : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Automatically activate on start.")]
		private bool activateOnAwake = true;

		[SerializeField]
		[Tooltip("Raise this event to activate.")]
		EventSO onActivateEvent;

		[SerializeField]
		[Tooltip("Raise this event to deactivate.")]
		EventSO onDeactivateEvent;

		private bool isActive = false;
		public bool IsActive => isActive;

		protected virtual void OnEnable()
		{
			onActivateEvent?.Subscribe(OnActivate);
			onDeactivateEvent?.Subscribe(OnDeactivate);
		}

		protected virtual void OnDisable()
		{
			onActivateEvent?.Unsubscribe(OnActivate);
			onDeactivateEvent?.Unsubscribe(OnDeactivate);
		}

		protected virtual void Start()
		{
			if (activateOnAwake) OnActivate();
		}

		public virtual void OnActivate() =>	isActive = true;
		public virtual void OnDeactivate() => isActive = false;
	}
}