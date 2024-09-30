namespace ktsu.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;

/// <summary>
/// Provides custom ImGui widgets.
/// </summary>
public static partial class ImGuiWidgets
{
	/// <summary>
	/// Delegate to measure the size of a grid cell.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="item">The item to measure.</param>
	/// <returns>The size of the item.</returns>
	public delegate Vector2 MeasureGridCell<T>(T item);

	/// <summary>
	/// Delegate to draw a grid cell.
	/// </summary>
	/// <typeparam name="T">The type of the item.</typeparam>
	/// <param name="item">The item to draw.</param>
	/// <param name="cellSize">The calculated size of the grid cell.</param>
	/// <param name="itemSize">The calculated size of the item.</param>
	public delegate void DrawGridCell<T>(T item, Vector2 cellSize, Vector2 itemSize);

	/// <summary>
	/// Renders a grid with the specified items and delegates.
	/// </summary>
	/// <typeparam name="T">The type of the items.</typeparam>
	/// <param name="items">The items to be displayed in the grid.</param>
	/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
	/// <param name="drawDelegate">The delegate to draw each item.</param>
	public static void Grid<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate) =>
		GridImpl.Show(items, measureDelegate, drawDelegate);

	/// <summary>
	/// Contains the implementation details for rendering grids.
	/// </summary>
	internal static class GridImpl
	{
		/// <summary>
		/// Shows the grid with the specified items and delegates.
		/// </summary>
		/// <typeparam name="T">The type of the items.</typeparam>
		/// <param name="items">The items to be displayed in the grid.</param>
		/// <param name="measureDelegate">The delegate to measure the size of each item.</param>
		/// <param name="drawDelegate">The delegate to draw each item.</param>
		public static void Show<T>(IEnumerable<T> items, MeasureGridCell<T> measureDelegate, DrawGridCell<T> drawDelegate)
		{
			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			var itemDimensions = items.Select(i => measureDelegate(i) + itemSpacing).ToArray();
			var contentRegionAvailable = ImGui.GetContentRegionAvail();
			int numColumns = 1;
			bool columnFirst = true;

			while (numColumns <= itemList.Length)
			{
				int numRowsForColumns = (int)Math.Ceiling((float)itemList.Length / numColumns);

				float maxRowWidth = 0f;

				if (columnFirst)
				{
					// column first layout
					for (int i = 0; i < numColumns; i++)
					{
						int colOffset = i * numRowsForColumns;
						var colItems = itemDimensions.Skip(colOffset).Take(numRowsForColumns);
						if (colItems.Any())
						{
							maxRowWidth += colItems.Max(item => item.X) + itemSpacing.X;
						}
					}
				}
				else
				{
					// row first layout
					float rowWidth = 0f;

					for (int i = 0; i < itemList.Length; i++)
					{
						if (i % numColumns == 0)
						{
							rowWidth = 0f;
						}

						rowWidth += itemDimensions[i].X + itemSpacing.X;
						maxRowWidth = Math.Max(maxRowWidth, rowWidth);
					}
				}

				if (maxRowWidth > contentRegionAvailable.X)
				{
					numColumns--;
					break;
				}
				numColumns++;
			}

			if (numColumns < 1)
			{
				numColumns = 1;
			}

			int numRows = (int)Math.Ceiling((float)itemList.Length / numColumns);

			// calculate column widths and row heights
			float[] columnWidths = new float[numColumns];
			float[] rowHeights = new float[numRows];

			for (int i = 0; i < numColumns * numRows; i++)
			{
				int column = i % numColumns;
				int row = i / numColumns;

				int itemIndex = columnFirst
					? (column * numRows) + row
					: i;

				if (itemIndex < itemList.Length)
				{
					var thisItemSize = itemDimensions[itemIndex];

					columnWidths[column] = Math.Max(columnWidths[column], thisItemSize.X);
					rowHeights[row] = Math.Max(rowHeights[row], thisItemSize.Y);
				}
			}

			float totalContentWidth = columnWidths.Sum();
			float extraSpace = contentRegionAvailable.X - totalContentWidth;
			float extraSpacePerColumn = extraSpace / numColumns;

			for (int i = 0; i < numColumns; i++)
			{
				columnWidths[i] += extraSpacePerColumn;
			}

			var marginTopLeftCursor = ImGui.GetCursorScreenPos();

			int numCells = numColumns * numRows;
			for (int i = 0; i < numCells; i++)
			{
				var itemStartCursor = ImGui.GetCursorScreenPos();
				int column = i % numColumns;
				int row = i / numColumns;

				int itemIndex = columnFirst
					? (column * numRows) + row
					: i;

				var cellSize = new Vector2(columnWidths[column], rowHeights[row]);

				if (itemIndex < itemList.Length)
				{
					drawDelegate(itemList[itemIndex], cellSize, itemDimensions[itemIndex]);
				}

				var advance = new Vector2(marginTopLeftCursor.X, itemStartCursor.Y + cellSize.Y);
				if (column < numColumns - 1)
				{
					advance = new Vector2(itemStartCursor.X + cellSize.X, itemStartCursor.Y);
				}
				ImGui.SetCursorScreenPos(advance);
			}
		}
	}
}
