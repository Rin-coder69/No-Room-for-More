using UnityEngine;

namespace CGL.Inventory
{
	// defines the general category of an item
	public enum ItemType
	{
		Equipment,  // wearable or usable gear that provides passive benefits
		Weapon,     // items used to deal damage
		Consumable, // single-use items with immediate effects
		Resource,   // crafting or currency items
		Quest       // items related to quests or story progression
	}

	// defines how an item is used when activated
	public enum UsageType
	{
		Single, // one use per activation
		Auto,   // continuous automatic use while activated
		Burst,  // multiple uses in quick succession per activation
		Stream  // continuous stream effect while activated
	}

	// scriptable object that defines the properties and behavior of an item.
	// used as a template for creating item instances in the game.
	[CreateAssetMenu(fileName = "Item", menuName = "CGL/Inventory/Item")]
	public class ItemData : ScriptableObject
	{
		[Header("Basic Information")]
		[SerializeField]
		[Tooltip("Unique identifier for this item.")]
		public string id;

		[SerializeField]
		[Tooltip("Display name shown to the player.")]
		public string displayName;

		[SerializeField]
		[TextArea(3, 5)]
		[Tooltip("Description shown in inventory and tooltips.")]
		public string description;

		[SerializeField]
		[Tooltip("Icon displayed in inventory and UI.")]
		public Sprite icon;

		[SerializeField]
		[Tooltip("The general category this item belongs to.")]
		public ItemType itemType;

		[Header("Inventory Properties")]
		[SerializeField]
		[Tooltip("Whether multiple copies can be stacked in inventory.")]
		public bool allowMultiple = false;

		[SerializeField]
		[Tooltip("Maximum stack size if multiple copies are allowed.")]
		public int maxStackSize = 1;

		[SerializeField]
		[Tooltip("Whether this item can be equipped by the player.")]
		public bool equipable = false;

		[SerializeField]
		[Tooltip("How this item is used when activated.")]
		public UsageType usageType = UsageType.Single;

		[SerializeField]
		[Tooltip("Name of the bone or socket to attach this item to.")]
		public string attachBone = "";

		[Header("Animation")]
		[SerializeField]
		[Tooltip("Trigger name for usage animation.")]
		public string animTriggerName;

		[SerializeField]
		[Tooltip("Trigger name for equip animation.")]
		public string animEquipName;

		[SerializeField]
		[Tooltip("Weights for animation rig layers when item is equipped.")]
		public float[] rigLayerWeight;

		[Header("Prefabs")]
		[SerializeField]
		[Tooltip("Prefab instantiated when item is equipped.")]
		public GameObject itemPrefab;

		[SerializeField]
		[Tooltip("Prefab instantiated when item is dropped in the world.")]
		public GameObject pickupPrefab;

		private void OnValidate()
		{
			if (string.IsNullOrEmpty(id))
				Debug.LogWarning($"Item {name} is missing an ID!", this);

			if (equipable && itemPrefab == null)
				Debug.LogWarning($"Equipable item {name} has no item prefab assigned!", this);

			if (allowMultiple && maxStackSize <= 1)
				Debug.LogWarning($"Item {name} allows multiple but max stack size is {maxStackSize}.", this);
		}
	}
}