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

	private void OnStart()
	{
		DividerContainer.Add(new("Left", 0.25f, ShowLeftPanel));
		DividerContainer.Add(new("Right", 0.75f, ShowRightPanel));


		for (int i = 0; i < 32; i++)
		{
			string randomString = $"{i}:";
			int randomAmount = new Random().Next(12, 32);
			for (int j = 0; j < randomAmount; j++)
			{
				randomString += (char)new Random().Next(32, 127);
			}
			GridStrings.Add(randomString);
		}
	}

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

		float tileWidthEms = 10;
		float iconWidthEms = 7.5f;
		float tilePaddingEms = 0.5f;
		float tileWidthPx = ImGuiApp.EmsToPx(tileWidthEms);
		float iconWidthPx = ImGuiApp.EmsToPx(iconWidthEms);
		float tilePaddingPx = ImGuiApp.EmsToPx(tilePaddingEms);

		var iconSize = new Vector2(iconWidthPx, iconWidthPx);


		if (ImGuiWidgets.Tile("Tile1", tileWidthPx, tilePaddingPx, () =>
		{
			ImGuiWidgets.ImageCenteredWithin(ktsuTexture.TextureId, iconSize, tileWidthPx);
			ImGuiWidgets.TextCenteredWithin("Click me", tileWidthPx, clip: true);
		}))
		{
			MessageOK.Open("Click", "You chose Tile1");
		}

		ImGui.SameLine(0, 0);
		_ = ImGuiWidgets.Tile("Tile2", tileWidthPx, tilePaddingPx, () =>
		{
			ImGuiWidgets.ImageCenteredWithin(ktsuTexture.TextureId, iconSize, tileWidthPx);
			ImGuiWidgets.TextCenteredWithin("Double Click Me", tileWidthPx, clip: true);
		},
		new()
		{
			OnDoubleClick = () =>
			{
				MessageOK.Open("Double Click", $"Yippee!");
			},
		});

		ImGui.SameLine(0, 0);
		_ = ImGuiWidgets.Tile("Tile3", tileWidthPx, tilePaddingPx, () =>
		{
			ImGuiWidgets.ImageCenteredWithin(ktsuTexture.TextureId, iconSize, tileWidthPx);
			ImGuiWidgets.TextCenteredWithin("Right Click Me", tileWidthPx, clip: true);
		},
		new()
		{
			OnContextMenu = () =>
			{
				ImGui.MenuItem("Context Menu Item 1");
				ImGui.MenuItem("Context Menu Item 2");
				ImGui.MenuItem("Context Menu Item 3");
			},
		});

		float iconSizePx = ImGuiApp.EmsToPx(2.5f);

		ImGuiWidgets.Grid(GridStrings, i => ImGuiWidgets.CalcIconSize(i, iconSizePx), (item, cellSize, itemSize) =>
		{
			ImGuiWidgets.Icon(item, ktsuTexture.TextureId, iconSizePx, Color.White.Value);
		});

		float bigIconSize = iconSizePx * 2;
		string label = "IconIconIcon";

		ImGuiWidgets.Grid(GridStrings, i => ImGuiWidgets.CalcIconSize(label, bigIconSize, ImGuiWidgets.IconAlignment.Vertical), (item, cellSize, itemSize) =>
		{
			Alignment.CenterWithin(itemSize.X, cellSize.X);
			ImGuiWidgets.Icon(label, ktsuTexture.TextureId, bigIconSize, Color.White.Value, ImGuiWidgets.IconAlignment.Vertical);
		});

		MessageOK.ShowIfOpen();
	}
}
