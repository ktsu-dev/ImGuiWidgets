namespace ktsu.io.ImGuiWidgets;

using ImGuiNET;
using ktsu.io.ImGuiStyler;

public static class Text
{
	public static void Centered(string text)
	{
		float textWidth = ImGui.CalcTextSize(text).X;
		Alignment.Center(textWidth);
		ImGui.TextUnformatted(text);
	}

	public static void CenteredWithin(string text, float width) => CenteredWithin(text, width, false);

	public static void CenteredWithin(string text, float width, bool clip)
	{
		if (clip)
		{
			text = Clip(text, width);
		}

		float textWidth = ImGui.CalcTextSize(text).X;
		Alignment.CenterWithin(textWidth, width);
		ImGui.TextUnformatted(text);
	}

	public static string Clip(string text, float width)
	{
		float textWidth = ImGui.CalcTextSize(text).X;
		if (textWidth <= width)
		{
			return text;
		}

		string ellipsis = "...";
		float ellipsisWidth = ImGui.CalcTextSize(ellipsis).X;

		while (textWidth + ellipsisWidth > width && text.Length > 0)
		{
			text = text[..^1];
			textWidth = ImGui.CalcTextSize(text).X;
		}

		return text + ellipsis;
	}
}
