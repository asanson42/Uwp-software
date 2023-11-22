using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class FullBillPage : Page
    {
        public FullBillPage()
        {
            this.InitializeComponent();
            initBills();
        }

        public class billData
        {
            public string Date { get; set; }
            public string Client { get; set; }
            public string Montant { get; set; }
            public string Impayé { get; set; }
            public string Status { get; set; }

            public billData(
                string date, string client, string montant, string impayé, string status)
            {
                Date = date;
                Client = client;
                Montant = montant;
                Impayé = impayé;
                Status = status;
            }
        }

        private void initBills()
        {
            List<billData> display = new List<billData>();
            List<Bill> list = new List<Bill>();
            ClientName client = new ClientName();

            float ftotal = 0;
            float fpaid = 0;
            //try { list = Database.GetAllBills(); } catch { }
            list = Database.GetAzureBills();
            
            foreach (Bill current in list)
            {
                //try { client = Database.GetClientByIndex(current.ClientID); } catch { }
                try { client = Database.GetAzureClientByIndex(current.ClientID); } catch { client = new ClientName(); }
                try { current.Payments = Database.GetAzurePayments(current.Reference, current.ClientID); } catch { current.Payments = new List<Payment>(); }

                float total = current.TTC;
                ftotal += total;
                float rest = current.THT;
                foreach (Payment p in current.Payments)
                {
                    rest -= p.Amount;
                    fpaid += p.Amount;
                }
                if (rest > 0)
                {
                    if (rest >= total)
                        display.Add(new billData(current.Date, client.ToString(), total.ToString(), rest.ToString(), "❌"));
                    else
                        display.Add(new billData(current.Date, client.ToString(), total.ToString(), rest.ToString(), "En cours..."));
                }
            }

            dataGridFullRest.ItemsSource = display;
            float frest = ftotal - fpaid;
            restBlock.Text = " " + frest.ToString() + "€";
        }

        private bool isEqualBill()
        {
            if (dataGridFullRest.SelectedItem != null)
            {
                List<billData> list = dataGridFullRest.ItemsSource as List<billData>;
                int i = dataGridFullRest.SelectedIndex;

                billData it = list[i];
                foreach (ClientName current in App.User.ClientList)
                {
                    if (it.Client.Equals(current.ToString()))
                    {
                        //try { App.User.curClient = Database.GetUniqClient(current.nom, current.prenom); } catch { }
                        try { App.User.curClient = Database.GetAzureClient(current.nom, current.prenom); } catch { App.User.curClient = new Client(); }
                    }
                }
                //try { App.User.curClient.ClientBills = Database.GetClientBills(App.User.curClient.ClientID); } catch { }
                try { App.User.curClient.ClientBills = Database.GetAzureClientBills(App.User.curClient.ClientID); } catch { App.User.curClient.ClientBills = new List<Bill>(); }
                
                foreach (Bill current in App.User.curClient.ClientBills)
                {
                    if (it.Date.Equals(current.Date) && it.Montant.Equals(current.TTC))
                        App.User.curBill = current;
                }

                return true;
            }
            return false;
        }
        private void newPay_Click(object sender, RoutedEventArgs e)
        {
            if (isEqualBill())
                Frame.Navigate(typeof(FullBillNewPaymentPage));
        }

        private void exportBill_Click(object sender, RoutedEventArgs e)
        {
            if (isEqualBill())
                Pdf.Create_Bill_Pdf();
        }
    }
}
