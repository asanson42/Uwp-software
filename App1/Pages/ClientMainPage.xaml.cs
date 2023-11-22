using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Windows.Security.Isolation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
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
    public sealed partial class ClientMainPage : Page
    {
        public ClientMainPage()
        {
            this.InitializeComponent();
            initPage();
        }

        private void initPage()
        {

            App.User.ClientList.Clear();
            App.User.ClientList = Database.GetAzureClientNames();
                if (dataGridClients.ItemsSource != null)
                    dataGridClients.ItemsSource = null;
                dataGridClients.ItemsSource = App.User.ClientList;

        }

        private bool isEqual()
        {
            if (dataGridClients.SelectedItem != null)
            {
                ClientName selectedClient = dataGridClients.SelectedItem as ClientName;

                //App.User.curClient = Database.GetUniqClient(selectedClient.lastname, selectedClient.firstname);
                App.User.curClient = Database.GetAzureClient(selectedClient.nom, selectedClient.prenom);

                return true;
            }
            return false;
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            if (isEqual())
            {
                Frame.Navigate(typeof(EditClientPage));
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ConfirmPopup.IsOpen = false;
        }

        private void refreshList(ClientName removed)
        {
            List<ClientName> tmp = new List<ClientName>();

            foreach (ClientName current in App.User.ClientList)
            {
                if (!current.ToString().Equals(removed.ToString()))
                    tmp.Add(new ClientName(current.nom, current.prenom));
            }
            App.User.ClientList.Clear();
            App.User.ClientList = tmp;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            // Supprimer le client sélectionné ici
            Database.RemoveAzureClientsPayments(App.User.curClient.ClientID);
            Database.RemoveAzureBills(App.User.curClient.ClientID);
            Database.RemoveAzureConvs(App.User.curClient.ClientID);
            Database.RemoveAzureClientRecords(App.User.curClient.ClientID);

            ClientName current = new ClientName(App.User.curClient.LastName, App.User.curClient.FirstName);
            refreshList(current);
            dataGridClients.ItemsSource = null;
            dataGridClients.ItemsSource = App.User.ClientList;
            App.User.curClient = null;
            ConfirmPopup.IsOpen = false;
        }

        private void RemoveClient_Click(object sender, RoutedEventArgs e)
        {
            // Afficher le popup de confirmation
            if (isEqual())
                ConfirmPopup.IsOpen = true;
        }

        private void NewClient_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewClientPage));
        }

        private void BillClient_Click(object sender, RoutedEventArgs e)
        {
            if (isEqual())
                Frame.Navigate(typeof(ClientPage));
        }

        private List<ClientName> GetFilteredClientNames(string searchText)
        {
            List<ClientName> list = new List<ClientName>();

            foreach (ClientName current in App.User.ClientList)
            {
                if (current.nom.ToLower().StartsWith(searchText.ToLower()) || current.prenom.ToLower().StartsWith(searchText.ToLower()))
                    list.Add(current);
            }
            return list;
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchBar.Text.Trim();
            List<ClientName> filteredList = new List<ClientName>();

            if (string.IsNullOrEmpty(searchText))
                initPage();
            else
            {
                filteredList = GetFilteredClientNames(searchText);
                dataGridClients.ItemsSource = filteredList;
            }
        }

        private void Search_Btn_Click(object sender, RoutedEventArgs e)
        {
            App.User.ClientList = Database.GetAzureClientNames();
            initPage();
        }

    }
}
