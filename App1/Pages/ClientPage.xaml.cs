using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    public sealed partial class ClientPage : Page
    {
        public ClientPage()
        {
            this.InitializeComponent();
            Header.Text = $"Client {App.User.curClient.ToString()}";

            App.User.curClient.ClientBills = Database.GetAzureClientBills(App.User.curClient.ClientID); 
            App.User.curClient.ClientConvs = Database.GetAzureClientConvs(App.User.curClient.ClientID);
            foreach (Bill current in App.User.curClient.ClientBills)
            {
                if (current.Conv == 1)
                    foreach (Convention conv in App.User.curClient.ClientConvs)
                        if (conv.Reference.Equals(current.convRef))
                            current.Convention = conv;
                try { current.Payments = Database.GetAzurePayments(current.Reference, App.User.curClient.ClientID); } catch { current.Payments = new List<Payment>(); }
            }
            
            setupPage();
        }

        public class conventionData
        {
            public string Date { get; set; }
            public string Total { get; set; }
            public string Facturé { get; set; }
            public string Impayé { get; set; }
            public string Signature { get; set; }

            private string _reference;

            public conventionData(
                string date, string total, string totalBill, string reste, int sign, string reference)
            {
                this.Date = date;
                this.Total = total;
                this.Facturé = totalBill;
                this.Impayé = reste;
                if (sign == 1)
                    this.Signature = "✅";
                else
                    this.Signature = "";
                this._reference = reference;
            }

            public string getRef()
            {
                return this._reference;
            }

        }

        public class billData
        {
            public string Date { get; set; }
            public string Total { get; set; }
            public string Impayé { get; set; }
            public string Status { get; set; }

            private string _reference;

            public billData(
                string date, string total, string reste, string state, string reference)
            {
                this.Date = date;
                this.Total = total;
                this.Impayé = reste;
                this.Status = state;
                _reference = reference;
            }

            public string getRef()
            {
                return this._reference;
            }
        }

        public class paymentData
        {
            public string Date { get; set; }
            public string Montant { get; set; }

            public paymentData(
                string date, string montant)
            {
                this.Date = date;
                this.Montant = montant;
            }
        }

        private void clearData()
        {
            dataGridConventions = null;
            dataGridFactures = null;
            dataGridPayments = null;
        }

        private void GetConvs()
        {
            List<conventionData> list = new List<conventionData>();
            list.Clear();
            foreach(Convention current in App.User.curClient.ClientConvs)
            {
                float totalBill = 0;
                float total = 0;
                try { total = current.Amount + current.Amount * (current.Tva / 100); } catch { }
                float paid = 0;
                float rest = 0;
                foreach(Bill cur in App.User.curClient.ClientBills)
                {
                    if (cur.Conv == 1)
                    {
                        if (cur.Convention.Reference.Equals(current.Reference))
                        {
                            foreach (Payment p in cur.Payments)
                                try { paid += p.Amount; } catch { }
                            try { rest = cur.TTC - paid; } catch { }
                            try { totalBill = cur.TTC; } catch { }
                            break;
                        }
                    }
                }
                list.Add(new conventionData(current.Date.ToString(), total.ToString(), totalBill.ToString(), rest.ToString(), current.sign, current.Reference));
            }
            //dataGridConventions.ItemsSource = null;
            if (list != null)
                dataGridConventions.ItemsSource = list;
        }

        private void GetBills()
        {
            List<billData> list = new List<billData>();
            list.Clear();

            foreach(Bill current in App.User.curClient.ClientBills)
            {
                string state;
                float total = 0;
                try { total = current.TTC; } catch {}
                float rest = 0;
                try { rest = current.TTC; } catch {}
                foreach (Payment p in current.Payments)
                    try { rest -= p.Amount; } catch { }
                if (rest >= total)
                    state = "❌";
                else if (rest > 0 && rest < total)
                    state = "En cours...";
                else
                    state = "✅";

                list.Add(new billData(current.Date, total.ToString(), rest.ToString(), state, current.Reference));
            }

            //dataGridFactures.ItemsSource = null;
            if (list != null)
                dataGridFactures.ItemsSource = list;
        }
        private void GetPayments()
        {
            List<paymentData> list = new List<paymentData>();
            list.Clear();

            foreach (Bill current in App.User.curClient.ClientBills)
            {
                foreach (Payment p in current.Payments)
                    list.Add(new paymentData(p.Date, p.Amount.ToString()));
            }

           // dataGridPayments.ItemsSource = null;
            if (list != null)
                dataGridPayments.ItemsSource = list;
        }

        private void DisplayRest()
        {
            float total = 0;
            float paid = 0;

            foreach (Bill current in App.User.curClient.ClientBills)
            {
                try { total += current.TTC; } catch { }
                foreach (Payment p in current.Payments)
                    try { paid += p.Amount; } catch { }
            }

            float rest = total - paid;
            totalRestantBlock.Text = " " + rest.ToString() + "€";
        }

        private void setupPage()
        {
            GetConvs();
            GetBills();
            GetPayments();
            DisplayRest();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ClientMainPage));

        }

        private bool isConvEqual()
        {
            if (dataGridConventions.SelectedItem != null)
            {
                List<conventionData> list = dataGridConventions.ItemsSource as List<conventionData>;
                int i = dataGridConventions.SelectedIndex;

                conventionData it = list[i];
                foreach (Convention current in App.User.curClient.ClientConvs)
                {
                    float totalBill = 0;
                    float total = 0;
                    try { total = current.Amount + (current.Amount) * (current.Tva / 100); } catch { }

                    float paid = 0;
                    float rest = 0;
                    foreach (Bill cur in App.User.curClient.ClientBills)
                    {
                        if (cur.Conv == 1)
                        {
                            if (cur.Convention.Date.Equals(current.Date))
                            {
                                foreach (Payment p in cur.Payments)
                                    try { paid += p.Amount; } catch { }
                                try { rest = cur.TTC - paid; } catch { }
                                try { totalBill = cur.TTC; } catch { }
                                break;
                            }
                        }
                    }
                    if (current.Reference.ToString().Equals(it.getRef()) && it.Date.Equals(current.Date) && it.Total.Equals(total.ToString()) && it.Facturé.Equals(totalBill.ToString()))
                    {
                        App.User.curConv = current;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        private void AddConv_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewConvPage));
        }

        private void UpdConv_Click(object sender, RoutedEventArgs e)
        {
            if (isConvEqual())
                Frame.Navigate(typeof(EditConvPage));
        }

        private void CancelConv_Click(object sender, RoutedEventArgs e)
        {
            ConfirmConv.IsOpen = false;
        }

        private void ConfirmConv_Click(object sender, RoutedEventArgs e)
        {
            //try { Database.RemoveConv(App.User.curConv); } catch { }
            Database.RemoveAzureConv(App.User.curConv);
            
            App.User.curClient.ClientConvs.Remove(App.User.curConv);
            App.User.curConv = null;
            ConfirmConv.IsOpen = false;
            setupPage();
        }

        private void RemConv_Click(object sender, RoutedEventArgs e)
        {
            if (isConvEqual())
                ConfirmConv.IsOpen = true;
        }

        private void ExpConv_Click(object sender, RoutedEventArgs e)
        {
            if (isConvEqual())
                Pdf.Create_Conv_Pdf();
        }

        private bool isBillEquals()
        {
            if (dataGridFactures.SelectedItem != null)
            {
                List<billData> list = dataGridFactures.ItemsSource as List<billData>;
                int i = dataGridFactures.SelectedIndex;

                billData it = list[i];

                foreach (Bill current in App.User.curClient.ClientBills)
                {
                    string state;
                    float total = current.TTC;
                    float rest = current.TTC;

                    foreach (Payment p in current.Payments)
                        rest -= p.Amount;
                    if (rest >= total)
                        state = "❌";
                    else if (rest > 0 && rest < total)
                        state = "En cours...";
                    else
                        state = "✅";

                    if (current.Reference.ToString().Equals(it.getRef()) && it.Date.Equals(current.Date) && it.Total.Equals(total.ToString()) && it.Impayé.Equals(rest.ToString()) && it.Status.Equals(state))
                    {
                        App.User.curBill = current;
                        return true;
                    }
                }
            }
            return false;
        }


        private void AddBill_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewBillPage));
        }

        private void UpdBill_Click(object sender, RoutedEventArgs e)
        {
            if (isBillEquals())
                Frame.Navigate(typeof(EditBillPage));
        }

        private void CancelBill_Click(object sender, RoutedEventArgs e)
        {
            ConfirmBill.IsOpen = false;
        }


        private void ConfirmBill_Click(object sender, RoutedEventArgs e)
        {
            //try { Database.RemoveBill(App.User.curBill); } catch { }
            Database.RemoveAzureBill(App.User.curBill);
            
            App.User.curClient.ClientBills.Remove(App.User.curBill);

            //try { Database.RemovePayments(App.User.curBill.Reference, App.User.curClient.ClientID); } catch { }
            Database.RemoveAzurePayments(App.User.curBill.Reference, App.User.curClient.ClientID);
            
            App.User.curBill = null;
            ConfirmBill.IsOpen = false;
            setupPage();
        }
        private void RemBill_Click(object sender, RoutedEventArgs e)
        {
            if (isBillEquals())
                ConfirmBill.IsOpen = true;
        }
        private void ExpBill_Click(object sender, RoutedEventArgs e)
        {
            if (isBillEquals())
                Pdf.Create_Bill_Pdf();
        }

        private bool isPayEquals()
        {
            if (dataGridPayments.SelectedItem != null)
            {
                List<paymentData> list = dataGridPayments.ItemsSource as List<paymentData>;
                int i = dataGridPayments.SelectedIndex;

                paymentData it = list[i];

                foreach (Bill current in App.User.curClient.ClientBills)
                {
                    foreach (Payment p in current.Payments)
                    {
                        if (it.Date.Equals(p.Date) && it.Montant.Equals(p.Amount))
                        {
                            App.User.curBill = current;
                            App.User.curPayment = p;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewPaymentPage));
        }

        private void CancelPay_Click(object sender, RoutedEventArgs e)
        {
            ConfirmPayment.IsOpen = false;    
        }

        private void ConfirmPay_Click(object sender, RoutedEventArgs e)
        {
            //try { Database.RemovePayment(App.User.curPayment); } catch { }
            Database.RemoveAzurePayment(App.User.curPayment);
            
            App.User.curBill.Payments.Remove(App.User.curPayment);
            App.User.curPayment = null;
            ConfirmPayment.IsOpen = false;
            setupPage();
        }

        private void RemPayment_Click(object sender, RoutedEventArgs e)
        {
            if (isPayEquals())
                ConfirmPayment.IsOpen = true;
        }
    }
}
