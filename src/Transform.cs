using OpenTK.Mathematics;

namespace ProjectileTK
{
	public class Transform
	{
		public Vector2 position;
		public float rotation;
		
		public Transform()
		{
			position = Vector2.Zero;
			rotation = 0f;
		}

		public Transform(Vector2 position, float rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}
	}
}
