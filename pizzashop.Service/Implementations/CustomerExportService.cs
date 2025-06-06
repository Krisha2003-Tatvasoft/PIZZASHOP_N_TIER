using ClosedXML.Excel;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;

namespace pizzashop.Service.Implementations;

public class CustomerExportService : ICustomerExportService
{
     public byte[] ExportCustomerToExcel(List<CustomerTable> customers, string searchText, DateTime? fromDate, DateTime? toDate)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Orders");

          
            // **HEADER SECTION**
            worksheet.Range("A2:B3").Merge().Value = "Account:";
            worksheet.Range("C2:F3").Merge().Value = "";


            worksheet.Range("A5:B6").Merge().Value = "Date:";
            worksheet.Range("C5:F6").Merge().Value = fromDate.HasValue ? $"{fromDate:dd-MM-yyyy} to {toDate:dd-MM-yyyy}" : "AllTime";

            worksheet.Range("H2:I3").Merge().Value = "Search Text:";
            worksheet.Range("J2:M3").Merge().Value = searchText;

            worksheet.Range("H5:I6").Merge().Value = "No. Of Records:";
            worksheet.Range("J5:M6").Merge().Value = customers.Count;



            var headerRange1 = worksheet.Range("A2:B3");
            var headerRange2 = worksheet.Range("A5:B6");
            var headerRange3 = worksheet.Range("H2:I3");
            var headerRange4 = worksheet.Range("H5:I6");

            void ApplyCommonStyle(IXLRange headerRange)
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#087cc4");
                headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange.Style.Font.FontColor = XLColor.White;
            }
            ApplyCommonStyle(headerRange1);
            ApplyCommonStyle(headerRange2);
            ApplyCommonStyle(headerRange3);
            ApplyCommonStyle(headerRange4);

            void styleHeaderValue(IXLRange headervalue)
            {
                headervalue.Style.Font.Bold = true;
                headervalue.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headervalue.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headervalue.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headervalue.Style.Font.FontColor = XLColor.FromHtml("#000000");
            }

            var headerValue1 = worksheet.Range("C2:F3");
            var headerValue2 = worksheet.Range("C5:F6");
            var headerValue3 = worksheet.Range("J2:M3");
            var headerValue4 = worksheet.Range("J5:M6");

            styleHeaderValue(headerValue1);
            styleHeaderValue(headerValue2);
            styleHeaderValue(headerValue3);
            styleHeaderValue(headerValue4);



            // **TABLE HEADER**
            var tableHeader = worksheet.Range("A9:P9");
            tableHeader.Style.Font.Bold = true;
            tableHeader.Style.Fill.BackgroundColor = XLColor.FromHtml("#087cc4");
            tableHeader.Style.Font.FontColor =XLColor.White;
            tableHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            tableHeader.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            tableHeader.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableHeader.Style.Border.InsideBorder = XLBorderStyleValues.Thin;


            worksheet.Cell(9, 1).Value = "Id";
            worksheet.Range("B9:D9").Merge().Value = "Name";
            worksheet.Range("E9:H9").Merge().Value = "Email";
            worksheet.Range("I9:K9").Merge().Value = "Date";
            worksheet.Range("L9:N9").Merge().Value = "Mobile Number";
            worksheet.Range("O9:P9").Merge().Value = "Total Order";
           
            // **INSERT ORDER DATA**
            int row = 10;
            foreach (var customer in customers)
            {
                worksheet.Cell(row, 1).Value = customer.Customerid;
                worksheet.Range($"B{row}:D{row}").Merge().Value = customer.Customername;
                worksheet.Range($"E{row}:H{row}").Merge().Value = customer.Email;
                worksheet.Range($"I{row}:K{row}").Merge().Value =customer.Orderdate.ToString();
                worksheet.Range($"L{row}:N{row}").Merge().Value = customer.Phoneno;
                worksheet.Range($"O{row}:P{row}").Merge().Value = customer.Totalorder;
                row++;
            }

            // Apply Style to Data
            var dataRange = worksheet.Range($"A10:P{row - 1}");
            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // // Auto-fit Columns
            // worksheet.Columns().AdjustToContents();

            // **INSERT LOGO**
            var imagePath = "wwwroot/images/pizzashop_logo.png";
            if (File.Exists(imagePath))
            {
                var image = worksheet.AddPicture(imagePath)
                    .MoveTo(worksheet.Cell("O2"))
                    .Scale(0.3);
            }

            // Return as Excel file
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }

}
