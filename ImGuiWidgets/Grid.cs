namespace ktsu.ImGuiWidgets;

using System.Numerics;
using ImGuiNET;

public static partial class ImGuiWidgets
{
	public static void Grid<T>(IEnumerable<T> items, Func<T, Vector2> measureDelegate, Action<T, Vector2> drawDelegate) =>
	GridImpl.Show(items, measureDelegate, drawDelegate);

	internal static class GridImpl
	{
		public static void Show<T>(IEnumerable<T> items, Func<T, Vector2> measureDelegate, Action<T, Vector2> drawDelegate)
		{
			var itemSpacing = ImGui.GetStyle().ItemSpacing;
			var itemList = items.ToArray();
			var itemDimensions = items.Select(i => measureDelegate(i) + itemSpacing).ToArray();
			var contentRegionAvailable = ImGui.GetContentRegionAvail();
			int numColumns = 1;
			bool columnFirst = true;

			while (numColumns < itemList.Length)
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
							maxRowWidth += colItems.Max(item => item.X);
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

						rowWidth += itemDimensions[i].X;
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

			ImGui.TextUnformatted($"Num Columns: {numColumns}");

			int numRows = (int)Math.Ceiling((float)itemList.Length / numColumns);

			// calculate column widths and row heights
			float[] columnWidths = new float[numColumns];
			float[] rowHeights = new float[numRows];

			for (int i = 0; i < numColumns * numRows; i++)
			{
				var itemStartCursor = ImGui.GetCursorScreenPos();
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

				var cellCize = new Vector2(columnWidths[column], rowHeights[row]);

				if (itemIndex < itemList.Length)
				{
					drawDelegate(itemList[itemIndex], cellCize);
				}

				var advance = new Vector2(marginTopLeftCursor.X, itemStartCursor.Y + cellCize.Y);
				if (column < numColumns - 1)
				{
					advance = new Vector2(itemStartCursor.X + cellCize.X, itemStartCursor.Y);
				}
				ImGui.SetCursorScreenPos(advance);
			}
		}
	}
}
