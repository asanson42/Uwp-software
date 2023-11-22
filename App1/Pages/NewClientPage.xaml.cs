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
    public sealed partial class NewClientPage : Page
    {
        public NewClientPage()
        {
            this.InitializeComponent();

            Ref.Text = DateTime.Now.ToString("yyyy-MM") + "-" + (App.User.current.ClientNum + 1).ToString();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ClientMainPage));
        }

        private bool IsCustomerOk()
        {
            var controlsToCheck = new[] { LastName, FirstName, Address, City, PostalCode, Country, PhoneNumber, Mail };

            foreach (var control in controlsToCheck)
            {
                control.BorderBrush = string.IsNullOrWhiteSpace(control.Text) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White);
            }

            Ref.BorderBrush = string.IsNullOrWhiteSpace(Ref.Text) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White);

            return controlsToCheck.All(control => !string.IsNullOrWhiteSpace(control.Text)) && !string.IsNullOrWhiteSpace(Ref.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (IsCustomerOk())
            {
                Client newcli = new Client();

                newcli.LastName = LastName.Text.ToString();
                newcli.FirstName = FirstName.Text.ToString();
                newcli.Address = Address.Text.ToString();
                newcli.City = City.Text.ToString();
                newcli.Country = Country.Text.ToString();
                newcli.PostalCode = PostalCode.Text.ToString();
                newcli.Phone = PhoneNumber.Text.ToString();
                newcli.Email = Mail.Text.ToString();
                newcli.Reference = Ref.Text.ToString();

                newcli.ClientID = App.User.ClientNumber + 1;

                Database.IncrementNumCli();

                App.User.ClientNumber += 1;

                // potentiel ajout d'un client a une liste App.User.clientName

                //Database.AddClient(newcli);
                Database.AddAzureClient(newcli);
                App.User.current.ClientNum++;

                Database.RemoveAzureUser();
                Database.CreateAzureUser();
                Database.AddAzureUser(App.User.current);

                ClientName current = new ClientName(newcli.LastName, newcli.FirstName);
                App.User.ClientList.Insert(0, current);

                Frame.Navigate(typeof(ClientMainPage));
            }
        }
    }
}
