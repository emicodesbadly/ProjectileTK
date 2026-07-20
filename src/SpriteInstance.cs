using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ProjectileTK.Rendering;
using ProjectileTK.Utilities;

namespace ProjectileTK
{
	public class SpriteInstance : GameObject
	{
		private SpriteInstance(Sprite spriteBase, Vector2 position, float rotation)
			: base(spriteBase.id + " (instance)", position, rotation)
		{

		}

		public static SpriteInstance CreateSprite(string spriteBase, Vector2 position, float rotation)
		{
			if (RenderingServer.Instance.GetSprite(spriteBase, out Sprite sprite))
			{
				Utils.ThrowWarning("SpriteInstance", $"SpriteInstance creation failed! No Sprite with id \"{spriteBase}\" exists!");

				return null;
			}

			SpriteInstance instance = new(sprite, position, rotation);
			instance.Init();

			return instance;
		}

		// Calculate transformation matrix
		public Matrix4 CalculateTransformationMatrix()
		{
			// We start with the identity matrix, which applies no transformation
			Matrix4 transformation = Matrix4.Identity;

			// First we scale our sprite
			transformation *= Matrix4.CreateScale(transform.scale.X, transform.scale.Y, 1.0f);

			// Then we rotate it
			transformation *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(transform.rotation));

			// Lastly we move it
			transformation *= Matrix4.CreateTranslation(transform.position.X, transform.position.Y, 0.0f);

			return transformation;
		}
	}
}
