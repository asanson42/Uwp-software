using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            getUtils();
        }

        private void getUtils()
        {
            TVA.Text = App.User.current.TVA.ToString();
            HeadBill.Text = App.User.current.BillBegin;
            EndBill.Text = App.User.current.BillEnd;
            ConvRules.Text = App.User.current.ConvRules;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            TVA.BorderBrush = new SolidColorBrush(Colors.White);
            try
            {
                float.Parse(TVA.Text);
                TVA.BorderBrush = new SolidColorBrush(Colors.White);
            }
            catch
            {
                TVA.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }

            float tva = 0;
            try { tva = float.Parse(TVA.Text); } catch { tva = 0; }

            Users newUsers = new Users(tva, HeadBill.Text, EndBill.Text, ConvRules.Text, App.User.current.ConvNum, App.User.current.ClientNum, App.User.current.BillNum); ;

            App.User.current = newUsers;

            /*
            Database.RemoveUser();
            Database.CreateUserTable();
            Database.AddUserRecords(newUsers);
            */
            Database.RemoveAzureUser();
            Database.CreateAzureUser();
            Database.AddAzureUser(newUsers);

            Frame.Navigate(typeof(ClientMainPage));
        }
    }
}
