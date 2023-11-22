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
    public sealed partial class NewConvPage : Page
    {
        public NewConvPage()
        {
            this.InitializeComponent();
            TVA.Text = App.User.current.TVA.ToString();
            date.Text = DateTime.Now.ToString("dd/MM/yyyy");
            reference.Text = DateTime.Now.ToString("yyyy-MM") + "-" + (App.User.current.ConvNum + 1).ToString();
            header.Text = $"Nouvelle Convention: Client {App.User.curClient}";
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ClientPage));
        }

        private bool isConvOk()
        {
            bool isDateValid = !string.IsNullOrEmpty(date.Text);
            bool isServiceValid = !string.IsNullOrEmpty(services.Text);
            bool isRefValid = !string.IsNullOrEmpty(reference.Text);
            bool isBaseValid = float.TryParse(base_horaire.Text, out float baseV);
            bool isMountValid = float.TryParse(montant.Text, out float MountV);
            bool isTVAValid = float.TryParse(TVA.Text, out float TaxV);

            date.BorderBrush = isDateValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            services.BorderBrush = isServiceValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            reference.BorderBrush = isRefValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            base_horaire.BorderBrush = isBaseValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            montant.BorderBrush = isMountValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            TVA.BorderBrush = isTVAValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);

            return isDateValid && isServiceValid && isRefValid && isBaseValid && isMountValid && isTVAValid;
        }

        private void Sign_Toggled(object sender, RoutedEventArgs e)
        {
            if (Sign.IsOn)
                App.User.curConv.sign = 1;
            else
                App.User.curConv.sign = 0;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (isConvOk())
            {
                Convention newConv = new Convention();

                newConv.Date = date.Text;
                newConv.Service = services.Text;
                newConv.Reference = reference.Text;
                newConv.Fees = float.Parse(base_horaire.Text);
                newConv.Amount = float.Parse(montant.Text);
                newConv.Tva = float.Parse(TVA.Text);
                newConv.current = App.User.curClient;
                newConv.ClientID = App.User.curClient.ClientID;

                if (Sign.IsOn)
                    newConv.sign = 1;
                else
                    newConv.sign = 0;

                App.User.curClient.ClientConvs.Add(newConv);

                //try { Database.AddConv(newConv); } catch { }
                Database.AddAzureConv(newConv);
                App.User.current.ConvNum++;

                Database.RemoveAzureUser();
                Database.CreateAzureUser();
                Database.AddAzureUser(App.User.current);

                Frame.Navigate(typeof(ClientPage));
            }
        }
    }
}
