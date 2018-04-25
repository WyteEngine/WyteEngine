using System;

[Serializable]
public struct ItemStack
{
	public string Id { get; }
	public int Amount { get; private set; }
	public IItem Item => ItemManager.Instance[Id];

	public ItemStack(string id, int amount = 1)
	{
		Id = id;
		Amount = amount;
	}

	public void Add(int amount) => Amount += amount;
	public void Set(int amount) => Amount = amount;

	public static bool operator ==(ItemStack i1, ItemStack i2) => i1.Equals(i2);
	public static bool operator !=(ItemStack i1, ItemStack i2) => !(i1 == i2);

	public static ItemStack operator +(ItemStack i1, int amount) => new ItemStack(i1.Id, i1.Amount + amount);
	public static ItemStack operator -(ItemStack i1, int amount) => new ItemStack(i1.Id, i1.Amount - amount);

}
