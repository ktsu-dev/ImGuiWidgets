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
	private int gridItemsToShow = InitialGridSize;

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

		float iconSizePx = ImGuiApp.EmsToPx(2.5f);

		ImGui.NewLine();

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
		ImGui.SliderInt("Grid items to show", ref gridItemsToShow, 1, GridStrings.Count);

		ImGui.SeparatorText("Grid (Column Major) Icon Alignment Horizontal");
		ImGuiWidgets.Grid(GridStrings.Take(gridItemsToShow), i => ImGuiWidgets.CalcIconSize(i, iconSizePx), (item, cellSize, itemSize) =>
		{
			ImGuiWidgets.Icon(item, ktsuTexture.TextureId, iconSizePx, Color.White.Value);
		}, ImGuiWidgets.GridOrder.ColumnMajor);

		ImGui.NewLine();
		ImGui.SeparatorText("Grid (Row Major) Icon Alignment Vertical");
		float bigIconSize = iconSizePx * 2;
		ImGuiWidgets.Grid(GridStrings.Take(gridItemsToShow), i => ImGuiWidgets.CalcIconSize(i, bigIconSize, ImGuiWidgets.IconAlignment.Vertical), (item, cellSize, itemSize) =>
		{
			using (new Alignment.CenterWithin(itemSize, cellSize))
			{
				ImGuiWidgets.Icon(item, ktsuTexture.TextureId, bigIconSize, Color.White.Value, ImGuiWidgets.IconAlignment.Vertical);
			}
		}, ImGuiWidgets.GridOrder.RowMajor);

		MessageOK.ShowIfOpen();
	}
}
