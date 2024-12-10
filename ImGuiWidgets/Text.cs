namespace ktsu.ImGuiWidgets;

using ImGuiNET;
using ktsu.ImGuiStyler;

public static partial class ImGuiWidgets
{
	public static void Text(string text) => TextImpl.Show(text);
	public static void TextCentered(string text) => TextImpl.Centered(text);
	public static void TextCenteredWithin(string text, float width) => TextImpl.CenteredWithin(text, width);
	public static void TextCenteredWithin(string text, float width, bool clip) => TextImpl.CenteredWithin(text, width, clip);

	internal static class TextImpl
	{
		public static void Show(string text) => ImGui.TextUnformatted(text);

		public static void Centered(string text)
		{
			var textSize = ImGui.CalcTextSize(text);
			using (new Alignment.Center(textSize))
			{
				ImGui.TextUnformatted(text);
			}
		}

		public static void CenteredWithin(string text, float width) => CenteredWithin(text, width, false);

		public static void CenteredWithin(string text, float width, bool clip)
		{
			if (clip)
			{
				text = Clip(text, width);
			}

			var textSize = ImGui.CalcTextSize(text);
			using (new Alignment.Center(textSize))
			{
				ImGui.TextUnformatted(text);
			}
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
}
