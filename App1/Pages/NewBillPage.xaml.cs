using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace App1.Pages
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class NewBillPage : Page
    {
        public NewBillPage()
        {
            this.InitializeComponent();
            TVA.Text = App.User.current.TVA.ToString();
            App.User.curBill = null;
            App.User.curBill = new Bill();
            App.User.curConv = null;
            App.User.curConv = new Convention();
            App.User.curBill.Conv = 0;
            header.Text = $"Nouvelle Facture: Client {App.User.curClient}";
            foreach (Convention current in App.User.curClient.ClientConvs)
                Box.Items.Add(current.Date + " - ref. " + current.Reference);

            date.Text = DateTime.Now.ToString("dd/MM/yyyy");
            reference.Text = DateTime.Now.ToString("yyyy-MM") + "-" + (App.User.current.BillNum + 1).ToString();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string conv = e.AddedItems[0].ToString();
            foreach (Convention current in App.User.curClient.ClientConvs)
            {
                if (conv.Equals(current.Date + " - ref. " + current.Reference))
                {
                    App.User.curBill.Conv = 1;
                    App.User.curBill.Convention = current;
                    App.User.curConv = current;
                    TVA.Text = current.Tva.ToString();
                }
            }
        }
        private void HourChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            string hour = heure.Text;
            if (App.User.curBill.Conv == 1 && App.User.curBill.Convention != null)
            {
                float THT = 0;
                try { THT = float.Parse(hour) * (App.User.curConv.Fees); } catch { }
                float TTC = 0;
                try { TTC = THT + (THT * (App.User.curConv.Tva / 100)); } catch { }
                TotalHT.Text = THT.ToString();
                TotalTC.Text = TTC.ToString();
                TVA.Text = App.User.curConv.Tva.ToString();
            }
        }

        private void THTChanged(object sender, TextBoxTextChangingEventArgs e)
        {
            string tht = TotalHT.Text;
            float tva = 0;
            try { tva = float.Parse(TVA.Text); }
            catch { tva = 0; }
            if (App.User.curBill.Conv != 1)
            {
                float THT = 0;
                float TTC = 0;
                try { THT = float.Parse(tht); } catch { }
                try { TTC = THT + (THT * (tva / 100)); } catch { }
                TotalTC.Text = TTC.ToString();
            }
        }

        private bool isBillOk()
        {
            bool isTotalTcValid = float.TryParse(TotalTC.Text, out float totalTcValue);
            bool isTotalHtValid = float.TryParse(TotalHT.Text, out float totalHtValue);
            bool isTvaValid = float.TryParse(TVA.Text, out float tvaValue);
            bool isDateValid = !string.IsNullOrEmpty(date.Text);
            bool isServicesValid = !string.IsNullOrEmpty(services.Text);
            bool isReferenceValid = !string.IsNullOrEmpty(reference.Text);

            TotalTC.BorderBrush = isTotalTcValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            TotalHT.BorderBrush = isTotalHtValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            TVA.BorderBrush = isTvaValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            date.BorderBrush = isDateValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            services.BorderBrush = isServicesValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            reference.BorderBrush = isReferenceValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);

            return isTotalTcValid && isTotalHtValid && isTvaValid && isDateValid && isServicesValid && isReferenceValid;
        }



        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ClientPage));
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (isBillOk())
            {
                App.User.curBill.Date = date.Text;
                App.User.curBill.Service = services.Text;
                App.User.curBill.Reference = reference.Text;
                App.User.curBill.Hour = float.Parse(heure.Text);
                App.User.curBill.TTC = float.Parse(TotalTC.Text);
                App.User.curBill.THT = float.Parse(TotalHT.Text);
                App.User.curBill.Tva = float.Parse(TVA.Text);
                App.User.curBill.ClientID = App.User.curClient.ClientID;

                
                App.User.curClient.ClientBills.Add(App.User.curBill);

                //try { Database.AddBill(App.User.curBill); } catch { }
                Database.AddAzureBill(App.User.curBill);
                App.User.current.BillNum++;

                Database.RemoveAzureUser();
                Database.CreateAzureUser();
                Database.AddAzureUser(App.User.current);

                Frame.Navigate(typeof(ClientPage));
            }
        }
    }
}
