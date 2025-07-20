// Copyright (c) ktsu.dev
// All rights reserved.
// Licensed under the MIT license.

namespace ktsu.ImGuiWidgetsDemo;

using System.Collections.ObjectModel;
using System.Numerics;
using Hexa.NET.ImGui;
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

internal static class ImGuiWidgetsDemo
{
	private static void Main()
	{
		ImGuiApp.Start(new()
		{
			Title = "ImGuiWIdgets Demo",
			OnStart = OnStart,
			OnAppMenu = OnAppMenu,
			OnMoveOrResize = OnMoveOrResize,
			OnRender = OnRender,
		});
	}

	private static float value = 0.5f;
	private static float tab2Value = 0.5f;

	private static ImGuiWidgets.DividerContainer DividerContainer { get; } = new("DemoDividerContainer");
	private static ImGuiPopups.MessageOK MessageOK { get; } = new();
	private static ImGuiWidgets.TabPanel DemoTabPanel { get; } = new("DemoTabPanel", true, true);
	private static Dictionary<string, string> TabIds { get; } = [];
	private static int NextDynamicTabId { get; set; } = 1;

	private static List<string> GridStrings { get; } = [];
	private static int InitialGridItemCount { get; } = 32;
	private static int GridItemsToShow { get; set; } = InitialGridItemCount;
	private static float GridHeight { get; set; } = 500f;
	private static ImGuiWidgets.GridOrder GridOrder { get; set; } = ImGuiWidgets.GridOrder.RowMajor;
	private static ImGuiWidgets.IconAlignment GridIconAlignment { get; set; } = ImGuiWidgets.IconAlignment.Vertical;
	private static bool GridIconSizeBig { get; set; } = true;
	private static bool GridIconCenterWithinCell { get; set; } = true;
	private static bool GridFitToContents { get; set; }
	private static EnumValues selectedEnumValue = EnumValues.Value1;
	private static string selectedStringValue = "Hello";
	private static readonly Collection<string> possibleStringValues = ["Hello", "World", "Goodbye"];
	private static StrongStringExample selectedStrongString = "Strong Hello".As<StrongStringExample>();
	private static readonly Collection<StrongStringExample> possibleStrongStringValues = ["Strong Hello".As<StrongStringExample>(),
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
	private static void OnStart()
	{
		DividerContainer.Add(new("Left", 0.25f, ShowLeftPanel));
		DividerContainer.Add(new("Right", 0.75f, ShowRightPanel));

		// Initialize TabPanel demo
		TabIds["tab1"] = DemoTabPanel.AddTab("tab1", "Tab 1", ShowTab1Content);
		TabIds["tab2"] = DemoTabPanel.AddTab("tab2", "Tab 2", ShowTab2Content);
		TabIds["tab3"] = DemoTabPanel.AddTab("tab3", "Tab 3", ShowTab3Content);

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

	private static void OnRender(float dt) => DividerContainer.Tick(dt);

	private static void OnAppMenu()
	{
		// Method intentionally left empty.
	}

	private static void OnMoveOrResize()
	{
		// Method intentionally left empty.
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
	private static void ShowLeftPanel(float size)
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

			bool value = true;
			int currentItem = 0;
			string[] items = ["Item 1", "Item 2", "Item 3"];

			ImGui.Checkbox("Disabled Checkbox", ref value);
			ImGui.Combo("Disabled Combo", ref currentItem, items, items.Length);
		}

		ImGui.SeparatorText("Tree");
		using (ImGuiWidgets.Tree tree = new())
		{
			for (int i = 0; i < 5; i++)
			{
				using (tree.Child)
				{
					ImGui.Button($"Hello, Child {i}!");
					using (ImGuiWidgets.Tree subtree = new())
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

	private static void ShowRightPanel(float size)
	{
		AbsoluteFilePath ktsuIconPath = (AbsoluteDirectoryPath)Environment.CurrentDirectory / (FileName)"ktsu.png";
		ImGuiAppTextureInfo ktsuTexture = ImGuiApp.GetOrLoadTexture(ktsuIconPath);

		ImGui.TextUnformatted("Right Divider Zone");

		ShowTabPanelDemo();
		ShowImageDemo(ktsuTexture);
		ShowGridSettings();
		ShowSearchBoxDemo();
		ShowGridDemo(ktsuTexture);

		MessageOK.ShowIfOpen();
	}

	private static void ShowTabPanelDemo()
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
			int tabIndex = NextDynamicTabId++;
			string tabKey = $"dynamic{tabIndex}";
			string tabId = $"dyntab_{tabIndex}";
			TabIds[tabKey] = DemoTabPanel.AddTab(tabId, $"Extra Tab {tabIndex}", () => ShowDynamicTabContent(tabIndex));
		}

		// Display tab panel
		DemoTabPanel.Draw();

		ImGui.Separator();
	}

	private static void ShowImageDemo(ImGuiAppTextureInfo ktsuTexture)
	{
		if (ImGuiWidgets.Image(ktsuTexture.TextureId, new Vector2(128, 128)))
		{
			MessageOK.Open("Click", "You chose the image");
		}

		float iconWidthEms = 7.5f;
		float tilePaddingEms = 0.5f;
		float iconWidthPx = ImGuiApp.EmsToPx(iconWidthEms);
		float tilePaddingPx = ImGuiApp.EmsToPx(tilePaddingEms);

		Vector2 iconSize = new(iconWidthPx, iconWidthPx);

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

	private static void ShowGridSettings()
	{
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
				ImGuiWidgets.GridOrder gridOrder = GridOrder;
				ImGuiWidgets.IconAlignment gridIconAlignment = GridIconAlignment;
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

	private static void ShowSearchBoxDemo()
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
			List<string> filteredResults = [.. ImGuiWidgets.SearchBox(
				"##FilteredSearch",
				ref FilteredSearchTerm,
				GridStrings,
				s => s,
				ref FilteredFilterType,
				ref FilteredMatchOptions)]; // Materialize the collection

			if (!string.IsNullOrEmpty(FilteredSearchTerm))
			{
				ImGui.TextUnformatted($"Search results for: {FilteredSearchTerm} (Type: {FilteredFilterType}, Match: {FilteredMatchOptions})");
				foreach (string? item in filteredResults.Take(10))
				{
					ImGui.TextUnformatted(item);
				}

				// Show count if there are more
				int totalCount = filteredResults.Count;
				if (totalCount > 10)
				{
					ImGui.TextUnformatted($"... and {totalCount - 10} more items");
				}
			}

			ImGui.Separator();
			ImGui.TextUnformatted("Ranked SearchBox");

			// Using a ranked search box
			List<string> rankedResults = [.. ImGuiWidgets.SearchBoxRanked("##RankedSearch",
				ref RankedSearchTerm,
				GridStrings,
				s => s)]; // Materialize the collection to avoid multiple enumerations

			if (!string.IsNullOrEmpty(RankedSearchTerm))
			{
				ImGui.TextUnformatted($"Ranked results for: {RankedSearchTerm} (using fuzzy matching)");
				foreach (string? item in rankedResults.Take(10))
				{
					ImGui.TextUnformatted(item);
				}

				// Show count if there are more
				int totalCount = rankedResults.Count;
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
			List<string> globResults = [.. ImGuiWidgets.SearchBox("##GlobSearch",
				ref GlobSearchTerm,
				GridStrings,
				s => s,
				ref GlobFilterType,
				ref GlobMatchOptions)]; // Materialize the collection

			if (!string.IsNullOrEmpty(GlobSearchTerm))
			{
				ImGui.TextUnformatted($"Results for: {GlobSearchTerm}");
				foreach (string? item in globResults.Take(10))
				{
					ImGui.TextUnformatted(item);
				}

				int globCount = globResults.Count;
				if (globCount > 10)
				{
					ImGui.TextUnformatted($"... and {globCount - 10} more items");
				}
			}

			ImGui.NextColumn();

			ImGui.TextUnformatted("Regex Filter:");
			// Regex filter - pass GridStrings to use search box with filtering
			List<string> regexResults = [.. ImGuiWidgets.SearchBox("##RegexSearch",
				ref RegexSearchTerm,
				GridStrings,
				s => s,
				ref RegexFilterType,
				ref RegexMatchOptions)]; // Materialize the collection

			if (!string.IsNullOrEmpty(RegexSearchTerm))
			{
				ImGui.TextUnformatted($"Results for: {RegexSearchTerm}");
				foreach (string? item in regexResults.Take(10))
				{
					ImGui.TextUnformatted(item);
				}

				int regexCount = regexResults.Count;
				if (regexCount > 10)
				{
					ImGui.TextUnformatted($"... and {regexCount - 10} more items");
				}
			}

			ImGui.Columns(1);
		}
	}

	private static void ShowGridDemo(ImGuiAppTextureInfo ktsuTexture)
	{
		float iconSizePx = ImGuiApp.EmsToPx(2.5f);
		float bigIconSizePx = iconSizePx * 2;
		float gridIconSize = GridIconSizeBig ? bigIconSizePx : iconSizePx;
		Vector2 gridSize = new(ImGui.GetContentRegionAvail().X, GridHeight);

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
	private static void ShowTab1Content()
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

	private static void ShowTab2Content()
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

	private static void ShowTab3Content()
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

	private static void ShowDynamicTabContent(int tabIndex)
	{
		string tabKey = $"dynamic{tabIndex}";
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
