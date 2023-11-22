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
    public sealed partial class EditClientPage : Page
    {
        public EditClientPage()
        {
            this.InitializeComponent();
            initPage();
        }

        private void initPage()
        {
            LastName.Text = App.User.curClient.LastName;
            FirstName.Text = App.User.curClient.FirstName;
            Address.Text = App.User.curClient.Address;
            City.Text = App.User.curClient.City;
            Country.Text = App.User.curClient.Country;
            PostalCode.Text = App.User.curClient.PostalCode;
            PhoneNumber.Text = App.User.curClient.Phone;
            Mail.Text = App.User.curClient.Email;
            Ref.Text = App.User.curClient.Reference;
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ClientMainPage));
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (IsCustomerOk())
            {
                Client newcli = new Client();

                newcli.LastName = LastName.Text;
                newcli.FirstName = FirstName.Text;
                newcli.Address = Address.Text;
                newcli.City = City.Text;
                newcli.Country = Country.Text;
                newcli.PostalCode = PostalCode.Text;
                newcli.Phone = PhoneNumber.Text;
                newcli.Email = Mail.Text;
                newcli.Reference = Ref.Text;

                newcli.ClientID = App.User.curClient.ClientID;

                /*
                try { Database.RemoveClientRecords(newcli.ClientID); } catch { }
                try { Database.AddClient(newcli); } catch { }
                */
                Database.RemoveAzureClientRecords(newcli.ClientID);
                Database.AddAzureClient(newcli);

                App.User.curClient = newcli;
                ClientName current = new ClientName(newcli.LastName, newcli.FirstName);
                App.User.ClientList.Insert(0, current);

                Frame.Navigate(typeof(ClientMainPage));
            }
        }
    }
}
