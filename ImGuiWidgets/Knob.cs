namespace ktsu.io.ImGuiWidgets;

using System.Globalization;
using System.Numerics;
using ImGuiNET;

/// <summary>
/// Knob widget for ImGui.NET
/// </summary>
public static class Knob
{
	/// <summary>
	/// The diameter of the knob
	/// </summary>
	public const float Size = 36;

	/// <summary>
	/// Calculate the width a knob widget should consume based on the number of knobs in a row to consume the available space
	/// </summary>
	/// <param name="numKnobs">The number of knobs to fill the available space</param>
	/// <returns>The width each knob should consume</returns>
	public static float CalculateKnobWidth(int numKnobs)
	{
		var style = ImGui.GetStyle();
		float width = (ImGui.GetContentRegionMax().X - (style.ItemInnerSpacing.X * 2 * numKnobs)) / numKnobs;
		return width;
	}

	/// <summary>
	/// Draw a knob widget
	/// </summary>
	/// <param name="label">The label to display below the knob</param>
	/// <param name="value">The value to display and modify</param>
	/// <param name="minv">The minimum value</param>
	/// <param name="maxv">The maximum value</param>
	/// <param name="width">The width of the widget</param>
	/// <param name="valueLabels">Optional labels to display instead of the value</param>
	/// <returns>True if the knob was touched this frame</returns>
	public static bool Draw(string label, ref int value, float minv, float maxv, float width = 0, string[]? valueLabels = null)
	{
		float f = value;
		bool b = Draw(label, ref f, minv, maxv, width, valueLabels);
		value = (int)f;
		return b;
	}

	/// <summary>
	/// Draw a knob widget
	/// </summary>
	/// <param name="label">The label to display below the knob</param>
	/// <param name="value">The value to display and modify</param>
	/// <param name="minv">The minimum value</param>
	/// <param name="maxv">The maximum value</param>
	/// <param name="width">The width of the widget</param>
	/// <param name="valueLabels">Optional labels to display instead of the value</param>
	/// <returns>True if the knob was touched this frame</returns>
	public static bool Draw(string label, ref float value, float minv, float maxv, float width = 0, string[]? valueLabels = null)
	{
		if (Math.Abs(value) < 0.01)
		{
			value = 0;
		}

		var style = ImGui.GetStyle();
		float line_height = ImGui.GetTextLineHeight();

		var p = ImGui.GetCursorScreenPos();
		float sz = Size;
		float radio = sz * 0.5f;

		float xpos = width * 0.5f;

		var center = new Vector2(p.X + xpos, p.Y + radio + line_height + (style.ItemInnerSpacing.Y * 2));
		float val1 = (value - minv) / (maxv - minv);

		string format = "";
		string[] split = maxv.ToString(CultureInfo.InvariantCulture).Split('.');
		for (int i = 0; i < split[0].Length - 1; ++i)
		{
			format += "#";
		}
		format += "0.";
		if (split.Length == 2)
		{
			for (int i = 0; i < split[1].Length; ++i)
			{
				format += "0";
			}
		}
		while (format.Length < 4)
		{
			format += "0";
		}


		string textval = value.ToString(format, CultureInfo.InvariantCulture);
		float roundedValue = (float)Math.Round(value);
		if (valueLabels != null)
		{
			textval = roundedValue >= 0 && roundedValue < valueLabels.Length ? valueLabels[(int)roundedValue] : "Off";
		}

		float gamma = (float)Math.PI / 4.0f;
		float alpha = (((float)Math.PI - gamma) * val1 * 2.0f) + gamma;

		float x2 = (-(float)Math.Sin(alpha) * radio) + center.X;
		float y2 = ((float)Math.Cos(alpha) * radio) + center.Y;

		ImGui.InvisibleButton(label, new Vector2(xpos * 2, sz + (line_height * 2) + (style.ItemInnerSpacing.Y * 3)));

		bool is_active = ImGui.IsItemActive();
		bool is_hovered = ImGui.IsItemHovered();
		bool touched = false;

		if (is_active)
		{
			touched = true;
			var mp = ImGui.GetIO().MousePos;
			alpha = (float)Math.Atan2(mp.X - center.X, center.Y - mp.Y) + (float)Math.PI;
			alpha = MathF.Max(gamma, MathF.Min((2.0f * (float)Math.PI) - gamma, alpha));
			float knobValue = 0.5f * (alpha - gamma) / ((float)Math.PI - gamma);
			value = (knobValue * (maxv - minv)) + minv;
		}

		uint col32 = ImGui.GetColorU32(is_active ? ImGuiCol.FrameBgActive : is_hovered ? ImGuiCol.FrameBgHovered : ImGuiCol.FrameBg);
		uint col32line = ImGui.GetColorU32(ImGuiCol.SliderGrabActive);
		uint col32text = ImGui.GetColorU32(ImGuiCol.Text);
		var draw_list = ImGui.GetWindowDrawList();
		draw_list.AddCircleFilled(center, radio, col32, 16);
		draw_list.AddLine(center, new Vector2(x2, y2), col32line, 1);
		draw_list.AddText(new Vector2(center.X - (ImGui.CalcTextSize(label).X * 0.5f), p.Y + style.ItemInnerSpacing.Y), col32text, label);
		draw_list.AddText(new Vector2(center.X - (ImGui.CalcTextSize(textval).X * 0.5f), p.Y + sz + (style.ItemInnerSpacing.Y * 2) + line_height), col32text, textval);

		return touched;
	}
}
