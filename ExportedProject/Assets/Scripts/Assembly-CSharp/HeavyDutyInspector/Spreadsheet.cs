using System;
using System.Collections.Generic;
using System.Linq;

namespace HeavyDutyInspector
{
	public class Spreadsheet
	{
		protected List<List<string>> spreadsheet = new List<List<string>>();

		public List<string> GetRow(int row)
		{
			return spreadsheet[row];
		}

		public List<string> GetRow(string row)
		{
			return spreadsheet.Where((List<string> r) => r[0] == row).FirstOrDefault();
		}

		public string GetValue(int row, int column)
		{
			try
			{
				return spreadsheet[row][column];
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetValue(int row, string column)
		{
			return spreadsheet[row][spreadsheet[0].IndexOf(column)];
		}

		public string GetValue(string row, int column)
		{
			try
			{
				return spreadsheet.Where((List<string> r) => r[0] == row).FirstOrDefault()[column];
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetValue(string row, string column)
		{
			return spreadsheet.Where((List<string> r) => r[0] == row).FirstOrDefault()[spreadsheet[0].IndexOf(column)];
		}

		public string GetVariableName(int column)
		{
			return spreadsheet[0][column].Split('_')[0];
		}

		public int GetIndexOf(string column)
		{
			int num = spreadsheet[0].IndexOf(column);
			if (num == -1)
			{
				throw new Exception(string.Format("Colum {0} was not found", column));
			}
			return num;
		}

		public int GetNbRows()
		{
			return spreadsheet.Count;
		}

		public int GetNbColumns()
		{
			return spreadsheet[0].Count;
		}

		public void Load(string content)
		{
			spreadsheet.Clear();
			string[] array = content.Split('\n');
			string[] array2 = array;
			foreach (string text in array2)
			{
				spreadsheet.Add(text.Split('\t').ToList());
			}
			foreach (List<string> item in spreadsheet)
			{
				while (item.Count < spreadsheet[0].Count)
				{
					item.Add(string.Empty);
				}
			}
		}
	}
}
