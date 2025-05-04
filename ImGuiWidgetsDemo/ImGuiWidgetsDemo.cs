// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.ImGuiWidgetsDemo;

using System.Collections.ObjectModel;
using System.Numerics;
using ImGuiNET;
using ktsu.Extensions;
using ktsu.ImGuiApp;
using ktsu.ImGuiPopups;
using ktsu.ImGuiStyler;
using ktsu.ImGuiWidgets;
using ktsu.StrongPaths;
using ktsu.StrongStrings;
using ktsu.TextFilter;

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
	private float tab2Value = 0.5f;

	private ImGuiWidgets.DividerContainer DividerContainer { get; } = new("DemoDividerContainer");
	private ImGuiPopups.MessageOK MessageOK { get; } = new();
	private ImGuiWidgets.TabPanel DemoTabPanel { get; } = new("DemoTabPanel", true, true);
	private Dictionary<string, string> TabIds { get; } = [];
	private int NextDynamicTabId { get; set; } = 1;

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

	// Static fields for SearchBox filter persistence
	private static string BasicSearchTerm = string.Empty;
	private static TextFilterType BasicFilterType = TextFilterType.Glob;
	private static TextFilterMatchOptions BasicMatchOptions = TextFilterMatchOptions.ByWholeString;

	private static string FilteredSearchTerm = string.Empty;
	private static TextFilterType FilteredFilterType = TextFilterType.Glob;
	private static TextFilterMatchOptions FilteredMatchOptions = TextFilterMatchOptions.ByWholeString;

	private static string RankedSearchTerm = string.Empty;

	private static string GlobSearchTerm = string.Empty;
	private static TextFilterType GlobFilterType = TextFilterType.Glob;
	private static TextFilterMatchOptions GlobMatchOptions = TextFilterMatchOptions.ByWholeString;

	private static string RegexSearchTerm = string.Empty;
	private static TextFilterType RegexFilterType = TextFilterType.Regex;
	private static TextFilterMatchOptions RegexMatchOptions = TextFilterMatchOptions.ByWholeString;

#pragma warning disable CA5394 //Do not use insecure randomness
	private void OnStart()
	{
		DividerContainer.Add(new("Left", 0.25f, ShowLeftPanel));
		DividerContainer.Add(new("Right", 0.75f, ShowRightPanel));

		// Initialize TabPanel demo
		TabIds["tab1"] = DemoTabPanel.AddTab("tab1", "Tab 1", ShowTab1Content);
		TabIds["tab2"] = DemoTabPanel.AddTab("tab2", "Tab 2", ShowTab2Content);
		TabIds["tab3"] = DemoTabPanel.AddTab("tab3", "Tab 3", ShowTab3Content);

		for (var i = 0; i < InitialGridItemCount; i++)
		{
			var randomString = $"{i}:";
			var randomAmount = new Random().Next(2, 32);
			for (var j = 0; j < randomAmount; j++)
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
		ImGui.TextUnformatted("Left Divider Zone");

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

			var value = true;
			var currentItem = 0;
			string[] items = ["Item 1", "Item 2", "Item 3"];

			ImGui.Checkbox("Disabled Checkbox", ref value);
			ImGui.Combo("Disabled Combo", ref currentItem, items, items.Length);
		}

		ImGui.SeparatorText("Tree");
		using (var tree = new ImGuiWidgets.Tree())
		{
			for (var i = 0; i < 5; i++)
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

		ImGui.TextUnformatted("Right Divider Zone");

		ShowTabPanelDemo();
		ShowImageDemo(ktsuTexture);
		ShowGridSettings();
		ShowSearchBoxDemo();
		ShowGridDemo(ktsuTexture);

		MessageOK.ShowIfOpen();
	}

	private void ShowTabPanelDemo()
	{
		ImGui.SeparatorText("TabPanel Demo");

		// Tab Panel controls
		if (ImGui.Button("Mark Active Tab Dirty"))
		{
			DemoTabPanel.MarkActiveTabDirty();
		}

		ImGui.SameLine();

		if (ImGui.Button("Mark Active Tab Clean"))
		{
			DemoTabPanel.MarkActiveTabClean();
		}

		ImGui.SameLine();

		if (ImGui.Button("Add New Tab"))
		{
			var tabIndex = NextDynamicTabId++;
			var tabKey = $"dynamic{tabIndex}";
			var tabId = $"dyntab_{tabIndex}";
			TabIds[tabKey] = DemoTabPanel.AddTab(tabId, $"Extra Tab {tabIndex}", () => ShowDynamicTabContent(tabIndex));
		}

		// Display tab panel
		DemoTabPanel.Draw();

		ImGui.Separator();
	}

	private void ShowImageDemo(ImGuiAppTextureInfo ktsuTexture)
	{
		if (ImGuiWidgets.Image(ktsuTexture.TextureId, new Vector2(128, 128)))
		{
			MessageOK.Open("Click", "You chose the image");
		}

		var iconWidthEms = 7.5f;
		var tilePaddingEms = 0.5f;
		float iconWidthPx = ImGuiApp.EmsToPx(iconWidthEms);
		float tilePaddingPx = ImGuiApp.EmsToPx(tilePaddingEms);

		var iconSize = new Vector2(iconWidthPx, iconWidthPx);

		ImGuiWidgets.Icon("Click Me", ktsuTexture.TextureId, iconWidthPx, ImGuiWidgets.IconAlignment.Vertical,
			new ImGuiWidgets.IconOptions()
			{
				OnClick = () => MessageOK.Open("Click", "You chose Tile1")
			});

		ImGui.SameLine();
		ImGuiWidgets.Icon("Double Click Me", ktsuTexture.TextureId, iconWidthPx, ImGuiWidgets.IconAlignment.Vertical,
			new ImGuiWidgets.IconOptions()
			{
				OnDoubleClick = () => MessageOK.Open("Double Click", "Yippee!!!!!!!!")
			});
		ImGui.SameLine();
		ImGuiWidgets.Icon("Right Click Me", ktsuTexture.TextureId, iconWidthPx, ImGuiWidgets.IconAlignment.Vertical,
			new ImGuiWidgets.IconOptions()
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
			new ImGuiWidgets.IconOptions()
			{
				Tooltip = "You hovered over me"
			});

		ImGui.NewLine();
	}

	private void ShowGridSettings()
	{
		if (ImGui.CollapsingHeader("Grid Settings"))
		{
			var showGridDebug = ImGuiWidgets.EnableGridDebugDraw;
			if (ImGui.Checkbox("Show Grid Debug", ref showGridDebug))
			{
				ImGuiWidgets.EnableGridDebugDraw = showGridDebug;
			}

			var showIconDebug = ImGuiWidgets.EnableIconDebugDraw;
			if (ImGui.Checkbox("Show Icon Debug", ref showIconDebug))
			{
				ImGuiWidgets.EnableIconDebugDraw = showIconDebug;
			}

			{
				var gridIconCenterWithinCell = GridIconCenterWithinCell;
				var gridIconSizeBig = GridIconSizeBig;
				var gridFitToContents = GridFitToContents;
				var gridItemsToShow = GridItemsToShow;
				var gridOrder = GridOrder;
				var gridIconAlignment = GridIconAlignment;
				var gridHeight = GridHeight;

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

				if (ImGui.SliderInt("Items to show", ref gridItemsToShow, 0, GridStrings.Count))
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
	}

	private void ShowSearchBoxDemo()
	{
		if (ImGui.CollapsingHeader("SearchBox Demo"))
		{
			ImGui.TextUnformatted("Search the grid items:");

			ImGui.Separator();
			ImGui.TextUnformatted("Basic SearchBox");

			// Basic search demo - just show the search box control
			ImGuiWidgets.SearchBox("##BasicSearch", ref BasicSearchTerm, ref BasicFilterType, ref BasicMatchOptions);

			ImGui.Separator();
			ImGui.TextUnformatted("Filtered SearchBox with Items");

			// Using the SearchBox that returns filtered results
			var filteredResults = ImGuiWidgets.SearchBox(
				"##FilteredSearch",
				ref FilteredSearchTerm,
				GridStrings,
				s => s,
				ref FilteredFilterType,
				ref FilteredMatchOptions).ToList(); // Materialize the collection

			if (!string.IsNullOrEmpty(FilteredSearchTerm))
			{
				ImGui.TextUnformatted($"Search results for: {FilteredSearchTerm} (Type: {FilteredFilterType}, Match: {FilteredMatchOptions})");
				foreach (var item in filteredResults.Take(10))
				{
					ImGui.TextUnformatted(item);
				}

				// Show count if there are more
				var totalCount = filteredResults.Count;
				if (totalCount > 10)
				{
					ImGui.TextUnformatted($"... and {totalCount - 10} more items");
				}
			}

			ImGui.Separator();
			ImGui.TextUnformatted("Ranked SearchBox");

			// Using a ranked search box
			var rankedResults = ImGuiWidgets.SearchBoxRanked("##RankedSearch",
				ref RankedSearchTerm,
				GridStrings,
				s => s)
				.ToList(); // Materialize the collection to avoid multiple enumerations

			if (!string.IsNullOrEmpty(RankedSearchTerm))
			{
				ImGui.TextUnformatted($"Ranked results for: {RankedSearchTerm} (using fuzzy matching)");
				foreach (var item in rankedResults.Take(10))
				{
					ImGui.TextUnformatted(item);
				}

				// Show count if there are more
				var totalCount = rankedResults.Count;
				if (totalCount > 10)
				{
					ImGui.TextUnformatted($"... and {totalCount - 10} more items");
				}
			}

			ImGui.Separator();
			ImGui.TextUnformatted("Filter Type Comparison");

			// Side-by-side comparison of different filter types
			ImGui.Columns(2);

			ImGui.TextUnformatted("Glob Filter:");
			// Glob filter - pass GridStrings to use search box with filtering
			var globResults = ImGuiWidgets.SearchBox("##GlobSearch",
				ref GlobSearchTerm,
				GridStrings,
				s => s,
				ref GlobFilterType,
				ref GlobMatchOptions)
				.ToList(); // Materialize the collection

			if (!string.IsNullOrEmpty(GlobSearchTerm))
			{
				ImGui.TextUnformatted($"Results for: {GlobSearchTerm}");
				foreach (var item in globResults.Take(10))
				{
					ImGui.TextUnformatted(item);
				}

				var globCount = globResults.Count;
				if (globCount > 10)
				{
					ImGui.TextUnformatted($"... and {globCount - 10} more items");
				}
			}

			ImGui.NextColumn();

			ImGui.TextUnformatted("Regex Filter:");
			// Regex filter - pass GridStrings to use search box with filtering
			var regexResults = ImGuiWidgets.SearchBox("##RegexSearch",
				ref RegexSearchTerm,
				GridStrings,
				s => s,
				ref RegexFilterType,
				ref RegexMatchOptions)
				.ToList(); // Materialize the collection

			if (!string.IsNullOrEmpty(RegexSearchTerm))
			{
				ImGui.TextUnformatted($"Results for: {RegexSearchTerm}");
				foreach (var item in regexResults.Take(10))
				{
					ImGui.TextUnformatted(item);
				}

				var regexCount = regexResults.Count;
				if (regexCount > 10)
				{
					ImGui.TextUnformatted($"... and {regexCount - 10} more items");
				}
			}

			ImGui.Columns(1);
		}
	}

	private void ShowGridDemo(ImGuiAppTextureInfo ktsuTexture)
	{
		float iconSizePx = ImGuiApp.EmsToPx(2.5f);
		var bigIconSizePx = iconSizePx * 2;
		var gridIconSize = GridIconSizeBig ? bigIconSizePx : iconSizePx;
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
			GridSize = new Vector2(ImGui.GetContentRegionAvail().X, GridHeight),
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
	}

	// Tab content methods
	private void ShowTab1Content()
	{
		ImGui.TextUnformatted("This is the content of Tab 1");

		if (ImGui.Button("Edit Content"))
		{
			DemoTabPanel.MarkTabDirty(TabIds["tab1"]);
		}

		if (ImGui.Button("Save Content"))
		{
			DemoTabPanel.MarkTabClean(TabIds["tab1"]);
		}

		ImGui.TextUnformatted("Dirty State: " + (DemoTabPanel.IsTabDirty(TabIds["tab1"]) ? "Modified" : "Unchanged"));
	}

	private void ShowTab2Content()
	{
		ImGui.TextUnformatted("This is the content of Tab 2");

		if (ImGui.SliderFloat("Value", ref tab2Value, 0.0f, 1.0f))
		{
			// Mark tab as dirty when slider value changes
			DemoTabPanel.MarkTabDirty(TabIds["tab2"]);
		}

		if (ImGui.Button("Reset"))
		{
			tab2Value = 0.5f;
			DemoTabPanel.MarkTabClean(TabIds["tab2"]);
		}
	}

	private void ShowTab3Content()
	{
		ImGui.TextUnformatted("This is the content of Tab 3");
		ImGui.TextUnformatted("Try clicking 'Mark Active Tab Dirty' button above");
		ImGui.TextUnformatted("to see the dirty indicator (*) appear next to the tab name.");

		if (ImGui.Button("Toggle Dirty State"))
		{
			if (DemoTabPanel.IsTabDirty(TabIds["tab3"]))
			{
				DemoTabPanel.MarkTabClean(TabIds["tab3"]);
			}
			else
			{
				DemoTabPanel.MarkTabDirty(TabIds["tab3"]);
			}
		}
	}

	private void ShowDynamicTabContent(int tabIndex)
	{
		var tabKey = $"dynamic{tabIndex}";
		ImGui.TextUnformatted($"This is a dynamically added tab ({tabIndex})");
		ImGui.TextUnformatted("The (*) indicator shows when content has been modified.");

		if (ImGui.Button("Toggle Dirty State"))
		{
			if (DemoTabPanel.IsTabDirty(TabIds[tabKey]))
			{
				DemoTabPanel.MarkTabClean(TabIds[tabKey]);
			}
			else
			{
				DemoTabPanel.MarkTabDirty(TabIds[tabKey]);
			}
		}
	}
}
