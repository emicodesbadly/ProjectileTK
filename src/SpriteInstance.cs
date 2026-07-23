using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ProjectileTK.Rendering;
using ProjectileTK.Utilities;

namespace ProjectileTK
{
	public class SpriteInstance : GameObject
	{
		public static readonly Dictionary<string, List<SpriteInstance>> allInstances = [];

		private string spriteBaseID;

		private SpriteInstance(Sprite spriteBase, Vector2 position, float rotation)
			: base(spriteBase.id + " (instance)", position, rotation)
		{
			this.spriteBaseID = spriteBase.id;
		}

		public static SpriteInstance CreateSpriteInstance(string spriteBaseID, Vector2 position, float rotation)
		{
			if (!RenderingServer.Instance.GetSprite(spriteBaseID, out Sprite sprite))
			{
				Utils.ThrowWarning("SpriteInstance", $"SpriteInstance creation failed! No Sprite with id \"{spriteBaseID}\" exists!");

				return null;
			}

			SpriteInstance instance = new(sprite, position, rotation);

			if (!allInstances.TryGetValue(spriteBaseID, out List<SpriteInstance> l))
			{
				allInstances.Add(spriteBaseID, []);
				l = allInstances[spriteBaseID];
			}

			l.Add(instance);

			instance.Init();

			return instance;
		}

		// Calculate transformation matrix
		public Matrix4 CalculateTransformationMatrix()
		{
			// We start with the rotation
			Matrix4 transformation = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(transform.rotation));

			// Then we scale our sprite
			transformation *= Matrix4.CreateScale(transform.scale.X, transform.scale.Y, 1.0f);

			// Then we move it
			transformation *= Matrix4.CreateTranslation(transform.position.X, transform.position.Y, 0.0f);

			// Lastly we apply the world-to-screen matrix
			transformation *= RenderingServer.Instance.Screen.WorldToScreenMatrix;

			return transformation;
		}
	}
}
