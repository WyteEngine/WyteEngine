using System;
using System.Collections.Generic;

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

	public override bool Equals(object obj)
	{
		if (!(obj is ItemStack))
		{
			return false;
		}

		var stack = (ItemStack)obj;
		return Id == stack.Id &&
			   Amount == stack.Amount &&
			   EqualityComparer<IItem>.Default.Equals(Item, stack.Item);
	}

	public override int GetHashCode()
	{
		var hashCode = -511196713;
		hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
		hashCode = hashCode * -1521134295 + Amount.GetHashCode();
		hashCode = hashCode * -1521134295 + EqualityComparer<IItem>.Default.GetHashCode(Item);
		return hashCode;
	}

	public static bool operator ==(ItemStack i1, ItemStack i2) => i1.Equals(i2);
	public static bool operator !=(ItemStack i1, ItemStack i2) => !(i1 == i2);

	public static ItemStack operator +(ItemStack i1, int amount) => new ItemStack(i1.Id, i1.Amount + amount);
	public static ItemStack operator -(ItemStack i1, int amount) => new ItemStack(i1.Id, i1.Amount - amount);



}
