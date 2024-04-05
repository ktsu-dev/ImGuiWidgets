#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace ktsu.io.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;
using ktsu.io.ScopedAction;

public class Tree : ScopedAction
{
	private Vector2 CursorStart { get; init; }
	private Vector2 CursorEnd { get; set; }
	private float IndentWidth { get; init; }
	private float HalfIndentWidth => IndentWidth / 2f;
	private float FrameHeight { get; init; }
	private float HalfFrameHeight => FrameHeight / 2f;
	private float ItemSpacingY { get; init; }
	private float Left { get; init; }
	private float Top { get; init; }
	private const float LineThickness = 2f;
	private const float HalfLineThickness = LineThickness / 2f;

	public Tree() : base()
	{
		ImGui.Indent();
		CursorStart = ImGui.GetCursorScreenPos();
		IndentWidth = ImGui.GetStyle().IndentSpacing;
		ItemSpacingY = ImGui.GetStyle().ItemSpacing.Y;
		FrameHeight = ImGui.GetFrameHeight();
		Left = CursorStart.X - HalfIndentWidth;
		Top = CursorStart.Y - ItemSpacingY - HalfLineThickness;

		OnClose = () =>
		{
			ImGui.SameLine();
			float bottom = CursorEnd.Y + HalfFrameHeight + HalfLineThickness;
			var a = new Vector2(Left, Top);
			var b = new Vector2(Left, bottom);
			ImGui.GetWindowDrawList().AddLine(a, b, ImGui.GetColorU32(Color.Gray.Value), LineThickness);
			ImGui.NewLine();
			ImGui.Unindent();
		};
	}

	public TreeChild Child => new(this);

	public class TreeChild : ScopedAction
	{
		public TreeChild(Tree parent) : base(
			onOpen: () =>
			{
				var cursor = ImGui.GetCursorScreenPos();
				parent.CursorEnd = cursor;
				float right = cursor.X;
				float y = cursor.Y + parent.HalfFrameHeight;

				var a = new Vector2(parent.Left, y);
				var b = new Vector2(right, y);

				ImGui.GetWindowDrawList().AddLine(a, b, ImGui.GetColorU32(Color.Gray.Value), LineThickness);
			},
			onClose: null)
		{
		}
	}
}
