using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Principal;
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
    public sealed partial class FullBillNewPaymentPage : Page
    {
        public FullBillNewPaymentPage()
        {
            this.InitializeComponent();
            header.Text = header.Text = $"Nouveau versement: Client {App.User.curClient}";

            int index = 0;
            int tmp = 0;
            foreach (Bill current in App.User.curClient.ClientBills)
            {
                if (!current.Equals(App.User.curBill))
                    tmp++;
                else
                    index = tmp;
                Box.Items.Add(current.ToString());
            }
            Box.SelectedIndex = index;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string bill = e.AddedItems[0].ToString();
            foreach (Bill current in App.User.curClient.ClientBills)
            {
                if (current.ToString().Equals(bill.ToString()))
                {
                    App.User.curBill = current;
                    return;
                }
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ClientPage));
        }

        private bool isPayOK()
        {
            bool isBoxOk = App.User.curBill != null;
            bool isAmountOk = float.TryParse(amount.Text, out float amountV);
            bool isdateOk = !string.IsNullOrEmpty(date.Text);

            date.BorderBrush = isdateOk ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            amount.BorderBrush = isAmountOk ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            Box.BorderBrush = isBoxOk ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);

            return isdateOk && isBoxOk && isAmountOk;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (isPayOK())
            {
                Payment current = new Payment();
                current.Amount = float.Parse(amount.Text);
                current.Date = date.Text;
                current.BillRef = App.User.curBill.Reference;
                current.ClientID = App.User.curClient.ClientID;

                //try { Database.AddPayment(current); } catch { }
                Database.AddAzurePayment(current);
                
                Frame.Navigate(typeof(FullBillPage));
            }
        }
    }
}
