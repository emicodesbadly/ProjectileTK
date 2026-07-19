using OpenTK.Mathematics;

namespace ProjectileTK
{
	public class Transform
	{
		public Vector2 position;
		public float rotation;
		public Vector2 scale;
		
		public Transform()
		{
			position = Vector2.Zero;
			rotation = 0f;
			scale = Vector2.One;
		}

		public Transform(Vector2 position, float rotation)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = Vector2.One;
		}
	}
}
