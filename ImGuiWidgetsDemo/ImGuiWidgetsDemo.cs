namespace ktsu.ImGuiWidgetsDemo;

using System.Numerics;
using ImGuiNET;
using ktsu.ImGuiApp;
using ktsu.ImGuiStyler;
using ktsu.ImGuiPopups;
using ktsu.ImGuiWidgets;
using ktsu.StrongPaths;
using System.Collections.ObjectModel;
using ktsu.StrongStrings;
using ktsu.Extensions;

internal sealed record class StrongStringExample : StrongStringAbstract<StrongStringExample> { }

/// <summary>
/// Demo enum values.
/// </summary>
public enum EnumValues
{
	/// <summary>
	/// First enum value.
	/// </summary>
	Value1,
	/// <summary>
	/// Second enum value.
	/// </summary>
	ValueB,
	/// <summary>
	/// Third enum value.
	/// </summary>
	ValueIII,
}

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
	private static int InitialGridItemCount { get; } = 32;
	private int GridItemsToShow { get; set; } = InitialGridItemCount;
	private float GridHeight { get; set; } = 500f;
	private ImGuiWidgets.GridOrder GridOrder { get; set; } = ImGuiWidgets.GridOrder.RowMajor;
	private ImGuiWidgets.IconAlignment GridIconAlignment { get; set; } = ImGuiWidgets.IconAlignment.Vertical;
	private bool GridIconSizeBig { get; set; } = true;
	private bool GridIconCenterWithinCell { get; set; } = true;
	private bool GridFitToContents { get; set; }
	private EnumValues selectedEnumValue = EnumValues.Value1;
	private string selectedStringValue = "Hello";
	private readonly Collection<string> possibleStringValues = ["Hello", "World", "Goodbye"];
	private StrongStringExample selectedStrongString = "Strong Hello".As<StrongStringExample>();
	private readonly Collection<StrongStringExample> possibleStrongStringValues = ["Strong Hello".As<StrongStringExample>(),
		 "Strong World".As<StrongStringExample>(), "Strong Goodbye".As<StrongStringExample>()];

#pragma warning disable CA5394 //Do not use insecure randomness
	private void OnStart()
	{
		DividerContainer.Add(new("Left", 0.25f, ShowLeftPanel));
		DividerContainer.Add(new("Right", 0.75f, ShowRightPanel));

		for (int i = 0; i < InitialGridItemCount; i++)
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

		ImGui.SeparatorText("Knobs");
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Wiper) + "Test Pascal Case", ref value, 0, 1, 0, null, ImGuiKnobVariant.Wiper);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.WiperOnly), ref value, 0, 1, 0, null, ImGuiKnobVariant.WiperOnly);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.WiperDot), ref value, 0, 1, 0, null, ImGuiKnobVariant.WiperDot);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Tick), ref value, 0, 1, 0, null, ImGuiKnobVariant.Tick);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Stepped), ref value, 0, 1, 0, null, ImGuiKnobVariant.Stepped);
		ImGuiWidgets.Knob(nameof(ImGuiKnobVariant.Space), ref value, 0, 1, 0, null, ImGuiKnobVariant.Space);
		ImGuiWidgets.Knob("Throttle Position", ref value, 0, 1, 0, null, ImGuiKnobVariant.Space);

		ImGui.SeparatorText("Color Indicators");
		ImGuiWidgets.ColorIndicator(Color.Red, true);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Red, false);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Green, true);
		ImGui.SameLine();
		ImGuiWidgets.ColorIndicator(Color.Green, false);

		ImGui.SeparatorText("Combos");
		ImGuiWidgets.Combo("Enum Combo", ref selectedEnumValue);
		ImGuiWidgets.Combo("String Combo", ref selectedStringValue, possibleStringValues);
		ImGuiWidgets.Combo("Strong String Combo", ref selectedStrongString, possibleStrongStringValues);

		using (new ScopedDisable(true))
		{
			ImGui.SeparatorText("Disabled");

			bool value = true;
			int currentItem = 0;
			string[] items = ["Item 1", "Item 2", "Item 3"];

			ImGui.Checkbox("Disabled Checkbox", ref value);
			ImGui.Combo("Disabled Combo", ref currentItem, items, items.Length);
		}

		ImGui.SeparatorText("Tree");
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
		var ktsuIconPath = (AbsoluteDirectoryPath)Environment.CurrentDirectory / (FileName)"ktsu.png";
		var ktsuTexture = ImGuiApp.GetOrLoadTexture(ktsuIconPath);

		ImGui.Text("Right Divider Zone");

		if (ImGuiWidgets.Image(ktsuTexture.TextureId, new(128, 128)))
		{
			MessageOK.Open("Click", "You chose the image");
		}

		float iconWidthEms = 7.5f;
		float tilePaddingEms = 0.5f;
		float iconWidthPx = ImGuiApp.EmsToPx(iconWidthEms);
		float tilePaddingPx = ImGuiApp.EmsToPx(tilePaddingEms);

		var iconSize = new Vector2(iconWidthPx, iconWidthPx);

		ImGuiWidgets.Icon("Click Me", ktsuTexture.TextureId, iconWidthPx, ImGuiWidgets.IconAlignment.Vertical,
			new()
			{
				OnClick = () => MessageOK.Open("Click", "You chose Tile1")
			});

		ImGui.SameLine();
		ImGuiWidgets.Icon("Double Click Me", ktsuTexture.TextureId, iconWidthPx, ImGuiWidgets.IconAlignment.Vertical,
			new()
			{
				OnDoubleClick = () => MessageOK.Open("Double Click", "Yippee!!!!!!!!")
			});
		ImGui.SameLine();
		ImGuiWidgets.Icon("Right Click Me", ktsuTexture.TextureId, iconWidthPx, ImGuiWidgets.IconAlignment.Vertical,
			new()
			{
				OnContextMenu = () =>
				{
					ImGui.MenuItem("Context Menu Item 1");
					ImGui.MenuItem("Context Menu Item 2");
					ImGui.MenuItem("Context Menu Item 3");
				},
			});
		ImGui.SameLine();
		ImGuiWidgets.Icon("Hover Me", ktsuTexture.TextureId, iconWidthPx, ImGuiWidgets.IconAlignment.Vertical,
			new()
			{
				Tooltip = "You hovered over me"
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
				bool gridFitToContents = GridFitToContents;
				int gridItemsToShow = GridItemsToShow;
				var gridOrder = GridOrder;
				var gridIconAlignment = GridIconAlignment;
				float gridHeight = GridHeight;

				if (ImGui.Checkbox("Use Big Grid Icons", ref gridIconSizeBig))
				{
					GridIconSizeBig = gridIconSizeBig;
				}

				if (ImGui.Checkbox("Center within cell", ref gridIconCenterWithinCell))
				{
					GridIconCenterWithinCell = gridIconCenterWithinCell;
				}

				if (ImGui.Checkbox("Fit to contents", ref gridFitToContents))
				{
					GridFitToContents = gridFitToContents;
				}

				if (ImGui.SliderInt("Items to show", ref gridItemsToShow, 1, GridStrings.Count))
				{
					GridItemsToShow = gridItemsToShow;
				}

				if (ImGuiWidgets.Combo("Order", ref gridOrder))
				{
					GridOrder = gridOrder;
				}

				if (ImGuiWidgets.Combo("Icon Alignment", ref gridIconAlignment))
				{
					GridIconAlignment = gridIconAlignment;
				}

				if (ImGui.SliderFloat("Grid Height", ref gridHeight, 1f, 1000f))
				{
					GridHeight = gridHeight;
				}
			}
		}

		float iconSizePx = ImGuiApp.EmsToPx(2.5f);
		float bigIconSizePx = iconSizePx * 2;
		float gridIconSize = GridIconSizeBig ? bigIconSizePx : iconSizePx;
		var gridSize = new Vector2(ImGui.GetContentRegionAvail().X, GridHeight);

		ImGui.Separator();

		Vector2 MeasureGridSize(string item) => ImGuiWidgets.CalcIconSize(item, gridIconSize, GridIconAlignment);
		void DrawGridCell(string item, Vector2 cellSize, Vector2 itemSize)
		{
			if (GridIconCenterWithinCell)
			{
				using (new Alignment.CenterWithin(itemSize, cellSize))
				{
					ImGuiWidgets.Icon(item, ktsuTexture.TextureId, gridIconSize, GridIconAlignment);
				}
			}
			else
			{
				ImGuiWidgets.Icon(item, ktsuTexture.TextureId, gridIconSize, GridIconAlignment);
			}
		}

		ImGuiWidgets.GridOptions gridOptions = new()
		{
			GridSize = new(ImGui.GetContentRegionAvail().X, GridHeight),
			FitToContents = GridFitToContents,
		};
		switch (GridOrder)
		{
			case ImGuiWidgets.GridOrder.RowMajor:
				ImGuiWidgets.RowMajorGrid("demoRowMajorGrid", GridStrings.Take(GridItemsToShow), MeasureGridSize, DrawGridCell, gridOptions);
				break;

			case ImGuiWidgets.GridOrder.ColumnMajor:
				ImGuiWidgets.ColumnMajorGrid("demoColumnMajorGrid", GridStrings.Take(GridItemsToShow), MeasureGridSize, DrawGridCell, gridOptions);
				break;

			default:
				throw new NotImplementedException();
		}

		ImGui.Separator();

		MessageOK.ShowIfOpen();
	}
}
