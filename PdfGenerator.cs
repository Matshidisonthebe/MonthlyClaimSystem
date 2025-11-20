using System;
using System.Collections.Generic;
using System.Linq;
using MonthlyClaimSystem.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MonthlyClaimSystem.Helpers
{
    public static class PdfGenerator
    {
        public static byte[] GenerateInvoicePdf(Claim claim)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Invoice for Claim #{claim.claimId}").FontSize(20).Bold();
                        col.Item().Text($"Generated: {DateTime.UtcNow:G}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        
                        col.Spacing(10);

                        col.Item().Row(row =>
                        {
                            row.RelativeColumn().Column(left =>
                            {
                                left.Item().Text("Lecturer").SemiBold();
                                left.Item().Text(claim.LecturerName);                              
                                left.Item().PaddingTop(6).Text("Submitted By").SemiBold(); 
                                left.Item().Text(claim.SubmittedBy);
                            });

                            row.ConstantColumn(200).Column(right =>
                            {
                                right.Item().Text("Claim Details").SemiBold();
                                right.Item().Text($"Date: {claim.claimDate.ToLocalTime():yyyy-MM-dd}");
                                right.Item().Text($"Status: {claim.Status}");
                            });
                        });

                        col.Spacing(10);
                        col.Item().Text("Description").SemiBold();
                        col.Item().Text(string.IsNullOrEmpty(claim.claimDescription) ? "-" : claim.claimDescription).FontSize(11);

                        col.Spacing(10);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Item");
                                header.Cell().Element(CellStyle).Text("Hours / Rate");
                                header.Cell().Element(CellStyle).Text("Amount");
                            });

                            table.Cell().Element(CellStyle).Text(claim.claimType);
                            table.Cell().Element(CellStyle).Text($"{claim.HoursWorked} hrs @ {claim.HourlyRate:C}");
                            table.Cell().Element(CellStyle).Text($"{claim.TotalAmount:C}");
                        });

                        col.Spacing(10);
                        col.Item().AlignRight().Text($"Total: {claim.TotalAmount:C}").FontSize(14).Bold();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public static byte[] GenerateClaimsReportPdf(IEnumerable<Claim> claims)
        {
            var list = claims.OrderBy(c => c.claimId).ToList();
            var total = list.Sum(c => c.TotalAmount);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(11));
                    page.Content().Column(col =>
                    {
                        col.Item().Text("Claims Report").FontSize(18).Bold();
                        col.Item().Text($"Generated: {DateTime.UtcNow:G}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        col.Spacing(8);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40); // id
                                columns.RelativeColumn(); // lecturer
                                columns.ConstantColumn(80); // date
                                columns.ConstantColumn(80); // total
                                columns.RelativeColumn(); // status
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("ID");
                                header.Cell().Element(CellStyle).Text("Lecturer");
                                header.Cell().Element(CellStyle).Text("Date");
                                header.Cell().Element(CellStyle).Text("Total");
                                header.Cell().Element(CellStyle).Text("Status");
                            });

                            foreach (var c in list)
                            {
                                table.Cell().Element(CellStyle).Text(c.claimId.ToString());
                                table.Cell().Element(CellStyle).Text(c.LecturerName);
                                table.Cell().Element(CellStyle).Text(c.claimDate.ToLocalTime().ToString("yyyy-MM-dd"));
                                table.Cell().Element(CellStyle).Text(c.TotalAmount.ToString("C"));
                                table.Cell().Element(CellStyle).Text(c.Status);
                            }
                        });

                        col.Spacing(10);
                        col.Item().AlignRight().Text($"Grand Total: {total:C}").FontSize(13).Bold();
                    });
                });
            });

            return document.GeneratePdf();
        }

        // small helper for table cell padding
        private static IContainer CellStyle(IContainer container)
        {
            return container.PaddingVertical(4).PaddingHorizontal(6);
        }
    }
}