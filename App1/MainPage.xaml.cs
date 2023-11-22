using App1.Pages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Dictionary<string, Type> pages = new Dictionary<string, Type>()
        {
            { "Clients", typeof(ClientMainPage) },
            { "Bills", typeof(FullBillPage) },
            { "setting", typeof(SettingsPage) }
        };

        public static void tryFullInit()
        {
            Database.InitAzureSQL();
            Database.CreateAzureUser();
            Database.InitAzureClientNumber();
            Database.CreateAzureBills();
            Database.CreateAzureConvs();
            Database.CreateAzurePayments();
        }

        public MainPage()
        {
            this.InitializeComponent();
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems[0];
            ContentFrame.Navigate(typeof(ClientMainPage));
        }

        private void NavigationView_Navigate(string tag, NavigationTransitionInfo transitionInfo)
        {
            Type page = null;
            var item = pages.FirstOrDefault(p => p.Key.Equals(tag));
            page = item.Value;
            var previousNavPageType = ContentFrame.CurrentSourcePageType;
            if (page != null && !Equals(previousNavPageType, page))
                ContentFrame.Navigate(page, null, transitionInfo);
        }

        private void NavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var NavItemTag = args.InvokedItemContainer.Tag.ToString();
            NavigationView_Navigate(NavItemTag, args.RecommendedNavigationTransitionInfo);
        }

    }
}
