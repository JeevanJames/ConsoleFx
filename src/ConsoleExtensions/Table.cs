#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleFx.ConsoleExtensions
{
    public sealed class Table
    {
        private readonly List<TableColumn> _columns;
        private readonly List<TableRow> _rows = new List<TableRow>();
        private bool _columnsLocked;
        private int _totalWidth;

        public Table()
        {
            _columns = new List<TableColumn>();
        }

        public Table(params TableColumn[] columns)
        {
            if (columns is null)
                throw new ArgumentNullException(nameof(columns));
            _columns = new List<TableColumn>(columns);
        }

        public Table AddColumn(TableColumn column)
        {
            if (_columnsLocked)
                throw new InvalidOperationException("Cannot add more columns once you start adding data.");
            _columns.Add(column);
            return this;
        }

        public Table AddColumn(string header, TableColumnAlignment alignment = TableColumnAlignment.Left) =>
            AddColumn(new TableColumn(header, alignment));

        public Table AddRow(params object[] data)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (!_columnsLocked)
            {
                _columnsLocked = true;
                CalculateColumnWidths();
            }

            var row = data
                .Take(_columns.Count)
                .Select(d =>
                {
                    if (d is null)
                        return string.Empty;
                    string str = d.ToString() ?? string.Empty;
                    return str.Trim().Length == 0 ? string.Empty : str;
                });
            _rows.Add(new TableRow(row));

            return this;
        }

        private void CalculateColumnWidths()
        {
            int allottedWidth = Console.WindowWidth - _columns.Count - 1;
            int remainingWidth = allottedWidth;

            List<TableColumn> sizedColumns = _columns.Where(column => column.Percentage.HasValue).ToList();
            foreach (TableColumn sizedColumn in sizedColumns)
            {
                sizedColumn.CalculatedWidth = (sizedColumn.Percentage.Value * allottedWidth) / 100;
                remainingWidth -= sizedColumn.CalculatedWidth;
            }

            int remainingColumnWidth = remainingWidth / (_columns.Count - sizedColumns.Count);
            foreach (TableColumn column in _columns)
            {
                if (!column.Percentage.HasValue)
                    column.CalculatedWidth = remainingColumnWidth;
            }

            _totalWidth = _columns.Sum(column => column.CalculatedWidth) + _columns.Count + 1;
        }

        public void Print()
        {
            UpdateRows();
            Console.WriteLine(new string('-', _totalWidth));
            foreach (TableRow row in _rows)
            {
                for (int r = 0; r < row.Height; r++)
                {
                    Console.Write("| ");
                    for (int c = 0; c < _columns.Count; c++)
                    {
                        if (row.Data[c].Length > r)
                            Console.Write(row.Data[c][r]);
                        Console.Write(" | ");
                    }

                    Console.WriteLine();
                }

                Console.WriteLine(new string('-', _totalWidth));
            }
        }

        private void UpdateRows()
        {
            foreach (TableRow row in _rows)
                row.Calculate(_columns);
        }
    }

    public sealed class TableColumn
    {
        public TableColumn(string header, TableColumnAlignment alignment = TableColumnAlignment.Left, int? percentage = null)
        {
            Header = string.IsNullOrWhiteSpace(header) ? string.Empty : header;
            Alignment = alignment;
            Percentage = percentage;
        }

        public string Header { get; }

        public TableColumnAlignment Alignment { get; }

        public int? Percentage { get; }

        internal int CalculatedWidth { get; set; }
    }

    public enum TableColumnAlignment
    {
        Left,
        Center,
        Right,
    }

    internal sealed class TableRow
    {
        internal TableRow(IEnumerable<string> data)
        {
            Data = data.Select(d => new[] { d }).ToList();
        }

        internal List<string[]> Data { get; }

        internal int Height { get; private set; }

        internal void Calculate(IList<TableColumn> columns)
        {
            Height = 1;
            for (int i = 0; i < Data.Count; i++)
            {
                Data[i] = SplitString(Data[i][0], columns[i].CalculatedWidth - 2).ToArray();
                if (Data[i].Length > Height)
                    Height = Data[i].Length;
            }
        }

        private IEnumerable<string> SplitString(string str, int width)
        {
            int start = 0, length = str.Length;
            while (start < length)
            {
                int len = Math.Min(width, length - start);
                yield return str.Substring(start, len).PadRight(width);
                start += width;
            }
        }
    }
}
