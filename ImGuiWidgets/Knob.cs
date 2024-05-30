namespace ktsu.io.ImGuiWidgets;
using System;
using System.Globalization;
using System.Numerics;
using ImGuiNET;


#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

[Flags]
public enum ImGuiKnobFlags
{
	None = 0,
	NoTitle = 1 << 0,
	NoInput = 1 << 1,
	ValueTooltip = 1 << 2,
	DragHorizontal = 1 << 3,
	TitleBelow = 1 << 4,
};

[Flags]
public enum ImGuiKnobVariant
{
	Tick = 1 << 0,
	Dot = 1 << 1,
	Wiper = 1 << 2,
	WiperOnly = 1 << 3,
	WiperDot = 1 << 4,
	Stepped = 1 << 5,
	Space = 1 << 6,
};

/// <summary>
/// Knob widget for ImGui.NET
/// </summary>
public static class Knob
{
	public class KnobColors
	{
		public ImColor Base { get; set; }
		public ImColor Hovered { get; set; }
		public ImColor Active { get; set; }

		public KnobColors() { }

		public KnobColors(ImColor color)
		{
			Base = color;
			Hovered = color;
			Active = color;
		}
	}

	public static bool Draw(string label, ref float value, float vMin, float vMax, float speed = 0, string? format = null, ImGuiKnobVariant variant = ImGuiKnobVariant.Tick, float size = 0, ImGuiKnobFlags flags = ImGuiKnobFlags.None, int steps = 10)
	{
		format ??= "%.3f";
		return KnobInternal<float>.BaseKnob(label, ImGuiDataType.Float, ref value, vMin, vMax, speed, format, variant, size, flags, steps);
	}

	public static bool Draw(string label, ref int value, int vMin, int vMax, float speed = 0, string? format = null, ImGuiKnobVariant variant = ImGuiKnobVariant.Tick, float size = 0, ImGuiKnobFlags flags = ImGuiKnobFlags.None, int steps = 10)
	{
		format ??= "%if";
		return KnobInternal<int>.BaseKnob(label, ImGuiDataType.S32, ref value, vMin, vMax, speed, format, variant, size, flags, steps);
	}

	private static void DrawArc1(Vector2 center, float radius, float startAngle, float endAngle, float thickness, ImColor color, int numSegments)
	{
		var start = new Vector2(
					center[0] + (MathF.Cos(startAngle) * radius),
					center[1] + (MathF.Sin(startAngle) * radius));

		var end = new Vector2(
					center[0] + (MathF.Cos(endAngle) * radius),
					center[1] + (MathF.Sin(endAngle) * radius));

		// Calculate bezier arc points
		float ax = start[0] - center[0];
		float ay = start[1] - center[1];
		float bx = end[0] - center[0];
		float by = end[1] - center[1];
		float q1 = (ax * ax) + (ay * ay);
		float q2 = q1 + (ax * bx) + (ay * by);
		float k2 = 4.0f / 3.0f * (MathF.Sqrt(2.0f * q1 * q2) - q2) / ((ax * by) - (ay * bx));
		var arc1 = new Vector2(center[0] + ax - (k2 * ay), center[1] + ay + (k2 * ax));
		var arc2 = new Vector2(center[0] + bx + (k2 * by), center[1] + by - (k2 * bx));

		var drawlist = ImGui.GetWindowDrawList();

		drawlist.AddBezierCubic(start, arc1, arc2, end, ImGui.GetColorU32(color.Value), thickness, numSegments);
	}

	public static void DrawArc(Vector2 center, float radius, float startAngle, float endAngle, float thickness, ImColor color, int numSegments, int bezierCount)
	{
		// Overlap and angle of ends of bezier curves needs work, only looks good when not transperant
		float overlap = thickness * radius * 0.00001f * MathF.PI;
		float delta = endAngle - startAngle;
		float bez_step = 1.0f / bezierCount;
		float mid_angle = startAngle + overlap;

		for (int i = 0; i < bezierCount - 1; i++)
		{
			float mid_angle2 = (delta * bez_step) + mid_angle;
			DrawArc1(center, radius, mid_angle - overlap, mid_angle2 + overlap, thickness, color, numSegments);
			mid_angle = mid_angle2;
		}

		DrawArc1(center, radius, mid_angle - overlap, endAngle, thickness, color, numSegments);
	}

	private class KnobInternal<TDataType> where TDataType : unmanaged, INumber<TDataType>
	{
		public float Radius { get; set; }
		public bool ValueChanged { get; set; }
		public Vector2 Center { get; set; }
		public bool IsActive { get; set; }
		public bool IsHovered { get; set; }
		public float AngleMin { get; set; }
		public float AngleMax { get; set; }
		public float T { get; set; }
		public float Angle { get; set; }
		public float AngleCos { get; set; }
		public float AngleSin { get; set; }

		private static float AccumulatedDiff { get; set; }
		private static bool AccumulatorDirty { get; set; }

		private static float InverseLerp(TDataType min, TDataType max, TDataType value) => float.CreateSaturating(value - min) / float.CreateSaturating(max - min);

		public KnobInternal(string label_, ImGuiDataType dataType, ref TDataType value, TDataType vMin, TDataType vMax, float speed, float radius_, string format, ImGuiKnobFlags flags)
		{
			Radius = radius_;
			T = InverseLerp(vMin, vMax, value);
			var screenPos = ImGui.GetCursorScreenPos();

			// Handle dragging
			ImGui.InvisibleButton(label_, new(Radius * 2.0f, Radius * 2.0f));

			ValueChanged = DragBehavior(dataType, ref value, vMin, vMax, speed, format, flags);

			AngleMin = MathF.PI * 0.75f;
			AngleMax = MathF.PI * 2.25f;
			Center = new(screenPos[0] + Radius, screenPos[1] + Radius);
			Angle = AngleMin + ((AngleMax - AngleMin) * T);
			AngleCos = MathF.Cos(Angle);
			AngleSin = MathF.Sin(Angle);
		}

		private bool DragBehavior(ImGuiDataType dataType, ref TDataType v, TDataType vMin, TDataType vMax, float speed, string format, ImGuiKnobFlags flags)
		{
			float floatValue = float.CreateSaturating(v);
			float floatMin = float.CreateSaturating(vMin);
			float floatMax = float.CreateSaturating(vMax);
			bool isClamped = vMin < vMax;
			float range = floatMax - floatMin;
			if (speed == 0.0f && isClamped && (range < float.MaxValue))
			{
				speed = range * 0.01f;
			}

			bool justActivated = ImGui.IsItemActivated();
			IsActive = ImGui.IsItemActive();
			IsHovered = ImGui.IsItemHovered();

			bool isFloatingPoint = dataType is ImGuiDataType.Float or ImGuiDataType.Double;
			int decimalPrecision = isFloatingPoint ? ParseFormatPrecision(format, 3) : 0;
			speed = MathF.Max(speed, GetMinimumStepAtDecimalPrecision(decimalPrecision));

			var mouseDelta = ImGui.GetIO().MouseDelta;
			float diff = (flags.HasFlag(ImGuiKnobFlags.DragHorizontal) ? mouseDelta.X : -mouseDelta.Y) * speed;

			diff = IsActive ? diff : 0.0f;

			if (justActivated)
			{
				AccumulatedDiff = 0.0f;
				AccumulatorDirty = false;
			}
			else if (diff != 0.0f)
			{
				AccumulatedDiff += diff;
				AccumulatorDirty = true;
			}

			if (!AccumulatorDirty)
			{
				return false;
			}

			float newValue = floatValue + diff;

			// Round to user desired precision based on format string
			if (isFloatingPoint)
			{
				newValue = MathF.Round(newValue, decimalPrecision);
			}

			float appliedDiff = newValue - floatValue;
			AccumulatedDiff -= appliedDiff;
			AccumulatorDirty = false;

			if (newValue == -0.0f)
			{
				newValue = 0.0f;
			}

			// Clamp values (+ handle overflow/wrap-around for integer types)
			if (newValue != floatValue && isClamped)
			{
				if (newValue < floatMin || (newValue > floatValue && diff < 0.0f && !isFloatingPoint))
				{
					newValue = floatMin;
				}

				if (newValue > floatMax || (newValue < floatValue && diff > 0.0f && !isFloatingPoint))
				{
					newValue = floatMax;
				}
			}

			if (newValue != floatValue)
			{
				v = TDataType.CreateSaturating(newValue);
				return true;
			}

			return false;
		}

		private static int ParseFormatPrecision(string fmt, int defaultPrecision)
		{

			var fmtSpan = ParseFormatFindStart(fmt);
			if (fmtSpan[0] != '%')
			{
				return defaultPrecision;
			}

			fmtSpan = fmtSpan[1..];
			while (fmtSpan[0] is >= '0' and <= '9')
			{
				fmtSpan = fmtSpan[1..];
			}

			int precision = int.MaxValue;
			if (fmtSpan[0] == '.')
			{
				fmtSpan = fmtSpan[1..];
				int precisionLength = 0;
				while (fmtSpan[precisionLength] is >= '0' and <= '9')
				{
					precisionLength++;
				}
				precision = int.Parse(fmtSpan[..precisionLength], CultureInfo.CurrentCulture);
				fmtSpan = fmtSpan[precisionLength..];

				if (precision is < 0 or > 99)
				{
					precision = defaultPrecision;
				}
			}
			if (fmtSpan[0] is 'e' or 'E') // Maximum precision with scientific notation
			{
				precision = -1;
			}

			if ((fmtSpan[0] == 'g' || fmtSpan[0] == 'G') && precision == int.MaxValue)
			{
				precision = -1;
			}

			return (precision == int.MaxValue) ? defaultPrecision : precision;
		}

		private static ReadOnlySpan<char> ParseFormatFindStart(string fmt)
		{
			var fmtSpan = fmt.AsSpan();
			while (fmtSpan.Length > 2)
			{
				char c = fmtSpan[0];
				if (c == '%' && fmtSpan[1] != '%')
				{
					return fmtSpan;
				}
				else if (c == '%')
				{
					fmtSpan = fmtSpan[1..];
				}

				fmtSpan = fmtSpan[1..];
			}

			return fmtSpan;
		}

		private static readonly List<float> MinSteps = new() { 1.0f, 0.1f, 0.01f, 0.001f, 0.0001f, 0.00001f, 0.000001f, 0.0000001f, 0.00000001f, 0.000000001f };
		private static float GetMinimumStepAtDecimalPrecision(int decimal_precision)
		{
			return decimal_precision < 0
				? float.MinValue
				: (decimal_precision < MinSteps.Count) ? MinSteps[decimal_precision] : MathF.Pow(10.0f, -decimal_precision);
		}

		private void DrawDot(float size, float radius, float angle, KnobColors color, int segments)
		{
			float dotSize = size * Radius;
			float dotRadius = radius * Radius;

			ImGui.GetWindowDrawList().AddCircleFilled(
						new(Center[0] + (MathF.Cos(angle) * dotRadius), Center[1] + (MathF.Sin(angle) * dotRadius)),
						dotSize,
						ImGui.GetColorU32((IsActive ? color.Active : (IsHovered ? color.Hovered : color.Base)).Value),
						segments);
		}

		private void DrawTick(float start, float end, float width, float angle, KnobColors color)
		{
			float tickStart = start * Radius;
			float tickEnd = end * Radius;
			float angleCos = MathF.Cos(angle);
			float angleSin = MathF.Sin(angle);

			ImGui.GetWindowDrawList().AddLine(

				new(Center[0] + (angleCos * tickEnd), Center[1] + (angleSin * tickEnd)),
				new(Center[0] + (angleCos * tickStart), Center[1] + (angleSin * tickStart)),
				ImGui.GetColorU32((IsActive ? color.Active : (IsHovered ? color.Hovered : color.Base)).Value),
				width * Radius);
		}

		private void DrawCircle(float size, KnobColors color, int segments)
		{
			float circleRadius = size * Radius;

			ImGui.GetWindowDrawList().AddCircleFilled(
					Center,
					circleRadius,
					ImGui.GetColorU32((IsActive ? color.Active : (IsHovered ? color.Hovered : color.Base)).Value),
					segments);
		}

		private void DrawArc(float radius, float size, float startAngle, float endAngle, KnobColors color, int segments, int bezierCount)
		{
			float trackRadius = radius * Radius;
			float trackSize = (size * Radius * 0.5f) + 0.0001f;

			Knob.DrawArc(
					Center,
					trackRadius,
					startAngle,
					endAngle,
					trackSize,
					IsActive ? color.Active : (IsHovered ? color.Hovered : color.Base),
					segments,
					bezierCount);
		}

		private static List<string> WrapStringToWidth(string text, float width)
		{
			var lines = new List<string>();
			string line;
			var textSpan = text.AsSpan();
			float textWidth = ImGui.CalcTextSize(text).X;

			if (textWidth <= width)
			{
				lines.Add(text);
				return lines;
			}

			while (textSpan.Length > 0)
			{
				while (textSpan.StartsWith(" "))
				{
					textSpan = textSpan[1..];
				}

				while (textSpan.EndsWith(" "))
				{
					textSpan = textSpan[..(textSpan.Length - 1)];
				}

				var lineSpan = textSpan;

				float lineSize = ImGui.CalcTextSize(lineSpan).X;

				while (lineSize > width)
				{
					int lastSpace = lineSpan.LastIndexOf(' ');
					if (lastSpace == -1)
					{
						break;
					}

					lineSpan = lineSpan[..lastSpace];
					while (lineSpan.StartsWith(" "))
					{
						lineSpan = lineSpan[1..];
					}

					while (lineSpan.EndsWith(" "))
					{
						lineSpan = lineSpan[..(lineSpan.Length - 1)];
					}
					lineSize = ImGui.CalcTextSize(lineSpan).X;
				}

				line = lineSpan.ToString();
				lines.Add(line);
				textSpan = textSpan[line.Length..];
			}

			return lines;
		}

		public static KnobInternal<TDataType> KnobWithDrag(string label, ImGuiDataType dataType, ref TDataType value, TDataType vMin, TDataType vMax, float speed, string format, float size, ImGuiKnobFlags flags)
		{
			speed = speed == 0 ? float.CreateSaturating(vMax - vMin) / 250.0f : speed;
			ImGui.PushID(label);
			float width = size == 0 ? ImGui.GetTextLineHeight() * 4.0f : size * ImGui.GetIO().FontGlobalScale;

			var titleLines = WrapStringToWidth(label, width);

			float maxTitleLineWidth = 0.0f;

			if (!flags.HasFlag(ImGuiKnobFlags.NoTitle))
			{
				maxTitleLineWidth = titleLines.Max(line => ImGui.CalcTextSize(line).X);
			}

			maxTitleLineWidth = Math.Max(maxTitleLineWidth, width);
			float knobPadding = (maxTitleLineWidth - width) * 0.5f;

			ImGui.PushItemWidth(width);

			ImGui.BeginGroup();

			// There's an issue with `SameLine` and Groups, see https://github.com/ocornut/imgui/issues/4190.
			// This is probably not the best solution, but seems to work for now
			//ImGui.GetCurrentWindow().DC.CurrLineTextBaseOffset = 0;

			if (!flags.HasFlag(ImGuiKnobFlags.TitleBelow))
			{
				DrawTitle(flags, maxTitleLineWidth, titleLines);
			}

			// Draw knob
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + knobPadding);
			var k = new KnobInternal<TDataType>(label, dataType, ref value, vMin, vMax, speed, width * 0.5f, format, flags);

			// Draw tooltip
			if (flags.HasFlag(ImGuiKnobFlags.ValueTooltip) && (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) || ImGui.IsItemActive()))
			{
				ImGui.BeginTooltip();
				ImGui.Text(string.Format(CultureInfo.CurrentCulture, format, value));
				ImGui.EndTooltip();
			}

			// Draw input
			if (!flags.HasFlag(ImGuiKnobFlags.NoInput))
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + knobPadding);
				unsafe
				{
					fixed (TDataType* pValue = &value)
					{
						var pMin = &vMin;
						var pMax = &vMax;
						k.ValueChanged = ImGui.DragScalar("###knob_drag", dataType, (nint)pValue, speed, (nint)pMin, (nint)pMax, format);
					}
				}
			}

			if (flags.HasFlag(ImGuiKnobFlags.TitleBelow))
			{
				DrawTitle(flags, maxTitleLineWidth, titleLines);
			}

			ImGui.EndGroup();
			ImGui.PopItemWidth();
			ImGui.PopID();

			return k;

			static void DrawTitle(ImGuiKnobFlags flags, float width, List<string> titleLines)
			{
				if (!flags.HasFlag(ImGuiKnobFlags.NoTitle))
				{
					foreach (string line in titleLines)
					{
						var lineWidth = ImGui.CalcTextSize(line, false, width);
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ((width - lineWidth[0]) * 0.5f));
						ImGui.TextUnformatted(line);
					}
				}
			}
		}

		public static bool BaseKnob(string label, ImGuiDataType dataType, ref TDataType value, TDataType vMin, TDataType vMax, float speed, string format, ImGuiKnobVariant variant, float size, ImGuiKnobFlags flags, int steps = 10)
		{

			var knob = KnobWithDrag(label, dataType, ref value, vMin, vMax, speed, format, size, flags);

			switch (variant)
			{
				case ImGuiKnobVariant.Tick:
				{
					knob.DrawCircle(0.85f, GetSecondaryColorSet(), 32);
					knob.DrawTick(0.5f, 0.85f, 0.08f, knob.Angle, GetPrimaryColorSet());
					break;
				}
				case ImGuiKnobVariant.Dot:
				{
					knob.DrawCircle(0.85f, GetSecondaryColorSet(), 32);
					knob.DrawDot(0.12f, 0.6f, knob.Angle, GetPrimaryColorSet(), 12);
					break;
				}

				case ImGuiKnobVariant.Wiper:
				{
					knob.DrawCircle(0.7f, GetSecondaryColorSet(), 32);
					knob.DrawArc(0.8f, 0.41f, knob.AngleMin, knob.AngleMax, GetTrackColorSet(), 16, 2);

					if (knob.T > 0.01f)
					{
						knob.DrawArc(0.8f, 0.43f, knob.AngleMin, knob.Angle, GetPrimaryColorSet(), 16, 2);
					}
					break;
				}
				case ImGuiKnobVariant.WiperOnly:
				{
					knob.DrawArc(0.8f, 0.41f, knob.AngleMin, knob.AngleMax, GetTrackColorSet(), 32, 2);

					if (knob.T > 0.01)
					{
						knob.DrawArc(0.8f, 0.43f, knob.AngleMin, knob.Angle, GetPrimaryColorSet(), 16, 2);
					}
					break;
				}
				case ImGuiKnobVariant.WiperDot:
				{
					knob.DrawCircle(0.6f, GetSecondaryColorSet(), 32);
					knob.DrawArc(0.85f, 0.41f, knob.AngleMin, knob.AngleMax, GetTrackColorSet(), 16, 2);
					knob.DrawDot(0.1f, 0.85f, knob.Angle, GetPrimaryColorSet(), 12);
					break;
				}
				case ImGuiKnobVariant.Stepped:
				{
					for (float n = 0.0f; n < steps; n++)
					{
						float a = n / (steps - 1);
						float angle = knob.AngleMin + ((knob.AngleMax - knob.AngleMin) * a);
						knob.DrawTick(0.7f, 0.9f, 0.04f, angle, GetPrimaryColorSet());
					}

					knob.DrawCircle(0.6f, GetSecondaryColorSet(), 32);
					knob.DrawDot(0.12f, 0.4f, knob.Angle, GetPrimaryColorSet(), 12);
					break;
				}
				case ImGuiKnobVariant.Space:
				{
					knob.DrawCircle(0.3f - (knob.T * 0.1f), GetSecondaryColorSet(), 16);

					if (knob.T > 0.01f)
					{
						knob.DrawArc(0.4f, 0.15f, knob.AngleMin - 1.0f, knob.Angle - 1.0f, GetPrimaryColorSet(), 16, 2);
						knob.DrawArc(0.6f, 0.15f, knob.AngleMin + 1.0f, knob.Angle + 1.0f, GetPrimaryColorSet(), 16, 2);
						knob.DrawArc(0.8f, 0.15f, knob.AngleMin + 3.0f, knob.Angle + 3.0f, GetPrimaryColorSet(), 16, 2);
					}
					break;
				}

				default:
					break;
			}

			return knob.ValueChanged;
		}
	}

	private static KnobColors GetPrimaryColorSet()
	{
		var colors = ImGui.GetStyle().Colors;

		return new()
		{
			Active = new ImColor() { Value = colors[(int)ImGuiCol.ButtonActive] },
			Hovered = new ImColor() { Value = colors[(int)ImGuiCol.ButtonHovered] },
			Base = new ImColor() { Value = colors[(int)ImGuiCol.ButtonHovered] },
		};
	}

	private static KnobColors GetSecondaryColorSet()
	{
		var colors = ImGui.GetStyle().Colors;
		var activeColor = colors[(int)ImGuiCol.ButtonActive];
		var hoveredColor = colors[(int)ImGuiCol.ButtonHovered];

		var active = new Vector4(
				activeColor.X * 0.5f,
				activeColor.Y * 0.5f,
				activeColor.Z * 0.5f,
				activeColor.W);

		var hovered = new Vector4(
				hoveredColor.X * 0.5f,
				hoveredColor.Y * 0.5f,
				hoveredColor.Z * 0.5f,
				hoveredColor.W);

		return new()
		{
			Active = new ImColor() { Value = active },
			Hovered = new ImColor() { Value = hovered },
			Base = new ImColor() { Value = hovered },
		};
	}

	private static KnobColors GetTrackColorSet()
	{
		var colors = ImGui.GetStyle().Colors;
		var color = colors[(int)ImGuiCol.FrameBg];
		return new()
		{
			Active = new ImColor() { Value = color },
			Hovered = new ImColor() { Value = color },
			Base = new ImColor() { Value = color },
		};
	}
}
