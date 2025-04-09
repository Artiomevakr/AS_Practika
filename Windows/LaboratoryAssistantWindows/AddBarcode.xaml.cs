using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.IO.Font;
using Paragraph = iText.Layout.Element.Paragraph;
using iText.Kernel.Font;
using iText.IO.Image;
using Aspose.BarCode.Generation;

namespace MedLab.Windows.LaboratoryAssistantWindows
{
    public partial class AddBarcode : Window
    {
        ASEntities As = new ASEntities();
        Order newOrder;
        Service addService;
        public AddBarcode(Order order_, Service services_)
        {
            InitializeComponent();
            newOrder = order_;
            addService = services_;
        }

        private void TextNewBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int number = As.OrderAndServices.ToList().Last().Id + 1;
                TextNewBarcode.Text = number.ToString();
            }
        }

        private string finalBarcode;
        /// <summary>
        /// Генерирует уникальный штрих-код для нового заказа.
        /// </summary>
        /// <returns>
        /// true - если штрих-код успешно создан и сохранён, 
        /// false - если введённый код не является числом или уже занят.
        /// </returns>
        public bool GenerateBarcode()
        {
            if (int.TryParse(TextNewBarcode.Text, out int barcode))
            {
                if (As.OrderAndServices.Any(x => x.Id == barcode))
                {
                    MessageBox.Show("Данной код занят!\nНажмите клавишу Enter для формирования подходящего кода.");
                    return false;
                }
                else
                {
                    Random random = new Random();
                    string rndCode = random.Next(100000, 999999).ToString();
                    string stringBarcode = barcode.ToString() + DateTime.Now.ToString() + rndCode;
                    string barcode1 = stringBarcode.Replace(".", "");
                    string barcode2 = barcode1.Replace(":", "");
                    finalBarcode = barcode2.Replace(" ", "");
                    OrderAndService newOrderAndServices = new OrderAndService()
                    {
                        IdOrder = newOrder.Id,
                        IdStatus = 1,
                        BarcodeCode = finalBarcode,
                        ServiceCode = addService.Code,
                    };
                    As.OrderAndServices.Add(newOrderAndServices);
                    As.SaveChanges();
                    MessageBox.Show("Успешно!");
                    this.Close();
                    return true;
                }
            }
            else
            {
                MessageBox.Show("Код должен быть числом!");
                return false;
            }
        }

        private void SaveBarcode_Click(object sender, RoutedEventArgs e)
        {
            GenerateBarcode();  
        }

        private void SaveBarcodeAndGenBarcode_Click(object sender, RoutedEventArgs e)
        {
            if (GenerateBarcode())
            {
                try
                {
                    string imageType = "jpeg";
                    string imagePath = $"{Environment.CurrentDirectory}\\Barcode.{imageType}";
                    using (var generator = new BarcodeGenerator(EncodeTypes.Code128, finalBarcode))
                    {
                        generator.Parameters.Barcode.XDimension.Pixels = 5;
                        generator.Save(imagePath, BarCodeImageFormat.Jpeg);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.Title = "Сохранить PDF файл";
                saveFileDialog.FileName = "Service.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    using (PdfWriter writer = new PdfWriter(filePath))
                    using (PdfDocument pdf = new PdfDocument(writer))
                    using (Document document = new Document(pdf))
                    {
                        try
                        {
                            PdfFont font = PdfFontFactory.CreateFont("Fonts\\times.ttf", PdfEncodings.IDENTITY_H);
                            ImageData imageData = ImageDataFactory.Create($"{Environment.CurrentDirectory}\\Barcode.jpeg");
                            iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData)
                                .SetWidth(200).SetHeight(100);
                            document.Add(image);
                            document.Close();
                        }
                        catch (Exception ex)
                        {
                            document.Close();
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Сохранение отменено.");
                }
            }
        }
    }
}
