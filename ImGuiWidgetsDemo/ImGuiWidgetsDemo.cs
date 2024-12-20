namespace ktsu.ImGuiWidgetsDemo;

using System.Numerics;
using ImGuiNET;
using ktsu.ImGuiApp;
using ktsu.ImGuiStyler;
using ktsu.ImGuiPopups;
using ktsu.ImGuiWidgets;
using ktsu.StrongPaths;

internal class ImGuiWidgetsDemo
{
	private static void Main()
	{
		ImGuiWidgetsDemo imGuiWidgetsDemo = new();
		ImGuiApp.Start(nameof(ImGuiWidgetsDemo), new ImGuiAppWindowState(), imGuiWidgetsDemo.OnStart, imGuiWidgetsDemo.OnTick, imGuiWidgetsDemo.OnMenu, imGuiWidgetsDemo.OnWindowResized);
	}

	private static float value = 0.5f;

	private ImGuiWidgets.DividerContainer DividerContainer { get; } = new("DemoDividerContainer");
	private ImGuiPopups.MessageOK MessageOK { get; } = new();

	private List<string> GridStrings { get; } = [];
	private static int InitialGridSize { get; } = 32;
	private int GridItemsToShow { get; set; } = InitialGridSize;
	private ImGuiWidgets.GridOrder GridOrder { get; set; } = ImGuiWidgets.GridOrder.RowMajor;
	private ImGuiWidgets.GridRowAlignment GridRowAlignment { get; set; } = ImGuiWidgets.GridRowAlignment.Left;
	private ImGuiWidgets.IconAlignment GridIconAlignment { get; set; } = ImGuiWidgets.IconAlignment.Vertical;
	private bool GridIconSizeBig { get; set; } = true;
	private bool GridIconCenterWithinCell { get; set; } = true;

#pragma warning disable CA5394 //Do not use insecure randomness
	private void OnStart()
	{
		DividerContainer.Add(new("Left", 0.25f, ShowLeftPanel));
		DividerContainer.Add(new("Right", 0.75f, ShowRightPanel));

		for (int i = 0; i < InitialGridSize; i++)
		{
			string randomString = $"{i}:";
			int randomAmount = new Random().Next(2, 32);
			for (int j = 0; j < randomAmount; j++)
			{
				randomString += (char)new Random().Next(32, 127);
			}
			GridStrings.Add(randomString);
		}
	}
#pragma warning restore CA5394 //Do not use insecure randomness

	private void OnTick(float dt) => DividerContainer.Tick(dt);

	private void OnMenu()
	{
		// Method intentionally left empty.
	}

	private void OnWindowResized()
	{
		// Method intentionally left empty.
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
	private void ShowLeftPanel(float size)
	{
		ImGui.Text("Left Divider Zone");

		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Wiper) + "Test Pascal Case", ref value, 0, 1, 0, null, ImGuiKnobVariant.Wiper);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.WiperOnly), ref value, 0, 1, 0, null, ImGuiKnobVariant.WiperOnly);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.WiperDot), ref value, 0, 1, 0, null, ImGuiKnobVariant.WiperDot);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Tick), ref value, 0, 1, 0, null, ImGuiKnobVariant.Tick);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Stepped), ref value, 0, 1, 0, null, ImGuiKnobVariant.Stepped);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Space), ref value, 0, 1, 0, null, ImGuiKnobVariant.Space);
		ImGuiWidgets.Knob("Throttle Position", ref value, 0, 1, 0, null, ImGuiKnobVariant.Space);

		ImGuiWidgets.ColorIndicator(Color.Red, true);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Red, false);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Green, true);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Green, false);

		ImGui.Button("Hello, Tree!");
		using (var tree = new ImGuiWidgets.Tree())
		{
			for (int i = 0; i < 5; i++)
			{
				using (tree.Child)
				{
					ImGui.Button($"Hello, Child {i}!");
					using (var subtree = new ImGuiWidgets.Tree())
					{
						using (subtree.Child)
						{
							ImGui.Button($"Hello, Grandchild!");
						}
					}
				}
			}
		}
	}

	private void ShowRightPanel(float size)
	{
		ImGui.Text("Right Divider Zone");

		var ktsuIconPath = (AbsoluteDirectoryPath)Environment.CurrentDirectory / (FileName)"ktsu.png";
		var ktsuTexture = ImGuiApp.GetOrLoadTexture(ktsuIconPath);

		if (ImGuiWidgets.Image(ktsuTexture.TextureId, new(128, 128)))
		{
			MessageOK.Open("Click", "You chose the image");
		}

		float iconWidthEms = 7.5f;
		float tilePaddingEms = 0.5f;
		float iconWidthPx = ImGuiApp.EmsToPx(iconWidthEms);
		float tilePaddingPx = ImGuiApp.EmsToPx(tilePaddingEms);

		var iconSize = new Vector2(iconWidthPx, iconWidthPx);

		ImGuiWidgets.Icon("Click Me", ktsuTexture.TextureId, iconWidthPx, Color.White.Value, ImGuiWidgets.IconAlignment.Vertical, new ImGuiWidgets.IconDelegates()
		{
			OnClick = () => MessageOK.Open("Click", "You chose Tile1")
		});
		ImGui.SameLine();
		ImGuiWidgets.Icon("Double Click Me", ktsuTexture.TextureId, iconWidthPx, Color.White.Value, ImGuiWidgets.IconAlignment.Vertical, new ImGuiWidgets.IconDelegates()
		{
			OnDoubleClick = () => MessageOK.Open("Double Click", "Yippee!!!!!!!!")
		});
		ImGui.SameLine();
		ImGuiWidgets.Icon("Right Click Me", ktsuTexture.TextureId, iconWidthPx, Color.White.Value, ImGuiWidgets.IconAlignment.Vertical, new ImGuiWidgets.IconDelegates()
		{
			OnContextMenu = () =>
			{
				ImGui.MenuItem("Context Menu Item 1");
				ImGui.MenuItem("Context Menu Item 2");
				ImGui.MenuItem("Context Menu Item 3");
			},
		});

		ImGui.NewLine();

		if (ImGui.CollapsingHeader("Grid Settings"))
		{
			bool showGridDebug = ImGuiWidgets.EnableGridDebugDraw;
			if (ImGui.Checkbox("Show Grid Debug", ref showGridDebug))
			{
				ImGuiWidgets.EnableGridDebugDraw = showGridDebug;
			}
			bool showIconDebug = ImGuiWidgets.EnableIconDebugDraw;
			if (ImGui.Checkbox("Show Icon Debug", ref showIconDebug))
			{
				ImGuiWidgets.EnableIconDebugDraw = showIconDebug;
			}

			{
				bool gridIconCenterWithinCell = GridIconCenterWithinCell;
				bool gridIconSizeBig = GridIconSizeBig;
				int gridItemsToShow = GridItemsToShow;
				int gridOrder = (int)GridOrder;
				string[] gridOrderComboNames = Enum.GetNames<ImGuiWidgets.GridOrder>();
				int gridAlignment = (int)GridRowAlignment;
				string[] gridAlignmentComboNames = Enum.GetNames<ImGuiWidgets.GridRowAlignment>();
				int gridIconAlignment = (int)GridIconAlignment;
				string[] gridIconAlignmentComboNames = Enum.GetNames<ImGuiWidgets.IconAlignment>();

				if (ImGui.Checkbox("Use Big Grid Icons", ref gridIconSizeBig))
				{
					GridIconSizeBig = gridIconSizeBig;
				}
				if (ImGui.Checkbox("Center within cell", ref gridIconCenterWithinCell))
				{
					GridIconCenterWithinCell = gridIconCenterWithinCell;
				}
				if (ImGui.SliderInt("Items to show", ref gridItemsToShow, 1, GridStrings.Count))
				{
					GridItemsToShow = gridItemsToShow;
				}
				if (ImGui.Combo("Order", ref gridOrder, gridOrderComboNames, gridOrderComboNames.Length))
				{
					GridOrder = (ImGuiWidgets.GridOrder)gridOrder;
				}
				if (ImGui.Combo("Alignment", ref gridAlignment, gridAlignmentComboNames, gridAlignmentComboNames.Length))
				{
					GridRowAlignment = (ImGuiWidgets.GridRowAlignment)gridAlignment;
				}
				if (ImGui.Combo("Icon Alignment", ref gridIconAlignment, gridIconAlignmentComboNames, gridIconAlignmentComboNames.Length))
				{
					GridIconAlignment = (ImGuiWidgets.IconAlignment)gridIconAlignment;
				}
			}
		}

		float iconSizePx = ImGuiApp.EmsToPx(2.5f);
		float bigIconSizePx = iconSizePx * 2;
		float gridIconSize = GridIconSizeBig ? bigIconSizePx : iconSizePx;

		ImGui.Separator();

		ImGuiWidgets.Grid(GridStrings.Take(GridItemsToShow), i => ImGuiWidgets.CalcIconSize(i, gridIconSize, GridIconAlignment), (item, cellSize, itemSize) =>
		{
			// TODO: Make CenterWithin take an enabled arg to avoid this duplication 
			if (GridIconCenterWithinCell)
			{
				using (new Alignment.CenterWithin(itemSize, cellSize))
				{
					ImGuiWidgets.Icon(item, ktsuTexture.TextureId, gridIconSize, Color.White.Value, GridIconAlignment);
				}
			}
			else
			{
				ImGuiWidgets.Icon(item, ktsuTexture.TextureId, gridIconSize, Color.White.Value, GridIconAlignment);
			}
		}, GridOrder, GridRowAlignment);

		MessageOK.ShowIfOpen();
	}
}
