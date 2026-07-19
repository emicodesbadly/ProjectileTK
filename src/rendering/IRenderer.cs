using System;
using OpenTK.Mathematics;

namespace ProjectileTK.Rendering
{
	public interface IRenderer : IDisposable
	{
		Matrix4 CalculateTransformationMatrix();
		void Render();
	}
}
