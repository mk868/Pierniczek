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
        public const int DefaultMaxLines = 7;

        private string[] SplitLine(string line, FileDataSeparatorEnum separator)
        {
            if (separator == FileDataSeparatorEnum.WhiteChars)
            {
                return line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            }
            List<char> chars = new List<char>();

            if (separator.HasFlag(FileDataSeparatorEnum.Tab))
            {
                chars.Add('\t');
            }
            if (separator.HasFlag(FileDataSeparatorEnum.Dot))
            {
                chars.Add('.');
            }
            if (separator.HasFlag(FileDataSeparatorEnum.Comma))
            {
                chars.Add(',');
            }
            if (separator.HasFlag(FileDataSeparatorEnum.Semicolon))
            {
                chars.Add(';');
            }
            if (separator.HasFlag(FileDataSeparatorEnum.Space))
            {
                chars.Add(' ');
            }

            return line.Split(chars.ToArray());
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


        public string PreviewFile(string filePath, int lines)
        {
            var result = new StringBuilder();

            using (StreamReader reader = new StreamReader(filePath))
            {
                for (var i = 0; i < lines && !reader.EndOfStream; i++)
                {
                    result.AppendLine(reader.ReadLine());
                }
            }

            return result.ToString();
        }

        public string PreviewFile(string filePath)
        {
            return this.PreviewFile(filePath, DefaultMaxLines);
        }


        private object Convert(TypeEnum type, string value)
        {
            if (type == TypeEnum.Number)
            {
                value = value.Replace('.', ',');
                if (value.StartsWith(","))
                    value = "0" + value;

                if (double.TryParse(value, out var dval))
                {
                    return dval;
                }
                return default(double);
            }

            return value;
        }


        public void Save(string filePath, DataModel data, FileDataSeparatorEnum separator)
        {
            var separatorText = GetSeparatorText(separator);

            using (var fs = File.CreateText(filePath))
            {
                fs.WriteLine(string.Join(separatorText, data.Columns.Select(s => s.Name).ToArray()));

                foreach (var row in data.Rows)
                {
                    var objs = new List<string>();

                    foreach (var column in data.Columns)
                    {
                        var rowValue = row[column.Name];

                        if (column.Type == TypeEnum.String)
                        {
                            objs.Add(rowValue.ToString());
                            continue;
                        }

                        if (column.Type == TypeEnum.Number)
                        {
                            objs.Add(((double)rowValue).ToString().Replace(',', '.'));//FIXME
                            continue;
                        }

                        throw new NotImplementedException("not known type");
                    }

                    fs.WriteLine(string.Join(separatorText, objs.ToArray()));
                }
            }
        }

        private string GetSeparatorText(FileDataSeparatorEnum separator)
        {
            if (separator.HasFlag(FileDataSeparatorEnum.Dot))
            {
                return ".";
            }
            if (separator.HasFlag(FileDataSeparatorEnum.Semicolon))
            {
                return ";";
            }
            if (separator.HasFlag(FileDataSeparatorEnum.Space))
            {
                return " ";
            }
            if (separator.HasFlag(FileDataSeparatorEnum.Comma))
            {
                return ",";
            }

            return "\t";
        }

        public DataModel ReloadRows(string filePath, DataModel dataModel, FileDataSeparatorEnum separator)
        {
            dataModel.Rows.Clear();
            bool headerSkipped = false;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = GetNextLine(reader)) != null)
                {
                    var cells = SplitLine(line, separator);

                    if (!headerSkipped)
                    {
                        headerSkipped = true;

                        if (IsHeader(cells))
                        {
                            continue;
                        }
                    }

                    var row = new RowModel();

                    for (var i = 0; i < dataModel.Columns.Count && i < cells.Length; i++)
                    {
                        var value = cells[i];
                        var column = dataModel.Columns[i];

                        row[column.Name] = Convert(column.Type, value);
                    }

                    dataModel.Rows.Add(row);
                }
            }

            return dataModel;
        }



        public DataModel LoadDefinitions(string filePath, FileDataSeparatorEnum separator)
        {
            string[] columns = null;
            string[] values = null;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = GetNextLine(reader)) != null)
                {
                    if (columns == null)
                    {
                        columns = SplitLine(line, separator);
                        continue;
                    }

                    if (values == null)
                    {
                        values = SplitLine(line, separator);
                        break;
                    }
                }
            }

            if (columns == null || values == null)
                return null;

            var result = new DataModel();
            GenerateColumns(columns, values)
                .ForEach(c => result.Columns.Add(c));

            return result;
        }

        public DataModel Load(string filePath, FileDataSeparatorEnum separator)
        {
            var result = this.LoadDefinitions(filePath, separator);
            return this.ReloadRows(filePath, result, separator);
        }

        public FileDataSeparatorEnum DetectSeparatorType(string fileName)
        {
            var ext = Path.GetExtension(fileName)
               .Remove(0, 1) // remove dot
               .ToLower();

            if(ext == "csv")
            {
                return FileDataSeparatorEnum.Semicolon | FileDataSeparatorEnum.Comma;
            }

            return FileDataSeparatorEnum.WhiteChars;
        }
    }
}
