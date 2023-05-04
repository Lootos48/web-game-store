using GameStore.BLL.Util.Interfaces;
using GameStore.DomainModels.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;

namespace GameStore.BLL.Util
{
    public class BankInvoicePdfGenerator : IInvoicePdfGenerator
    {
        public byte[] Generate(Invoice invoice)
        {
            PdfDocument invoicePdf = new PdfDocument();

            PdfPage page = invoicePdf.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

            gfx.DrawString(invoice.TotalPrice.ToString(), font, XBrushes.Black,
                           new XRect(0, 0, page.Width, page.Height),
                           XStringFormats.TopCenter);

            using MemoryStream stream = new MemoryStream();

            invoicePdf.Save(stream);

            return stream.ToArray();
        }
    }
}
