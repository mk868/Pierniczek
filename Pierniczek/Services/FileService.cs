using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pierniczek.Services
{
    public class FileService : IFileService
    {
        public const int MaxLines = 7;


        private string[] SplitLine(string line)
        {
            return line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
        }

        private TypeEnum GetCellType(string value)
        {
            if (Regex.IsMatch(value, @"^[\-]?[0-9]*([,.][0-9]+)?$"))
                return TypeEnum.Number;
            return TypeEnum.String;
        }

        /// <summary>
        /// Is header row
        /// </summary>
        /// <returns></returns>
        private bool IsHeader(string[] cells)
        {
            foreach (var cell in cells)
            {
                if (Regex.Matches(cell, @"[a-zA-Z]").Count == 0)
                {//its value, not header
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Creates list of Fields
        /// </summary>
        /// <param name="columns">
        /// splitted first row with data
        /// </param>
        /// <param name="values">
        /// splitted second row with data
        /// </param>
        /// <returns></returns>
        private List<ColumnModel> GenerateColumns(string[] columns, string[] values)
        {
            var result = new List<ColumnModel>();

            var hasHeader = IsHeader(columns);

            for (var i = 0; i < columns.Length; i++)
            {
                var name = hasHeader ? columns[i] : $"column_{i + 1}";

                result.Add(new ColumnModel()
                {
                    Name = name,
                    Type = GetCellType(values[i]),
                    Use = true,
                    Description = $"Example value: {values[i]}"
                });
            }

            return result;
        }


        //TODO: move to extension method
        private string GetNextLine(StreamReader reader)
        {
            for (; !reader.EndOfStream;)
            {
                var line = reader.ReadLine();

                if (String.IsNullOrWhiteSpace(line))
                    continue;

                if (line.Trim().StartsWith("#"))
                    continue;

                return line;
            }

            return null;
        }



        public IList<ColumnModel> GenerateColumns(string filePath)
        {
            string[] columns = null;
            string[] values = null;

            using (StreamReader reader = new StreamReader(filePath))
            {
                for (; !reader.EndOfStream;)
                {
                    var line = GetNextLine(reader);

                    if (line == null)
                        break;

                    if (columns == null)
                    {
                        columns = SplitLine(line);
                    }
                    else if (values == null)
                    {
                        values = SplitLine(line);
                        break;
                    }
                }
            }

            if (columns == null || values == null)
                return null;

            return GenerateColumns(columns, values);
        }

        public string PreviewFile(string filePath)
        {
            return PreviewFile(filePath, MaxLines);
        }

        public string PreviewFile(string filePath, int lines)
        {
            var result = new StringBuilder();

            using (StreamReader reader = new StreamReader(filePath))
            {
                for (var i = 0; i < MaxLines && !reader.EndOfStream; i++)
                {
                    result.AppendLine(reader.ReadLine());
                }
            }

            return result.ToString();
        }


        private object Convert(TypeEnum type, string value)
        {
            if (type == TypeEnum.Number)
            {
                value = value.Replace('.', ',');
                if (value.StartsWith(","))
                    value = "0" + value;

                if (decimal.TryParse(value, out var dval))
                {
                    return dval;
                }
                return default(decimal);
            }

            return value;
        }

        public IList<RowModel> GetRows(string filePath, IList<ColumnModel> columns)
        {
            var result = new List<RowModel>();

            bool headerSkipped = false;

            using (StreamReader reader = new StreamReader(filePath))
            {
                for (; !reader.EndOfStream;)
                {
                    var line = GetNextLine(reader);

                    if (line == null)
                        break;

                    var cells = SplitLine(line);

                    if (!headerSkipped)
                    {
                        headerSkipped = true;

                        if (IsHeader(cells))
                        {
                            continue;
                        }
                    }

                    var row = new RowModel();

                    for (var i = 0; i < columns.Count && i < cells.Length; i++)
                    {
                        var value = cells[i];
                        var column = columns[i];

                        row[column.Name] = Convert(column.Type, value);
                    }

                    result.Add(row);
                }
            }

            return result;
        }

        public void SaveToFile(string filePath, IList<ColumnModel> columns, IList<RowModel> rows)
        {

            using (var fs = File.CreateText(filePath))
            {
                fs.WriteLine(string.Join("\t", columns.Select(s => s.Name).ToArray()));

                foreach (var row in rows)
                {
                    var objs = new List<string>();

                    foreach (var column in columns)
                    {
                        var rowValue = row[column.Name];

                        if (rowValue is string)
                        {
                            objs.Add(rowValue as string);
                            continue;
                        }

                        if (rowValue is decimal)
                        {
                            objs.Add(((decimal)rowValue).ToString());
                            continue;
                        }

                        if (rowValue is double)
                        {
                            objs.Add(((double)rowValue).ToString());
                            continue;
                        }

                        if (rowValue is int)
                        {
                            objs.Add(((int)rowValue).ToString());
                            continue;
                        }
                        throw new NotImplementedException("not known type");
                    }

                    fs.WriteLine(string.Join("\t", objs.ToArray()));
                }
            }

        }
    }
}
