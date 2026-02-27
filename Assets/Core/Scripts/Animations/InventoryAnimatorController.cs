using CGL.Events;
using CGL.Inventory;
using UnityEngine;

namespace CGL.Animation
{
	// drives animator parameters from inventory item state.
	// handles equip poses via animEquipName and use animations via animTriggerName.
	// subscribes to the same events as Inventory — assign the same SO assets in the inspector.
	[RequireComponent(typeof(AnimatorController))]
	public class InventoryAnimatorController : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Raised by inventory when the equipped item changes, passes the new item.")]
		private ItemEventSO onItemChangedEvent;

		[SerializeField]
		[Tooltip("Raised by inventory when the current item is used.")]
		private EventSO onUseEvent;

		private AnimatorController animatorController;
		private Item currentItem;
		private string currentPoseParameter;

		private void Awake()
		{
			animatorController = GetComponent<AnimatorController>();
		}

		private void OnEnable()
		{
			onItemChangedEvent?.Subscribe(OnItemChanged);
			onUseEvent?.Subscribe(OnUse);
		}

		private void OnDisable()
		{
			onItemChangedEvent?.Unsubscribe(OnItemChanged);
			onUseEvent?.Unsubscribe(OnUse);
		}

		private void OnItemChanged(Item item)
		{
			currentItem = item;

			// clear previous pose
			if (!string.IsNullOrEmpty(currentPoseParameter))
				animatorController.SetBoolFalse(currentPoseParameter);

			// set new pose from item data — empty animEquipName means no pose (unarmed)
			currentPoseParameter = item?.GetData()?.animEquipName;

			if (!string.IsNullOrEmpty(currentPoseParameter))
				animatorController.SetBoolTrue(currentPoseParameter);
		}

		private void OnUse()
		{
			string triggerName = currentItem?.GetData()?.animTriggerName;
			if (string.IsNullOrEmpty(triggerName)) return;

			animatorController.SetTrigger(triggerName);
		}
	}
}