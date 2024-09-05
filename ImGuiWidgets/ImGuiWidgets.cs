namespace ktsu.io.ImGuiWidgets;

using System.Numerics;

public static partial class ImGuiWidgets
{
	private static Stack<float> Scale { get; } = [];

	public static void PushScale(float scale) => Scale.Push(scale);
	public static void PopScale() => Scale.Pop();
	public static float GetScale() => Scale.Count > 0 ? Scale.Peek() : 1f;

	/// <summary>
	/// A semantic wrapper class for numeric types which need to be scaled by the current scale factor.
	/// </summary>
	/// <typeparam name="T">The numeric type of the value. Must implement IMultiplyOperators vs float.</typeparam>
	internal class Scaled<T> where T : IMultiplyOperators<T, float, T>
	{
		public T UnscaledValue { get; }
		public T ScaledValue { get; }
		public float Scale { get; }

		public Scaled(T unscaledValue)
		{
			float scale = GetScale();
			UnscaledValue = unscaledValue;
			Scale = scale;
			ScaledValue = unscaledValue * scale;
		}
	}

	// vector types need their own implementation because they don't implement IMultiplyOperators
	internal class ScaledVector2
	{
		public Vector2 UnscaledValue { get; }
		public Vector2 ScaledValue { get; }
		public float Scale { get; }

		public ScaledVector2(Vector2 unscaledValue)
		{
			float scale = GetScale();
			UnscaledValue = unscaledValue;
			Scale = scale;
			ScaledValue = unscaledValue * scale;
		}
	}
}
