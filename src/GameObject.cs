using OpenTK.Mathematics;

namespace ProjectileTK
{
	public class GameObject
	{
		public string name;
		public readonly Transform transform;

		public GameObject()
		{
			name      = "New GameObject";
			transform = new();
		}

		public GameObject(string name)
		{
			this.name = name;
			transform = new();
		}

		public GameObject(string name, Vector2 position, float rotation)
		{
			this.name = name;
			transform = new(position, rotation);
		}
	}
}
