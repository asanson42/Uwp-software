using App1.Pages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App1
{
    /// <summary>
    /// Fournit un comportement spécifique à l'application afin de compléter la classe Application par défaut.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initialise l'objet d'application de singleton.  Il s'agit de la première ligne du code créé
        /// à être exécutée. Elle correspond donc à l'équivalent logique de main() ou WinMain().
        /// </summary>
        /// 

        public static class User
        {
            public static Users current = new Users();

            public static int ClientNumber = 1;

            public static List<ClientName> ClientList = new List<ClientName>();

            public static Client curClient = new Client();
            public static Bill curBill = new Bill();
            public static Convention curConv = new Convention();
            public static Payment curPayment = new Payment();
        }
        /*
        public void tryToServDb()
        {
            Client test = new Client();

            // INIT THE SERVER DB
            Database.InitAzureSQL();
            Database.CreateAzureUser();
            Database.InitAzureClientNumber();
            Database.CreateAzureBills();
            Database.CreateAzureConvs();
            Database.CreateAzurePayments();

            foreach (ClientName current in User.ClientList)
            {
                // GET ALL DATA
                try { test = Database.GetUniqClient(current.nom, current.prenom); } catch { test = new Client(); }
                try { test.ClientConvs = Database.GetConvs(test.ClientID); } catch { test.ClientConvs = new List<Convention>(); }
                try { test.ClientBills = Database.GetClientBills(test.ClientID); } catch { test.ClientBills = new List<Bill>(); }
                foreach (Bill bill in test.ClientBills)
                {
                    try { bill.Payments = Database.GetClientPayments(bill.Reference, test.ClientID); } catch { bill.Payments = new List<Payment>(); }
                }

                // PUSH THESE DATA
                Database.AddAzureClient(test);
                foreach (Convention conv in test.ClientConvs)
                    Database.AddAzureConv(conv);
                foreach (Bill b in test.ClientBills)
                {
                    Database.AddAzureBill(b);
                    foreach (Payment p in b.Payments)
                        Database.AddAzurePayment(p);
                }
            }
        }
        */



        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHRqVVhkVFpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF5jSnxSdk1hWH9ecnZcTg==;Mgo+DSMBPh8sVXJ0S0J+XE9AflRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TdUdiWXdccXRVRGRaWQ==;ORg4AjUWIQA/Gnt2VVhkQlFacldJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkZhWH9WcnVVRmJaVkw=;MTM5MDQ4OEAzMjMwMmUzNDJlMzBWcDJvbnljdHZwZnpIOEwwNmJjSkJXTTFXMnhyVzdZQ3JvYnFTaldtRmpJPQ==;MTM5MDQ4OUAzMjMwMmUzNDJlMzBQdGhvSEdKSFNlaS9xSlh3d0lOR3EvUlZkanBUYnc3MVNodkRIV2FhZlQ0PQ==;NRAiBiAaIQQuGjN/V0Z+WE9EaFtKVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdUVgW35eeHZURmBfVUd+;MTM5MDQ5MUAzMjMwMmUzNDJlMzBVa2tMZjgwbm5MTmU1SHFoUXN5eW1RKzJtcWJ4dkYrYSs2QUtTNWsrb0RrPQ==;MTM5MDQ5MkAzMjMwMmUzNDJlMzBMek55Y2RCZEp2Z01HTTF3SlRoUTZyLzdVU0d1WGEyOFJya3c3L2RzYXdjPQ==;Mgo+DSMBMAY9C3t2VVhkQlFacldJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxQdkZhWH9WcnVVRmReV00=;MTM5MDQ5NEAzMjMwMmUzNDJlMzBlbEk1MG9XOGhKKzJrRkh0S2RHa25EZmgvQ1FqTEtVakdFQ2Uva2llZ3RrPQ==;MTM5MDQ5NUAzMjMwMmUzNDJlMzBrVE9IeDV5OGNqcng0Z3F2bXFrSWhTU0tRZ1p0UDNKNkh6UFpzQXVVSzhZPQ==;MTM5MDQ5NkAzMjMwMmUzNDJlMzBVa2tMZjgwbm5MTmU1SHFoUXN5eW1RKzJtcWJ4dkYrYSs2QUtTNWsrb0RrPQ==");

            //Database.ResetDB();

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoqué lorsque l'application est lancée normalement par l'utilisateur final.  D'autres points d'entrée
        /// seront utilisés par exemple au moment du lancement de l'application pour l'ouverture d'un fichier spécifique.
        /// </summary>
        /// <param name="e">Détails concernant la requête et le processus de lancement.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Ne répétez pas l'initialisation de l'application lorsque la fenêtre comporte déjà du contenu,
            // assurez-vous juste que la fenêtre est active
            if (rootFrame == null)
            {
                // Créez un Frame utilisable comme contexte de navigation et naviguez jusqu'à la première page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: chargez l'état de l'application précédemment suspendue
                }

                // Placez le frame dans la fenêtre active
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Quand la pile de navigation n'est pas restaurée, accédez à la première page,
                    // puis configurez la nouvelle page en transmettant les informations requises en tant que
                    // paramètre
                    rootFrame.Navigate(typeof(ConnexionPage), e.Arguments);
                }
                // Vérifiez que la fenêtre actuelle est active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Appelé lorsque la navigation vers une page donnée échoue
        /// </summary>
        /// <param name="sender">Frame à l'origine de l'échec de navigation.</param>
        /// <param name="e">Détails relatifs à l'échec de navigation</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Appelé lorsque l'exécution de l'application est suspendue.  L'état de l'application est enregistré
        /// sans savoir si l'application pourra se fermer ou reprendre sans endommager
        /// le contenu de la mémoire.
        /// </summary>
        /// <param name="sender">Source de la requête de suspension.</param>
        /// <param name="e">Détails de la requête de suspension.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: enregistrez l'état de l'application et arrêtez toute activité en arrière-plan
            deferral.Complete();
        }
    }
}
