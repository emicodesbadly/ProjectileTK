using System;
using OpenTK.Mathematics;

namespace ProjectileTK
{
	public class GameObject
	{
		public string name;
		public readonly Transform transform;

		protected GameObject(string name, Vector2 position, float rotation)
		{
			this.name = name;
			transform = new(position, rotation);
		}

		// Call this to create a new GameObject
		//   We can't use the constructor directly because of the constraints imposed by Init().
		//   Init() must be called at the end of the constructor, and it must be called EXACTLY ONCE.
		//   If we placed our Init() call in the constructor directly, we would be causing problems
		//   for child classes: either Init() would be called early, since the base constructor is
		//   called first, or it would be called multiple times, if we placed another call at the end
		//   of the child constructor.
		//   By using a function, we can ensure Init() is called EXACTLY ONCE, and only after the
		//   constructor has finished, even for child classes.
		public static GameObject CreateGameObject(string name, Vector2 position, float rotation)
		{
			GameObject gameObject = new(name, position, rotation);
			gameObject.Init();

			return gameObject;
		}

		// This is called immediately after the constructor finishes
		protected virtual void Init()
		{
			// Console.WriteLine($"({name}) Init");
		}
	}
}
