namespace WyteEngine
{
	public class PlayerData
	{
		public string Name { get; set; }
		public int Life { get; set; }
		public int MaxLife { get; set; }
		public int FoodPoint { get; set; }
		public int WaterPoint { get; set; }

		public static int MaxFoodPoint => 50;
		public static int MaxWaterPoint => 50;

		public PlayerData(string name, int life, int mlife, int food, int water)
		{
			// ばりでいしょん
			if (food < 0)
				food = 0;
			if (life < 0)
				life = 0;
			if (water < 0)
				water = 0;

			// ばりばりでいしょん
			if (life > mlife)
				life = mlife;
			if (food > MaxFoodPoint)
				food = MaxFoodPoint;
			if (water > MaxWaterPoint)
				water = MaxWaterPoint;

			// せっと
			Name = name;
			Life = life;
			MaxLife = mlife;
			FoodPoint = food;
			WaterPoint = water;
		}

		public PlayerData(string name, int maxLife) : this(name, maxLife, maxLife, MaxFoodPoint, MaxWaterPoint) { }
	}
}