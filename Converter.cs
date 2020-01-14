using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Excel2JsonConverter
{
    public partial class Converter : Form
    {
        public Converter()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var convertedFiles = new List<string>();
            var sourcePath = openFileDialog.FileName;
            var fileStream = openFileDialog.OpenFile();

            using var package = new ExcelPackage(fileStream);
            foreach (var worksheet in package.Workbook.Worksheets)
            {
                var data = new List<JObject>();

                var rowCount = worksheet.Dimension.Rows;
                var columnCount = worksheet.Dimension.Columns;

                var headers = new List<string>();

                for (int col = 1; col <= columnCount; col++)
                {
                    headers.Add(worksheet.Cells[1, col].Value.ToString().Trim());
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    dynamic obj = new JObject();

                    for (int col = 1; col <= columnCount; col++)
                    {
                        obj[headers[col - 1]] = worksheet.Cells[row, col].Value?.ToString().Trim();
                    }
                    data.Add(obj);
                }

                var destPath = $"{sourcePath.Substring(0, sourcePath.LastIndexOf('.'))}.json";
                using StreamWriter file = File.CreateText(destPath);
                new JsonSerializer().Serialize(file, data);
                convertedFiles.Add(destPath);
            }
            MessageBox.Show($"Converted Files: {string.Join('\n', convertedFiles)}");
        }
    }
}