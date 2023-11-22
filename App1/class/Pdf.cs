using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Popups;

namespace App1
{
    internal class Pdf
    {
        public class CustOrders
        {
            public string MontantHorsTaxe { get; set; }
            public string TaxeSurVA { get; set; }
            public string TauxTVA { get; set; }
            public string Acompte { get; set; }
            public string MontantTTC { get; set; }
            public CustOrders(string montantHorsTaxe, string taxeSurVA, string tauxTVA, string acompte, string montantTTC)
            {
                MontantHorsTaxe = montantHorsTaxe;
                TaxeSurVA = taxeSurVA;
                TauxTVA = tauxTVA;
                Acompte = acompte;
                MontantTTC = montantTTC;
            }
        }
        public class ShipDetails
        {
            public string ShipName { get; set; }
            public string ShipAddress { get; set; }
            public string ShipCity { get; set; }
            public string ShipCountry { get; set; }
            public ShipDetails(string shipName, string shipAddress, string shipCity, string shipCountry)
            {
                ShipName = shipName;
                ShipAddress = shipAddress;
                ShipCity = shipCity;
                ShipCountry = shipCountry;
            }
        }
        internal sealed class Orders
        {
            public static List<CustOrders> GetProduct()
            {
                List<CustOrders> list = new List<CustOrders>();

                float acompte = 0;
                foreach (Payment current in App.User.curBill.Payments)
                {
                    acompte += current.Amount;
                }
                float taxVA = App.User.curBill.THT * (App.User.curBill.Tva / 100);
                list.Add(new CustOrders(App.User.curBill.THT.ToString(), taxVA.ToString(), App.User.curBill.Tva.ToString(), acompte.ToString(), App.User.curBill.TTC.ToString()));

                return list;
            }
            public static ShipDetails GetShipDetails()
            {
                return (new ShipDetails(App.User.curClient.ToString(), App.User.curClient.Address, App.User.curClient.PostalCode + ", " + App.User.curClient.City, ""));
            }
        }
        public async static void Save(Stream stream, string filename, int test)
        {
            stream.Position = 0;

            StorageFile stFile;
            if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.DefaultFileExtension = ".pdf";
                if (test == 1)
                    savePicker.SuggestedFileName = App.User.curClient.ToString() + " - " + App.User.curBill.Reference.ToString();
                else
                    savePicker.SuggestedFileName = App.User.curClient.ToString() + " - " + App.User.curConv.Reference.ToString();
                savePicker.FileTypeChoices.Add("Adobe PDF Document", new List<string>() { ".pdf" });
                stFile = await savePicker.PickSaveFileAsync();
            }
            else
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                await local.CreateFolderAsync("Factures", CreationCollisionOption.OpenIfExists);
                stFile = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            }
            if (stFile != null)
            {
                Windows.Storage.Streams.IRandomAccessStream fileStream = await stFile.OpenAsync(FileAccessMode.ReadWrite);
                Stream st = fileStream.AsStreamForWrite();
                st.SetLength(0);
                st.Write((stream as MemoryStream).ToArray(), 0, (int)stream.Length);
                st.Flush();
                st.Dispose();
                fileStream.Dispose();
                MessageDialog msgDialog = new MessageDialog("Do you want to view the document?", "File created.");
                UICommand yesCMD = new UICommand("yes");
                msgDialog.Commands.Add(yesCMD);
                UICommand noCMD = new UICommand("no");
                msgDialog.Commands.Add(noCMD);
                IUICommand cmd = await msgDialog.ShowAsync();
                if (cmd == yesCMD)
                {
                    // Launch the retrieved file
                    bool success = await Windows.System.Launcher.LaunchFileAsync(stFile);
                }
            }

        }

        public static void Create_Bill_Pdf()
        {
            PdfDocument document = new PdfDocument();

            document.PageSettings.Orientation = PdfPageOrientation.Portrait;
            document.PageSettings.Margins.All = 10;

            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;

            // ENTETE DE LA FACTURE
            PdfTextElement element = new PdfTextElement(App.User.current.BillBegin);
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(new PdfColor(89, 89, 93));
            PdfLayoutResult result = element.Draw(page, new RectangleF(0, 0, page.Graphics.ClientSize.Width, 200));

            PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);
            graphics.DrawRectangle(new PdfSolidBrush(new PdfColor(126, 151, 173)), new RectangleF(0, result.Bounds.Bottom + 20, graphics.ClientSize.Width, 30));

            element = new PdfTextElement("FACTURE", subHeadingFont);
            element.Brush = PdfBrushes.White;
            result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 24));

            // INSERER LA DATE DE LA FACTURE
            string currentDate = "DATE " + App.User.curBill.Date;
            SizeF textSize = subHeadingFont.MeasureString(currentDate);
            graphics.DrawString(currentDate, subHeadingFont, element.Brush, new PointF(graphics.ClientSize.Width - textSize.Width - 10, result.Bounds.Y));

            // INSERER LA REFERENCE CLIENT ET REFERENCE FACTURE
            element = new PdfTextElement("Reference cabinet: " + App.User.curClient.Reference + "\nFacture: " + App.User.curBill.Reference);
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(new PdfColor(89, 89, 93));
            result = element.Draw(page, new RectangleF(0, 125, page.Graphics.ClientSize.Width / 2, 200));

            element = new PdfTextElement("Facture pour: ", new PdfStandardFont(PdfFontFamily.TimesRoman, 10));
            element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
            result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 25));
            graphics.DrawLine(new PdfPen(new PdfColor(126, 151, 173), 0.70f), new PointF(0, result.Bounds.Bottom + 3), new PointF(graphics.ClientSize.Width, result.Bounds.Bottom + 3));

            List<CustOrders> list = Orders.GetProduct();

            var reducedList = list.Select(f => new { f.MontantHorsTaxe, f.TaxeSurVA, f.TauxTVA, f.Acompte, f.MontantTTC }).ToList();

            ShipDetails shipDetails = Orders.GetShipDetails();
            element = new PdfTextElement(shipDetails.ShipName, new PdfStandardFont(PdfFontFamily.TimesRoman, 10));
            element.Brush = new PdfSolidBrush(new PdfColor(89, 89, 93));
            result = element.Draw(page, new RectangleF(10, result.Bounds.Bottom + 5, graphics.ClientSize.Width / 2, 100));

            //Create a text element and draw it to PDF page.
            element = new PdfTextElement(string.Format("{0}, {1}", shipDetails.ShipAddress, shipDetails.ShipCity), new PdfStandardFont(PdfFontFamily.TimesRoman, 10));
            element.Brush = new PdfSolidBrush(new PdfColor(89, 89, 93));
            result = element.Draw(page, new RectangleF(10, result.Bounds.Bottom + 3, graphics.ClientSize.Width / 2, 100));

            element = new PdfTextElement("Prestations: ", new PdfStandardFont(PdfFontFamily.TimesRoman, 10));
            element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
            result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 25));
            graphics.DrawLine(new PdfPen(new PdfColor(126, 151, 173), 0.70f), new PointF(0, result.Bounds.Bottom + 3), new PointF(graphics.ClientSize.Width, result.Bounds.Bottom + 3));


            // INSERER SERVICES
            element = new PdfTextElement(App.User.curBill.Service, new PdfStandardFont(PdfFontFamily.TimesRoman, 10));
            element.Brush = new PdfSolidBrush(new PdfColor(89, 89, 93));
            result = element.Draw(page, new RectangleF(10, result.Bounds.Bottom + 5, graphics.ClientSize.Width / 2, 100));

            PdfGrid grid = new PdfGrid();
            grid.DataSource = reducedList;

            //Initialize PdfGridCellStyle and set border color.
            PdfGridCellStyle cellStyle = new PdfGridCellStyle();
            cellStyle.Borders.All = PdfPens.White;
            cellStyle.Borders.Bottom = new PdfPen(new PdfColor(217, 217, 217), 0.70f);
            cellStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12f);
            cellStyle.TextBrush = new PdfSolidBrush(new PdfColor(131, 130, 136));

            //Initialize PdfGridCellStyle and set header style.
            PdfGridCellStyle headerStyle = new PdfGridCellStyle();
            headerStyle.Borders.All = new PdfPen(new PdfColor(126, 151, 173));
            headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
            headerStyle.TextBrush = PdfBrushes.White;
            headerStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14f, PdfFontStyle.Regular);

            PdfGridRow header = grid.Headers[0];
            for (int i = 0; i < header.Cells.Count; i++)
            {
                header.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            }
            header.ApplyStyle(headerStyle);

            foreach (PdfGridRow row in grid.Rows)
            {
                row.ApplyStyle(cellStyle);
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    //Create and customize the string formats
                    PdfGridCell cell = row.Cells[i];
                    float val = float.MinValue;
                    float.TryParse(cell.Value.ToString(), out val);
                    if (i == 2)
                        cell.Value = "%" + val.ToString("0.00");
                    else
                        cell.Value = val.ToString("0.00") + " euros";
                    cell.StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
                }
            }


            //Set properties to paginate the grid.
            PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
            layoutFormat.Layout = PdfLayoutType.Paginate;

            //Draw grid to the page of PDF document.
            PdfGridLayoutResult gridResult = grid.Draw(page, new RectangleF(new PointF(0, result.Bounds.Bottom + 40), new SizeF(graphics.ClientSize.Width, graphics.ClientSize.Height - 100)), layoutFormat);
            float pos = 0.0f;
            for (int i = 0; i < grid.Columns.Count - 1; i++)
                pos += grid.Columns[i].Width;

            PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14f);

            float acompte = 0;
            foreach (Payment current in App.User.curBill.Payments)
            {
                acompte += current.Amount;
            }
            float total = App.User.curBill.TTC - acompte;

            gridResult.Page.Graphics.DrawString("Total Due", font, new PdfSolidBrush(new PdfColor(126, 151, 173)), new RectangleF(new PointF(pos, gridResult.Bounds.Bottom + 20), new SizeF(grid.Columns[3].Width - pos, 20)), new PdfStringFormat(PdfTextAlignment.Right));
            pos += grid.Columns[4].Width;
            gridResult.Page.Graphics.DrawString(string.Format("{0:N2}", total + " euros"), font, new PdfSolidBrush(new PdfColor(131, 130, 136)), new RectangleF(new PointF(pos, gridResult.Bounds.Bottom + 20), new SizeF(grid.Columns[4].Width - pos, 20)), new PdfStringFormat(PdfTextAlignment.Right));

            element = new PdfTextElement("Signature et cachet:", new PdfStandardFont(PdfFontFamily.TimesRoman, 10));
            element.Brush = new PdfSolidBrush(new PdfColor(89, 89, 93));
            result = element.Draw(page, new RectangleF(10, result.Bounds.Bottom + 200, graphics.ClientSize.Width / 2, 100));

            // INSERER LE BAS DE PAGE
            element = new PdfTextElement(App.User.current.BillEnd);
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(new PdfColor(89, 89, 93));
            result = element.Draw(page, new RectangleF(10, result.Bounds.Bottom + 275, graphics.ClientSize.Width, 100));

            //Save the PDF document to stream.
            MemoryStream stream = new MemoryStream();
            document.Save(stream);

            //Close the document.
            document.Close(true);

            //Save the stream as PDF document file in local machine. Refer to PDF/UWP section for respected code samples.
            Save(stream, App.User.curClient.ToString() + " - " + App.User.curBill.Reference.ToString(), 1);
        }

        private static void drawRect(PdfGraphics graphics, float x, float y, float w, float h)
        {
            graphics.DrawLine(new PdfPen(Color.Black), new PointF(x, y), new PointF(x + w, y));
            graphics.DrawLine(new PdfPen(Color.Black), new PointF(x, y), new PointF(x, y + h));
            graphics.DrawLine(new PdfPen(Color.Black), new PointF(x + w, y), new PointF(x + w, y + h));
            graphics.DrawLine(new PdfPen(Color.Black), new PointF(x, y + h), new PointF(x + w, y + h));
        }

        public static void Create_Conv_Pdf()
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Orientation = PdfPageOrientation.Portrait;
            document.PageSettings.Margins.All = 20;

            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;

            PdfTextElement element = new PdfTextElement("Convention d'honoraires");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            PdfLayoutResult result = element.Draw(page, new RectangleF(page.Graphics.ClientSize.Width / 3 + 45, 0, page.Graphics.ClientSize.Width, 200));

            element = new PdfTextElement("en trois exemplaires : 1er : dossier - 2e : justiciable et 3e : dossier suivi conventions");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 9);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(125, 12, page.Graphics.ClientSize.Width, 200));

            ////////////////////// 1er CADRE ////////////////////////////////////////////////

            // 1 - 1bis
            drawRect(graphics, 0, result.Bounds.Bottom + 20, graphics.ClientSize.Width / 2 + 50, 40);
            drawRect(graphics, graphics.ClientSize.Width / 2 + 50, result.Bounds.Bottom + 20, graphics.ClientSize.Width / 2 - 50, 40);

            element = new PdfTextElement("Entre Cabinet Bords de SEINE\nMe Pierre BORDESSOULE de BELLEFEUILLE\nBarreau de Versailles - C392 - 32 rue V.HUGO 78420 Carrieres/Seine");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 22, graphics.ClientSize.Width / 2 + 50, 200));

            element = new PdfTextElement("- et Mme / M. / Ste");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(graphics.ClientSize.Width / 2 + 55, result.Bounds.Bottom + 22, graphics.ClientSize.Width / 2 - 50, 200));


            // 2 3 4
            drawRect(graphics, 0, result.Bounds.Bottom + 20 + 40, graphics.ClientSize.Width, 25);
            element = new PdfTextElement("- nature de l'affaire :\n- Dossier");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 62, graphics.ClientSize.Width, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 20 + 65, graphics.ClientSize.Width, 25);
            element = new PdfTextElement("- tarif forfaitaire convenu              euros HT              soit              euros TTC :\nsur la base d'un tarif horaire de " + App.User.curConv.Fees + " euros HT, soit un total de     heures travaillees.");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 87, graphics.ClientSize.Width, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 20 + 90, graphics.ClientSize.Width, 16);
            element = new PdfTextElement("References cabinet : " + App.User.curClient.Reference);
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 112, graphics.ClientSize.Width, 200));

            //////////////////// 2e CADRE ///////////////////////////////////////////////////

            // 1
            drawRect(graphics, 0, result.Bounds.Bottom + 136, graphics.ClientSize.Width, 16);
            element = new PdfTextElement("A - Au dela il sera facture");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 138, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("en cours/fin de procedure/a demande :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(120, result.Bounds.Bottom + 138, graphics.ClientSize.Width, 200));

            // 2 3 4 - 4bis
            drawRect(graphics, 0, result.Bounds.Bottom + 136 + 16, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("rendez-vous justiciable");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 138 + 16, graphics.ClientSize.Width, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 136 + 30, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("examen piece/recherche JP/definition strategie");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 138 + 30, graphics.ClientSize.Width, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 136 + 44, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("redaction assignation / requete ou des conclusions initiales");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 138 + 44, graphics.ClientSize.Width, 200));

            drawRect(graphics, graphics.ClientSize.Width / 2 + 50, result.Bounds.Bottom + 136 + 16, graphics.ClientSize.Width / 2 - 50, 42);
            element = new PdfTextElement("tarif horaire 230,00 euros / heure");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(graphics.ClientSize.Width / 2 + 115, result.Bounds.Bottom + 138 + 30, graphics.ClientSize.Width, 200));

            // 5 - 5bis
            drawRect(graphics, 0, result.Bounds.Bottom + 180 + 14, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("pages de correspondances postales");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 182 + 14, graphics.ClientSize.Width, 200));

            drawRect(graphics, graphics.ClientSize.Width / 2 + 50, result.Bounds.Bottom + 180 + 14, graphics.ClientSize.Width / 2 - 50, 14);
            element = new PdfTextElement("50,00 euros HT");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(graphics.ClientSize.Width / 2 + 55, result.Bounds.Bottom + 182 + 14, graphics.ClientSize.Width, 200));

            // 6 - 6bis
            drawRect(graphics, 0, result.Bounds.Bottom + 194 + 14, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("pages dactylographiees hors correspondances courriels/post et ecritures procedurales.");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 196 + 14, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, graphics.ClientSize.Width / 2 + 50, result.Bounds.Bottom + 194 + 14, graphics.ClientSize.Width / 2 - 50, 14);
            element = new PdfTextElement("10,00 euros HT");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(graphics.ClientSize.Width / 2 + 55, result.Bounds.Bottom + 194 + 14, graphics.ClientSize.Width, 200));

            // 7 - 7bis
            drawRect(graphics, 0, result.Bounds.Bottom + 208 + 14, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("courriels (equivalent lettre postale)");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 210 + 14, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, graphics.ClientSize.Width / 2 + 50, result.Bounds.Bottom + 208 + 14, graphics.ClientSize.Width / 2 - 50, 14);
            element = new PdfTextElement("50,00 euros HT");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(graphics.ClientSize.Width / 2 + 55, result.Bounds.Bottom + 210 + 14, graphics.ClientSize.Width, 200));

            // 8 - 8bis
            drawRect(graphics, 0, result.Bounds.Bottom + 236, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("photocopies et/ou scans");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 238, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, graphics.ClientSize.Width / 2 + 50, result.Bounds.Bottom + 236, graphics.ClientSize.Width / 2 - 50, 14);
            element = new PdfTextElement("1,00 euros HT piece");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(graphics.ClientSize.Width / 2 + 55, result.Bounds.Bottom + 238, graphics.ClientSize.Width, 200));

            // 9 10 11 12 13 14 15 16 - 9bis
            drawRect(graphics, 0, result.Bounds.Bottom + 250, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("appels telephoniques : nombre (par unite de 5 mn)");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 252, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 250 + 14, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("formalites greffe, placement, demarches Palais");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 252 + 14, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 250 + 28, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("contact deontologique avec l'avocat adverse");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 252 + 28, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 250 + 42, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("lecture et analyse des pieces adverses et transmission");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 252 + 42, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 250 + 56, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("lecture et analyse des conclusions adverses et transmission");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 252 + 56, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 250 + 70, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("audience de plaidoiries dont confection dossier de plaidoiries");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 252 + 70, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 250 + 84, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("formalite diverse");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 252 + 84, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 250 + 98, graphics.ClientSize.Width / 2 + 50, 14);
            element = new PdfTextElement("lecture et analyse de la decision obtenue (mais non l'execution)");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 252 + 98, graphics.ClientSize.Width / 2 + 50, 200));

            drawRect(graphics, graphics.ClientSize.Width / 2 + 50, result.Bounds.Bottom + 250, graphics.ClientSize.Width / 2 - 50, 112);
            element = new PdfTextElement("tarif horaire 230,00 euros / heure");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(graphics.ClientSize.Width / 2 + 115, result.Bounds.Bottom + 252 + 46, graphics.ClientSize.Width, 200));

            /////////////////////////////////////////////////////////////////////////////////
            ///
            element = new PdfTextElement("B - ce tarif ne couvre pas les items suivant,");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 370, graphics.ClientSize.Width, 200));

            element = new PdfTextElement(" exigible des realisation :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(188, result.Bounds.Bottom - 11, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("- frais specifique : frais au reel : ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom, graphics.ClientSize.Width / 2, 200));
            element = new PdfTextElement("LRAR, deplacements (frais kilometriques : base fiscale), frais de sejour (train, taxis, parking, hotel, repas), debours (frais de greffe, droit de plaidoiries et de timbre...), coursier, traductions, recherches et obtention de documentation Internet payantes ...");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 12, graphics.ClientSize.Width - 20, 200));

            element = new PdfTextElement("- honoraires de resultat : ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 45, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("10% HT des sommes brutes obtenues/economisees, judicierement ou amiablement.");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(115, result.Bounds.Bottom + 46, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("C - Modes de reglement");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 70, graphics.ClientSize.Width / 2, 200));

            element = new PdfTextElement("(art. L441-3 a L441-6, D. 441-5 Code Com., loi n2012-387 du 22.03.12, directive 2011/7/UE du 16.02.11)");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8, PdfFontStyle.Italic);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(result.Bounds.Right - 170, result.Bounds.Bottom - 10, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("- date de reglement :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom, graphics.ClientSize.Width, 200));
            element = new PdfTextElement("a emission de la facture - sauf facture acquittee et accord d'echelonnement");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(result.Bounds.Right - 170, result.Bounds.Bottom, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("- mode de paiement :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 12, graphics.ClientSize.Width, 200));
            element = new PdfTextElement("cheque (liquidites : un recu sera donne) - virement bancaire : RIB fourni");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(result.Bounds.Right - 170, result.Bounds.Bottom + 12, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("- escompte :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 24, graphics.ClientSize.Width, 200));
            element = new PdfTextElement("aucun");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(result.Bounds.Right - 170, result.Bounds.Bottom + 24, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("- professionnels :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 36, graphics.ClientSize.Width, 200));
            element = new PdfTextElement("indemnite forfaitaire pour recouvrement: 40,00$ (outre indemnisation complementaire sur justificatifs et penalites de retard)");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(result.Bounds.Right - 170, result.Bounds.Bottom + 36, graphics.ClientSize.Width - 140, 200));

            element = new PdfTextElement("Fait le              a");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 74, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("Signatures :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 88, graphics.ClientSize.Width, 200));

            drawRect(graphics, 0, result.Bounds.Bottom + 20, graphics.ClientSize.Width / 2 + 50, 40);
            drawRect(graphics, graphics.ClientSize.Width / 2 + 50, result.Bounds.Bottom + 20, graphics.ClientSize.Width / 2 - 50, 40);


            page = document.Pages.Add();

            graphics = page.Graphics;

            // INSERER LE BAS DE PAGE
            element = new PdfTextElement(App.User.current.ConvRules);
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, 10, graphics.ClientSize.Width - 20, 1500));

            graphics.DrawLine(new PdfPen(Color.Black), new PointF(5, result.Bounds.Bottom + 8), new PointF(graphics.ClientSize.Width - 5, result.Bounds.Bottom + 8));

            element = new PdfTextElement("Droit de retractation");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width - 20, 200));
            element = new PdfTextElement("Le client consommateur beneficie d'un droit de retractation d'une duree de quatorze jours courant a compter de la date de signature de la presente convention et ne doit realiser aucun paiement avant l'expiration d'un delai de sept jours suivant la date de signature de la presente convention. Pour exercer ledit droit de retractation, le client doit notifier a l'Avocat, par lettre recommandee avec accuse de receotion expediee avant l'expiration du delai ci-dessus indique (la date de la Poste faisant foi), le formulaire de retractation joint a la presente convention apres l'avoir rempli et signe. Si le client prefere que l'Avocat debnute immediatement sa mission, il peut lui retourner le formulaire de renonciation a retractation joint a la presente convention (il peut, en pareille hypothese, le lui retourner par lettre simple).");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width - 20, 200));

            graphics.DrawLine(new PdfPen(Color.Black), new PointF(5, result.Bounds.Bottom + 8), new PointF(graphics.ClientSize.Width - 5, result.Bounds.Bottom + 8));

            element = new PdfTextElement("CHOIX 1 : FORMULAIRE DE RETRACTATION - ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width, 200));
            element = new PdfTextElement("Je, soussigne(e) : prenom et nom: ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width, 200));
            element = new PdfTextElement("vous notifie par la presente ma volonte de me retracter de la convention d'honoraire signee le");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("Fait a : ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width / 2, 200));
            element = new PdfTextElement("Le : ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5 + graphics.ClientSize.Width / 2, result.Bounds.Bottom + 16, graphics.ClientSize.Width / 2, 200));

            element = new PdfTextElement("Signature :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 16, graphics.ClientSize.Width, 200));
            element = new PdfTextElement("ce formulaire doit etre notifie par le client consommateur a l'Avocat par lettre recommandee avec accuse de reception, le tout avant l'expiration du delai legal de retractation (14 jours a compter de la signature de la presente convention).");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 6, PdfFontStyle.Italic);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width, 200));

            graphics.DrawLine(new PdfPen(Color.Black), new PointF(5, result.Bounds.Bottom + 16), new PointF(graphics.ClientSize.Width - 5, result.Bounds.Bottom + 16));

            element = new PdfTextElement("CHOIX 2 : FORMULAIRE DE RENONCIATION A RETRACTATION -");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 10, PdfFontStyle.Bold);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 16, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("Je, soussigne(e) : prenom et nom: ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width, 200));
            element = new PdfTextElement("vous notifie par la presente ma renonciation expresse a mon droit de retractation d'une duree de 14 jours apres la signature de la presente convention d'honoraire et vous demande l'execution immediate de la mission qui vous a ete confiee.");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width, 200));

            element = new PdfTextElement("Fait a : ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 8, graphics.ClientSize.Width / 2, 200));
            element = new PdfTextElement("Le : ");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5 + graphics.ClientSize.Width / 2, result.Bounds.Bottom + 16, graphics.ClientSize.Width / 2, 200));

            element = new PdfTextElement("Signature :");
            element.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 8);
            element.Brush = new PdfSolidBrush(Color.Black);
            result = element.Draw(page, new RectangleF(5, result.Bounds.Bottom + 16, graphics.ClientSize.Width, 200));


            // Save the Pdf document to stream
            MemoryStream stream = new MemoryStream();
            document.Save(stream);

            document.Close(true);
            Save(stream, App.User.curClient.ToString() + " - " + App.User.curConv.Reference.ToString(), 2);
        }
    }
}

