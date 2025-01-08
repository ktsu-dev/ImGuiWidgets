namespace ktsu.ImGuiWidgets;

using ImGuiNET;

/// <summary>
/// Provides custom ImGui widgets.
/// </summary>
public static partial class ImGuiWidgets
{
	/// <summary>
	/// Displays a color indicator checkbox widget.
	/// </summary>
	/// <param name="color">The color to be displayed in the checkbox when it is enabled.</param>
	/// <param name="enabled">A boolean indicating whether the checkbox is enabled.</param>
	public static void ColorIndicator(ImColor color, bool enabled) => ColorIndicatorImpl.Show(color, enabled);

	internal static class ColorIndicatorImpl
	{
		/// <summary>
		/// Pushes the specified color to the ImGui style stack for the checkbox.
		/// </summary>
		/// <param name="color">The color to be applied to the checkbox.</param>
		private static void PushCheckColor(ImColor color)
		{
			ImGui.PushStyleColor(ImGuiCol.FrameBg, color.Value);
			ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, color.Value);
			ImGui.PushStyleColor(ImGuiCol.FrameBgActive, color.Value);
			ImGui.PushStyleColor(ImGuiCol.CheckMark, color.Value);
		}

		/// <summary>
		/// Pops the color from the ImGui style stack for the checkbox.
		/// </summary>
		private static void PopCheckColor()
		{
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();
			ImGui.PopStyleColor();
		}

		/// <summary>
		/// Displays a color indicator checkbox widget.
		/// </summary>
		/// <param name="color">The color to be displayed in the checkbox when it is enabled.</param>
		/// <param name="enabled">A boolean indicating whether the checkbox is enabled.</param>
		public static void Show(ImColor color, bool enabled)
		{
			if (enabled)
			{
				PushCheckColor(color);
			}
			ImGui.Checkbox("##hidelabel", ref enabled);
			if (enabled)
			{
				PopCheckColor();
			}
		}
	}
}
