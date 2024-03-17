using DNTPersianUtils.Core;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using ViewModel;
using ViewModel.Setting;

namespace Services
{
    public interface IExcelReportService
    {
        ExcelWorksheet GetWorkSheet(string base64);
        byte[] Report<T>(List<T> data, List<Column> columns);

    }
    public class ExcelReportService : IExcelReportService
    {
        private readonly AppSettings _options;
        public ExcelReportService(IOptionsSnapshot<AppSettings> options)
        {
            _options = options.Value;
        }
        public ExcelWorksheet GetWorkSheet(string fileData)
        {
            if (string.IsNullOrEmpty(fileData))
            {
                return null;
            }
            var base64 = fileData.Split(',')[1];
            try
            {
                var byteArray = Convert.FromBase64String(base64);

                using var memStream = new MemoryStream(byteArray, 0, byteArray.Length);
                var package = new ExcelPackage(memStream);
                package.Load(memStream);
                var worksheet = package.Workbook.Worksheets[1];
                return worksheet;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public byte[] Report<T>(List<T> data, List<Column> columns)
        {
            var stream = new MemoryStream();
            using var package = new ExcelPackage(stream);
            var reportName = "report";
            var worksheet = package.Workbook.Worksheets.Add(reportName);

            columns = columns.Where(w => !string.IsNullOrEmpty(w.Name)).ToList();
            columns = columns.Where(w => w.Selected).ToList();
            columns = columns.Where(w => w.HideInExcel == false).ToList();

            var rowCounter = 0;

            AddHeaderReport(worksheet, columns.Count, rowCounter += 1);
            AdHeaderColumns(worksheet, columns, rowCounter += 1, true);

            AddBody(data, worksheet, columns, rowCounter += 1, true);
            if (data.Count < 5000)
            {
                worksheet.Cells.AutoFitColumns();
            }
            
            worksheet.View.RightToLeft = true;

            return package.GetAsByteArray();
        }
        private void AddHeaderReport(ExcelWorksheet worksheet, int count, int rowNUmber)
        {
            var row = worksheet.Cells[rowNUmber, 1, 1, count + 1];
            row.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            row.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row.Style.Fill.PatternType = ExcelFillStyle.Solid;
            row.Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
            row.Merge = true;
            row.Value = _options.ApplicationPersianName + $"( {DateTime.Now.ToShortPersianDateString()} )";
            worksheet.Row(rowNUmber).Height = 40;
        }
        private void AdHeaderColumns(ExcelWorksheet worksheet, List<Column> columns,
           int rowNUmber, bool? showFirstColumn)
        {
            var cellNumber = 1;
            if (showFirstColumn is null)
            {
                worksheet.Cells[rowNUmber, cellNumber].Value = "#";
                worksheet.Cells[rowNUmber, cellNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNUmber, cellNumber].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                worksheet.Cells[rowNUmber, cellNumber].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowNUmber, cellNumber].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                cellNumber += 1;
            }

            foreach (var column in columns)
            {
                worksheet.Cells[rowNUmber, cellNumber].Value = column.Title;
                worksheet.Cells[rowNUmber, cellNumber].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[rowNUmber, cellNumber].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                cellNumber += 1;
            }
            worksheet.Row(rowNUmber).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(rowNUmber).Height = 40;
        }

        private void AddBody<T>(List<T> data, ExcelWorksheet worksheet,
           List<Column> columns, int row, bool? showFirstColumn)
        {
            var increase = showFirstColumn is null ? 1 : 0;
            foreach (var item in data)
            {
                worksheet.Cells[row, 1].Value = data.IndexOf(item) + increase;

                foreach (var column in columns)
                {
                    var i = columns.IndexOf(column) + increase;
                    if (i <= columns.Count)
                    {
                        var name = columns[i - increase].Name;
                        var property = item.GetType().GetProperties()
                            .FirstOrDefault(f => f.Name == name);
                        if (property == null)
                            continue;
                        var value = property.GetValue(item);

                        value = ReplaceBooleanValue(value);

                        var cellNumber = i + 1;
                        worksheet.Cells[row, cellNumber].Value = value;

                        if (property.PropertyType.FullName != null &&
                            property.PropertyType.FullName.Contains("System.TimeSpan"))
                        {
                            worksheet.Cells[row, cellNumber]
                                .Style.Numberformat.Format = "hh:mm:ss";
                        }

                        if (value != null && (value.ToString() == "✓" ||
                            value.ToString().ToLower() == "✕"))
                        {
                            worksheet.Cells[row, cellNumber].Style.HorizontalAlignment =
                                ExcelHorizontalAlignment.Center;
                        }
                        else
                        {
                            worksheet.Cells[row, cellNumber]
                                .Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                    }
                }
                row += 1;
            }
        }

        private object ReplaceBooleanValue(object value)
        {
            if (value != null)
            {
                if (value.ToString().ToLower() == "true")
                {
                    value = "✓";
                }
                if (value.ToString().ToLower() == "false")
                {
                    value = "✕";
                }
            }
            return value;
        }
    }
}
