using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public sealed partial class EditBillPage : Page
    {
        public EditBillPage()
        {
            this.InitializeComponent();
            header.Text = $"Nouvelle Facture: Client {App.User.curClient}";
            int tmp = 0;
            int index = 0;
            foreach (Convention current in App.User.curClient.ClientConvs)
            {
                if (App.User.curBill.Conv == 1)
                {
                    if (!current.Equals(App.User.curBill.Convention))
                        tmp++;
                    else
                        index = tmp;
                }
                Box.Items.Add(current.Date);               
            }
            if (App.User.curBill.Conv == 1)
                Box.SelectedIndex = index;
            SetValues();
        }

        private void SetValues()
        {
            TVA.Text = App.User.current.TVA.ToString();
            date.Text = App.User.curBill.Date;
            services.Text = App.User.curBill.Service;
            reference.Text = App.User.curBill.Reference;
            heure.Text = App.User.curBill.Hour.ToString();
            TotalHT.Text = App.User.curBill.THT.ToString();
            TotalTC.Text = App.User.curBill.TTC.ToString();
            foreach (Convention current in App.User.curClient.ClientConvs)
                Box.Items.Add(current.ToString());
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string conv = e.AddedItems[0].ToString();
            foreach (Convention current in App.User.curClient.ClientConvs)
            {
                if (current.ToString().Equals(conv.ToString()))
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
                try { TTC = THT + (THT * (App.User.curConv.Tva) / 100); } catch { }
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
                float THT = float.Parse(tht);
                float TTC = THT + (THT * (tva / 100));
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
                Bill tmp = App.User.curBill;

                App.User.curBill.Date = date.Text;
                App.User.curBill.Service = services.Text;
                App.User.curBill.Reference = reference.Text;
                App.User.curBill.Hour = float.Parse(heure.Text);
                App.User.curBill.TTC = float.Parse(TotalTC.Text);
                App.User.curBill.THT = float.Parse(TotalHT.Text);
                App.User.curBill.Tva = float.Parse(TVA.Text);
                App.User.curBill.ClientID = App.User.curClient.ClientID;

                App.User.curClient.ClientBills.Remove(tmp);
                App.User.curClient.ClientBills.Add(App.User.curBill);

                /*
                try { Database.RemovePayments(tmp.Reference, App.User.curClient.ClientID); } catch { }
                foreach (Payment p in tmp.Payments)
                    Database.AddPayment(p);
                try { Database.RemoveBill(tmp); } catch { }
                try { Database.AddBill(App.User.curBill); } catch { }
                */

                Database.RemoveAzurePayments(tmp.Reference, App.User.curClient.ClientID);
                foreach (Payment p in tmp.Payments)
                    Database.AddAzurePayment(p);
                Database.RemoveAzureBill(tmp);
                Database.AddAzureBill(App.User.curBill);


                Frame.Navigate(typeof(ClientPage));
            }
        }
    }
}
