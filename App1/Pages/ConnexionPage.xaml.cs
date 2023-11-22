using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public sealed partial class ConnexionPage : Page
    {
        public ConnexionPage()
        {
            this.InitializeComponent();
        }

        private void Connexion_Click(object sender, RoutedEventArgs e)
        {
            if (Username.Text.Equals("RESET"))
                Database.ResetDB();
            Database.CreateAzureBills();
            Database.CreateAzureConvs();
            Database.CreateAzurePayments();
            Database.CreateAzureUser();
            Database.InitAzureSQL();

            App.User.current = Database.GetAzureUser();

            Frame.Navigate(typeof(MainPage));
        }

    }
}
