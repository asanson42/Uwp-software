using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace App1
{
    internal class Database
    {
        public Database()
        {

        }

        // UTILS

        private static string ft_parse(string str)
        {
            string nstr = "";

            if (str == null)
                return str;
            foreach (char current in str)
            {
                if (current == '\'')
                    nstr += '$';
                else
                    nstr += current;
            }
            return nstr;
        }

        private static string get_parse(string str)
        {
            string nstr = "";

            if (str == null)
                return str;
            foreach (char current in str)
            {
                if (current == '$')
                    nstr += '\'';
                else
                    nstr += current;
            }
            return nstr;
        }

        // RESET DB

        public static void ResetDB()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string resetUsers = "DROP TABLE UserTable";
                string resetClients = "DROP TABLE ClientTable";
                string resetBills = "DROP TABLE BillsTable";
                string resetConvs = "DROP TABLE ConvsTable";
                string resetPayments = "DROP TABLE PaymentTable";

                SqliteCommand RESET_USER = new SqliteCommand(resetUsers, con);
                RESET_USER.ExecuteNonQuery();
                SqliteCommand RESET_CLIENTS = new SqliteCommand(resetClients, con);
                RESET_CLIENTS.ExecuteNonQuery();
                SqliteCommand RESET_BILLS = new SqliteCommand(resetBills, con);
                RESET_BILLS.ExecuteNonQuery();
                SqliteCommand RESET_CONVS = new SqliteCommand(resetConvs, con);
                RESET_CONVS.ExecuteNonQuery();
                SqliteCommand RESET_PAYMENTS = new SqliteCommand(resetPayments, con);
                RESET_PAYMENTS.ExecuteNonQuery();

                con.Close();
            }
        }

        // MANAGE USER
        /*
        public static void CreateUserTable()
        {
            string tableName = "User";
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "DataBase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                String initCMD =
                    $"CREATE TABLE IF NOT EXISTS" +
                    $"'{tableName}' (Tva NVARCHAR(10)," +
                    $"BillBegin NVARCHAR(20000)," +
                    $"BillEnding NVARCHAR(20000)," +
                    $"ConvRules NVARCHAR(50000))";

                SqliteCommand CreateCMD = new SqliteCommand(initCMD, con);
                CreateCMD.ExecuteReader();
                con.Close();
            }
        }

        public static void AddUserRecords(Users current)
        {
            string tableName = "User";
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "DataBase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                String AddUser = $"INSERT INTO '{tableName}'(Tva, BillBegin, BillEnding, ConvRules) " +
                    $"VALUES('{ft_parse(current.TVA.ToString())}', '{ft_parse(current.BillBegin.ToString())}', '{ft_parse(current.BillEnd.ToString())}', '{ft_parse(current.ConvRules.ToString())}')";

                SqliteCommand CMD_Insert = new SqliteCommand(AddUser, con);

                CMD_Insert.ExecuteReader();
                con.Close();
            }
        }

        public static Users GetUsers()
        {
            Users newUsers = new Users();
            string tableName = "User";
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "DataBase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                String SelectCommand = $"SELECT Tva, BillBegin, BillEnding, ConvRules FROM '{tableName}'";

                SqliteCommand CMD_GETREC = new SqliteCommand(SelectCommand, con);

                SqliteDataReader reader = CMD_GETREC.ExecuteReader();

                while (reader.Read())
                {
                    newUsers = new Users(get_parse(reader.GetString(0)), get_parse(reader.GetString(1)), get_parse(reader.GetString(2)), get_parse(reader.GetString(3)));
                }

                con.Close();
            }
            return newUsers;
        }

        public static void RemoveUser()
        {
            string tableName = "User";
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "DataBase.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                String DelCMD = $"DROP TABLE '{tableName}'";

                SqliteCommand DeleteCMD = new SqliteCommand(DelCMD, con);
                DeleteCMD.ExecuteReader();

                con.Close();
            }
        }
        */

        //AZURE USER
        public static void CreateAzureUser()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string init =
                    "CREATE TABLE IF NOT EXISTS UserTable(" +
                    "[Tva] [float], " +
                    "[BillBegin] [nvarchar](2000), " +
                    "[BillEnding] [nvarchar](2000), " +
                    "[ConvRules] [nvarchar](2000), " +
                    "[NumConv] [float], " +
                    "[NumCli] [float], " +
                    "[NumBill] [float])";

                SqliteCommand create = new SqliteCommand(init, con);
                create.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void AddAzureUser(Users current)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string AddUser = $"INSERT INTO UserTable(Tva, BillBegin, BillEnding, ConvRules, NumConv, NumCli, NumBill) " +
                                $"VALUES('{current.TVA}', '{ft_parse(current.BillBegin)}', '{ft_parse(current.BillEnd)}', '{ft_parse(current.ConvRules)}', " +
                                $"'{current.ConvNum}', '{current.ClientNum}', '{current.BillNum}')";

                SqliteCommand command = new SqliteCommand(AddUser, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static Users GetAzureUser()
        {
            Users newUsers = new Users();
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string select = "SELECT Tva, BillBegin, BillEnding, ConvRules, NumConv, NumCli, NumBill FROM UserTable";
                SqliteCommand command = new SqliteCommand(select, con);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    double valeur0 = (double)reader[0];
                    double valeur4 = (double)reader[4];
                    double valeur5 = (double)reader[5];
                    double valeur6 = (double)reader[6];
                    float v0 = Convert.ToSingle(valeur0);
                    float v4 = Convert.ToSingle(valeur4);
                    float v5 = Convert.ToSingle(valeur5);
                    float v6 = Convert.ToSingle(valeur6);

                    newUsers = new Users(v0, get_parse(reader.GetString(1)), get_parse(reader.GetString(2)), get_parse(reader.GetString(3)), v4, v5, v6);
                }
                con.Close();
            }
            return newUsers;
        }

        public static void RemoveAzureUser()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string delete = "DROP TABLE UserTable";
                SqliteCommand command = new SqliteCommand(delete, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void IncrementNumConv()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                SqliteCommand cmd = new SqliteCommand("UPDATE [dbo].[UserTable] SET NumConv = NumConv + 1", con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void IncrementNumCli()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                SqliteCommand cmd = new SqliteCommand("UPDATE UserTable SET NumCli = NumCli + 1", con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void IncrementNumBill()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                SqliteCommand cmd = new SqliteCommand("UPDATE [dbo].[UserTable] SET NumBill = NumBill + 1", con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        // MANAGE CLIENTS
        /*
        public async static void InitializeDB()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("Database.db", CreationCollisionOption.OpenIfExists);
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();

                string INIT = "CREATE TABLE IF NOT EXISTS " +
                              "ClientTable (ClientLastName NVARCHAR(20), " +
                              "ClientFirstName NVARCHAR(20), " +
                              "ClientAddress NVARCHAR(30), " +
                              "ClientPostCode NVARCHAR(6), " +
                              "ClientCity NVARCHAR(20), " +
                              "ClientCountry NVARCHAR(20), " +
                              "ClientPhone NVARCHAR(12), " +
                              "ClientEmail NVARCHAR(40), " +
                              "ClientReference NVARCHAR(30), " +
                              "ClientID INT NOT NULL)";

                SqliteCommand CREATE = new SqliteCommand(INIT, con);
                CREATE.ExecuteReader();
                con.Close();
            }
        }

        public static void RemoveClients()
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();

                string DEL = "DROP TABLE ClientTable";
                SqliteCommand DELETE = new SqliteCommand(DEL, con);
                DELETE.ExecuteReader();
                con.Close();
            }
        }

        public static void RemoveClientRecords(int ID)
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();

                string DEL = $"DELETE FROM ClientTable WHERE ClientID = {ID}";
                SqliteCommand DELETE = new SqliteCommand(DEL, con);
                DELETE.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void AddClient(Client current)
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();
                SqliteCommand InsertCMD = new SqliteCommand();
                InsertCMD.Connection = con;

                InsertCMD.CommandText = $"INSERT INTO ClientTable(ClientLastName, ClientFirstName, ClientAddress, ClientPostCode, " +
                                        $"ClientCity, ClientCountry, ClientPhone, ClientEmail, ClientReference, ClientID) " +
                                        $"VALUES('{ft_parse(current.LastName)}', '{ft_parse(current.FirstName)}', '{ft_parse(current.Address)}', '{ft_parse(current.PostalCode)}'," +
                                        $"'{ft_parse(current.City)}', '{ft_parse(current.Country)}', '{ft_parse(current.Phone)}', '{ft_parse(current.Email)}', '{ft_parse(current.Reference)}', '{current.ClientID}')";

                InsertCMD.ExecuteReader();
                con.Close();
            }
        }

        public static Client GetUniqClient(string lastname, string firstname)
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            Client client = new Client();

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM ClientTable WHERE ClientLastName LIKE '{ft_parse(lastname)}' AND ClientFirstName LIKE '{ft_parse(firstname)}'";

                SqliteCommand CMD = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = CMD.ExecuteReader();

                while (reader.Read()) { 
                    client = new Client(
                        get_parse(reader.GetString(0)),
                        get_parse(reader.GetString(1)),
                        get_parse(reader.GetString(2)),
                        get_parse(reader.GetString(3)),
                        get_parse(reader.GetString(4)),
                        get_parse(reader.GetString(5)),
                        get_parse(reader.GetString(6)),
                        get_parse(reader.GetString(7)),
                        get_parse(reader.GetString(8)),
                        reader.GetInt32(9));
                }
                con.Close();
            }
            return client;
        }

        public static List<ClientName> GetFilteredClientNames(string searchText)
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<ClientName> list = new List<ClientName>();

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();
                string SELECT = "SELECT ClientLastName, ClientFirstName FROM ClientTable WHERE ClientLastName LIKE @searchText LIMIT 50";
                SqliteCommand CMD = new SqliteCommand(SELECT, con);
                CMD.Parameters.AddWithValue("@searchText", $"{searchText}%");
                SqliteDataReader reader = CMD.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ClientName(get_parse(reader.GetString(0)), get_parse(reader.GetString(1))));
                }
                con.Close();
            }
            return list;
        }


        public static List<ClientName> GetClientNames()
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<ClientName> list = new List<ClientName>();

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();
                string SELECT = "SELECT ClientLastName, ClientFirstName FROM ClientTable";
                SqliteCommand CMD = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = CMD.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ClientName(get_parse(reader.GetString(0)), get_parse(reader.GetString(1))));
                }
                con.Close();
            }
            return list;
        }

        public static ClientName GetClientByIndex(int ID)
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            ClientName current;

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();
                string SELECT = $"SELECT ClientLastName, ClientFirstName FROM ClientTable WHERE ClientID LIKE '{ID}'";
                SqliteCommand cmd = new SqliteCommand(SELECT, con);
                SqliteDataReader r = cmd.ExecuteReader();
                r.Read();
                current = new ClientName(r.GetString(0), r.GetString(1));
                con.Close();
            }
            return current;
        }
        */
        // CLIENT BY AZURE

        public static void InitAzureSQL()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string init = "CREATE TABLE IF NOT EXISTS " +
                              "ClientTable(" +
                    "[ClientLastName] [nvarchar](120) NULL, " +
                    "[ClientFirstName] [nvarchar](120) NULL," +
                    " [ClientAddress] [nvarchar](130) NULL, " +
                    "[ClientPostCode] [nvarchar](50) NULL, " +
                    "[ClientCity] [nvarchar](120) NULL, " +
                    "[ClientCountry] [nvarchar](120) NULL," +
                    " [ClientPhone] [nvarchar](40) NULL," +
                    " [ClientEmail] [nvarchar](140) NULL," +
                    " [ClientReference] [nvarchar](50) NULL," +
                    " [ClientID] [int] NOT NULL)";

                SqliteCommand CREATE = new SqliteCommand(init, con);

                CREATE.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzureClients()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string delete = "DROP TABLE ClientTable";
                SqliteCommand command = new SqliteCommand(delete, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzureClientRecords(int ID)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string delete = $"DELETE FROM ClientTable WHERE ClientID = {ID}";
                SqliteCommand command = new SqliteCommand(delete, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void AddAzureClient(Client current)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = con;
                command.CommandText = "INSERT INTO ClientTable(ClientLastName, ClientFirstName, ClientAddress, ClientPostCode, " +
                                      "ClientCity, ClientCountry, ClientPhone, ClientEmail, ClientReference, ClientID) " +
                                      "VALUES(@lastname, @firstname, @address, @postcode, @city, @country, @phone, @email, @ref, @id)";

                command.Parameters.AddWithValue("@lastname", ft_parse(current.LastName));
                command.Parameters.AddWithValue("@firstname", ft_parse(current.FirstName));
                command.Parameters.AddWithValue("@address", ft_parse(current.Address));
                command.Parameters.AddWithValue("@postcode", ft_parse(current.PostalCode));
                command.Parameters.AddWithValue("@city", ft_parse(current.Phone));
                command.Parameters.AddWithValue("@country", ft_parse(current.Country));
                command.Parameters.AddWithValue("@phone", ft_parse(current.Phone));
                command.Parameters.AddWithValue("@email", ft_parse(current.Email));
                command.Parameters.AddWithValue("@ref", ft_parse(current.Reference));
                command.Parameters.AddWithValue("@id", current.ClientID);

                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static Client GetAzureClient(string last, string first)
        {
            Client client = new Client();
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string select = $"SELECT * FROM ClientTable WHERE ClientLastName LIKE '{ft_parse(last)}' AND ClientFirstName LIKE '{ft_parse(first)}'";
                SqliteCommand command = new SqliteCommand(select, con);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    client = new Client(
                        get_parse(reader.GetString(0)),
                        get_parse(reader.GetString(1)),
                        get_parse(reader.GetString(2)),
                        get_parse(reader.GetString(3)),
                        get_parse(reader.GetString(4)),
                        get_parse(reader.GetString(5)),
                        get_parse(reader.GetString(6)),
                        get_parse(reader.GetString(7)),
                        get_parse(reader.GetString(8)),
                        reader.GetInt32(9));
                }

                con.Close();
            }
            return client;
        }

        public static List<ClientName> GetAzureClientNames()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<ClientName> list = new List<ClientName>();

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string SELECT = "SELECT ClientLastName, ClientFirstName FROM ClientTable";
                SqliteCommand cmd = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new ClientName(get_parse(reader.GetString(0)), get_parse(reader.GetString(1))));
                }
                con.Close();
            }
            return list;
        }

        public static ClientName GetAzureClientByIndex(int ID)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            ClientName current;

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string SELECT = $"SELECT ClientLastName, ClientFirstName FROM ClientTable WHERE ClientID = '{ID}'";
                SqliteCommand cmd = new SqliteCommand(SELECT, con);
                SqliteDataReader r = cmd.ExecuteReader();
                r.Read();
                current = new ClientName(r.GetString(0), r.GetString(1));
                con.Close();
            }
            return current;
        }



        // MANAGE CLIENT NUMBER
        /*
        public static void InitClientNumber()
        {
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();
                string INIT = "CREATE TABLE IF NOT EXISTS NumTable(ClientNum INT NOT NULL)";

                SqliteCommand CMD = new SqliteCommand(INIT, con);
                CMD.ExecuteReader();
                con.Close();
            }
        }

        public static int ReturnClientNumber()
        {
            int result = 0;
            string pathToDb = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDb}"))
            {
                con.Open();

                string SELECT = "SELECT ClientNum FROM NumTable";
                SqliteCommand CMD = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = CMD.ExecuteReader();
                reader.Read();
                result = reader.GetInt32(0);
                con.Close();
            }
            return result;
        }

        public static void IncrementClientNumber(int num)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                // Vérifier si la table NumTable existe
                string checkTableExists = "SELECT name FROM sqlite_master WHERE type='table' AND name='NumTable'";
                SqliteCommand checkCommand = new SqliteCommand(checkTableExists, con);
                var result = checkCommand.ExecuteScalar();

                // Si la table NumTable existe, la supprimer
                if (result != null)
                {
                    string deletePrevious = "DROP TABLE NumTable";
                    SqliteCommand deleteCommand = new SqliteCommand(deletePrevious, con);
                    deleteCommand.ExecuteReader();
                }

                // Créer la table NumTable
                string createTable = "CREATE TABLE IF NOT EXISTS NumTable (ClientNum INT NOT NULL)";
                SqliteCommand createCommand = new SqliteCommand(createTable, con);
                createCommand.ExecuteReader();

                // Insérer le nouveau numéro de client
                string addNext = $"INSERT INTO NumTable(ClientNum) VALUES('{num}')";
                SqliteCommand addCommand = new SqliteCommand(addNext, con);
                addCommand.ExecuteReader();

                con.Close();
            }
        }
        */
        //AZURE INCREMENT
        public static void InitAzureClientNumber()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string init = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NumTable]') AND type in (N'U'))" +
                                    "CREATE TABLE [dbo].[NumTable](" +
                                    "[ClientNum] [int] NOT NULL)";
                SqliteCommand CMD = new SqliteCommand(init, con);
                CMD.ExecuteNonQuery();
                con.Close();
            }
        }
        public static int ReturnAzureClientNumber()
        {
            int result = 0;
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string SELECT = "SELECT ClientNum FROM NumTable";
                SqliteCommand CMD = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = CMD.ExecuteReader();
                reader.Read();
                result = reader.GetInt32(0);
                con.Close();
            }
            return result;
        }

        public static void IncrementAzureNumber(int num)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                //string checkTableExists = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'NumTable'";
                //SqliteCommand check = new SqliteCommand(checkTableExists, con);
                //var result = check.ExecuteScalar();

                //if (result != null)
                //{
                    string delete = "DROP TABLE IF EXISTS NumTable";
                    SqliteCommand delcommand = new SqliteCommand(delete, con);
                    delcommand.ExecuteNonQuery();
                //}

                string init = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NumTable]') AND type in (N'U'))" +
                                                    "CREATE TABLE [dbo].[NumTable](" +
                                                    "[ClientNum] [int] NOT NULL)";
                SqliteCommand initcmd = new SqliteCommand(init, con);
                initcmd.ExecuteNonQuery();

                string addnew = $"INSERT INTO NumTable(ClientNum) VALUES('{num}')";
                SqliteCommand addcommand = new SqliteCommand(addnew, con);
                addcommand.ExecuteNonQuery();
                con.Close();
            }
        }

        // MANAGE BILLS
        /*
        public static void CreateBillTable()
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                string INIT = "CREATE TABLE IF NOT EXISTS " +
                                "BillTable (BillDate NVARCHAR(15), " +
                                "BillRef NVARCHAR(20), " +
                                "BillService NVARCHAR(2000), " +
                                "BillHour NVARCHAR(7), " +
                                "BillTVA NVARCHAR(10), " +
                                "BillTTC NVARCHAR(10), " +
                                "BillTHT NVARCHAR(10), " +
                                "BillConv INT, " +
                                "BillConvRef NVARCHAR(20), " +
                                "ClientID INT NOT NULL)";
                SqliteCommand initCommand = new SqliteCommand(INIT, con);
                initCommand.ExecuteReader();
                con.Close();
            }
        }

        public static void RemoveBills(int Client)
        {
            // DEMANDER A CHATGPT
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string DEL = $"DELETE FROM BillTable WHERE ClientID LIKE '{Client}'";
                SqliteCommand CMD = new SqliteCommand(DEL, con);
                CMD.ExecuteReader();
                con.Close();
            }
        }

        public static void RemoveBill(Bill current)
        {
            // DEMANDER A CHATGPT
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                string DEL = $"DELETE FROM BillTable WHERE BillRef LIKE '{ft_parse(current.Reference)}' AND ClientID LIKE '{current.ClientID}'";
                SqliteCommand CMD = new SqliteCommand(DEL, con);
                CMD.ExecuteReader();
                con.Close();
            }
        }

        public static void AddBill(Bill current)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            string convRef = "null";
            if (current.Conv == 1)
                convRef = current.Convention.Reference;

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                SqliteCommand INSERT = new SqliteCommand();
                INSERT.Connection = con;
                INSERT.CommandText = $"INSERT INTO BillTable(BillDate, BillRef, BillService, BillHour, BillTVA, BillTTC, BillTHT, BillConv, BillConvRef, ClientID) " +
                                     $"VALUES('{ft_parse(current.Date)}', '{ft_parse(current.Reference)}', '{ft_parse(current.Service)}', '{current.Hour}', '{current.Tva}', " +
                                     $"'{current.TTC}', '{current.THT}', '{current.Conv}', '{ft_parse(convRef)}', '{current.ClientID}')";
                INSERT.ExecuteReader();
                con.Close();
            }
        }

        public static List<Bill> GetClientBills(int ID)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<Bill> list = new List<Bill>();

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM BillTable WHERE ClientID LIKE '{ID}'";
                SqliteCommand CMD = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = CMD.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Bill(get_parse(reader.GetString(0)), get_parse(reader.GetString(1)), get_parse(reader.GetString(2)), reader.GetString(3), reader.GetString(4),
                                      reader.GetString(5), reader.GetString(6), reader.GetInt32(7), get_parse(reader.GetString(8)), reader.GetInt32(9)));
                }
                con.Close();
            }
            return list;
        }

        public static List<Bill> GetAllBills()
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<Bill> list = new List<Bill>();

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM BillTable";
                SqliteCommand CMD = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = CMD.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Bill(get_parse(reader.GetString(0)), get_parse(reader.GetString(1)), get_parse(reader.GetString(2)), reader.GetString(3), reader.GetString(4),
                                      reader.GetString(5), reader.GetString(6), reader.GetInt32(7), get_parse(reader.GetString(8)), reader.GetInt32(9)));
                }
                con.Close();
            }
            return list;
        }
        */
        //AZURE BILLS

        public static void CreateAzureBills()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string init = 
                              "CREATE TABLE IF NOT EXISTS BillsTable(" +
                              "[BillDate] [nvarchar](25), " +
                              "[BillRef] [nvarchar](120), " +
                              "[BillService] [nvarchar](4000), " +
                              "[BillHour] [float], " +
                              "[BillTva] [float], " +
                              "[BillTTC] [float], " +
                              "[BillTHT] [float], " +
                              "[BillConv] [int], " +
                              "[BillConvRef] [nvarchar](120), " +
                              "[ClientID] [int] NOT NULL)";
                SqliteCommand command = new SqliteCommand(init, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzureBills(int client)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string DEL = $"DELETE FROM BillsTable WHERE ClientID = {client}";
                SqliteCommand command = new SqliteCommand(DEL, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzureBill(Bill current)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string DEL = $"DELETE FROM BillsTable WHERE BillRef LIKE '{ft_parse(current.Reference)}' AND ClientID LIKE '{current.ClientID}'";
                SqliteCommand command = new SqliteCommand(DEL, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void AddAzureBill(Bill current)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            string convRef = "null";
            if (current.Conv == 1)
                convRef = current.Convention.Reference;

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = con;
                command.CommandText = "INSERT INTO BillsTable(BillDate, BillRef, BillService, BillHour, " +
                                      "BillTva, BillTTC, BillTHT, BillConv, BillConvRef, ClientID) " +
                                      "VALUES(@date, @ref, @service, @hour, @tva, @ttc, @tht, @conv, @convref, @id)";

                command.Parameters.AddWithValue("@date", ft_parse(current.Date));
                command.Parameters.AddWithValue("@ref", ft_parse(current.Reference));
                command.Parameters.AddWithValue("@service", ft_parse(current.Service));
                command.Parameters.AddWithValue("@hour", current.Hour);
                command.Parameters.AddWithValue("@tva", current.Tva);
                command.Parameters.AddWithValue("@ttc", current.TTC);
                command.Parameters.AddWithValue("@tht", current.THT);
                command.Parameters.AddWithValue("@conv", current.Conv);
                command.Parameters.AddWithValue("@convref", (object)(ft_parse(convRef)) ?? DBNull.Value);
                command.Parameters.AddWithValue("@id", current.ClientID);

                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static List<Bill> GetAzureClientBills(int id)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<Bill> list = new List<Bill>();

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM BillsTable WHERE ClientID = '{id}'";
                SqliteCommand command = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    double valeur3 = (double)reader[3];
                    double valeur4 = (double)reader[4];
                    double valeur5 = (double)reader[5];
                    double valeur6 = (double)reader[6];
                    float v3 = Convert.ToSingle(valeur3);
                    float v4 = Convert.ToSingle(valeur4);
                    float v5 = Convert.ToSingle(valeur5);
                    float v6 = Convert.ToSingle(valeur6);

                    list.Add(new Bill(get_parse(reader.GetString(0)), get_parse(reader.GetString(1)), get_parse(reader.GetString(2)), v3, v4,
                                              v5, v6, reader.GetInt32(7), get_parse(reader.GetString(8)), reader.GetInt32(9)));
                }
                con.Close();
            }
            return list;
        }

        public static List<Bill> GetAzureBills()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<Bill> list = new List<Bill>();

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM BillsTable";
                SqliteCommand command = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    double valeur3 = (double)reader[3];
                    double valeur4 = (double)reader[4];
                    double valeur5 = (double)reader[5];
                    double valeur6 = (double)reader[6];
                    float v3 = Convert.ToSingle(valeur3);
                    float v4 = Convert.ToSingle(valeur4);
                    float v5 = Convert.ToSingle(valeur5);
                    float v6 = Convert.ToSingle(valeur6);

                    list.Add(new Bill(get_parse(reader.GetString(0)), get_parse(reader.GetString(1)), get_parse(reader.GetString(2)), v3, v4,
                                              v5, v6, reader.GetInt32(7), get_parse(reader.GetString(8)), reader.GetInt32(9)));
                }
                con.Close();
            }
            return list;
        }

        // MANAGE CONVENTIONS
        /*
        public static void CreateConvTable()
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string INIT = "CREATE TABLE IF NOT EXISTS " +
                            "ConvTable(ConvDate NVARCHAR(15), " +
                            "ConvRef NVARCHAR(20), " +
                            "ConvServ NVARCHAR(2000), " +
                            "ConvFees NVARCHAR(10), " +
                            "ConvAmount NVARCHAR(10), " +
                            "ConvTax NVARCHAR(10), " +
                            "ConvSign INT, " +
                            "ClientID INT NOT NULL)";
                SqliteCommand cmd = new SqliteCommand(INIT, con);
                cmd.ExecuteReader();
                con.Close();
            }
        }

        public static void RemoveConvs(int ID)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                string DELETE = $"DELETE FROM ConvTable WHERE ClientID LIKE '{ID}'";
                SqliteCommand cmd = new SqliteCommand(DELETE, con);
                cmd.ExecuteReader();
                con.Close();
            }
        }
        public static void RemoveConv(Convention current)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                string DELETE = $"DELETE FROM ConvTable WHERE ConvRef LIKE '{ft_parse(current.Reference)}' AND ClientID LIKE '{current.ClientID}'";
                SqliteCommand cmd = new SqliteCommand(DELETE, con);
                cmd.ExecuteReader();
                con.Close();
            }
        }

        public static void AddConv(Convention current)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();

                string INSERT = $"INSERT INTO ConvTable(ConvDate, ConvRef, ConvServ, ConvFees, ConvAmount, ConvTax, ConvSign, ClientID)" +
                                $"VALUES('{ft_parse(current.Date)}', '{ft_parse(current.Reference)}', '{ft_parse(current.Service)}', '{current.Fees}', '{current.Amount}', " +
                                $"'{current.Tva}', '{current.sign}', '{current.ClientID}')";
                SqliteCommand cmd = new SqliteCommand(INSERT, con);
                cmd.ExecuteReader();
                con.Close();
            }
        }

        public static List<Convention> GetConvs(int ID)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<Convention> list = new List<Convention>();

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM ConvTable WHERE ClientID LIKE '{ID}'";
                SqliteCommand cm = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Convention(get_parse(reader.GetString(0)), get_parse(reader.GetString(1)), get_parse(reader.GetString(2)), reader.GetString(3),
                                            reader.GetString(4), reader.GetString(5), reader.GetInt32(6), reader.GetInt32(7)));
                }
                con.Close();
            }
            return list;
        }
        */
        //AZURE CONV

        public static void CreateAzureConvs()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string init = 
                              "CREATE TABLE IF NOT EXISTS ConvsTable(" +
                              "[ConvDate] [nvarchar](25), " +
                              "[ConvRef] [nvarchar](120), " +
                              "[ConvService] [nvarchar](4000), " +
                              "[ConvFees]  [float], " +
                              "[ConvAmount] [float], " +
                              "[ConvTva] [float], " +
                              "[ConvSign] [int], " +
                              "[ClientID] [int] NOT NULL)";
                SqliteCommand command = new SqliteCommand(init, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzureConvs(int client)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string DEL = $"DELETE FROM ConvsTable WHERE ClientID = {client}";
                SqliteCommand command = new SqliteCommand(DEL, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzureConv(Convention current)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string DEL = $"DELETE FROM ConvsTable WHERE ConvRef LIKE '{ft_parse(current.Reference)}' AND ClientID LIKE '{current.ClientID}'";
                SqliteCommand command = new SqliteCommand(DEL, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void AddAzureConv(Convention current)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = con;
                command.CommandText = "INSERT INTO ConvsTable(ConvDate, ConvRef, ConvService, ConvFees, " +
                                      "ConvAmount, ConvTva, ConvSign, ClientID) " +
                                      "VALUES(@date, @ref, @service, @fees, @amount, @tva, @sign, @id)";

                command.Parameters.AddWithValue("@date", ft_parse(current.Date));
                command.Parameters.AddWithValue("@ref", ft_parse(current.Reference));
                command.Parameters.AddWithValue("@service", ft_parse(current.Service));
                command.Parameters.AddWithValue("@fees", current.Fees);
                command.Parameters.AddWithValue("@amount", current.Amount);
                command.Parameters.AddWithValue("@tva", current.Tva);
                command.Parameters.AddWithValue("@sign", current.sign);
                command.Parameters.AddWithValue("@id", current.ClientID);

                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static List<Convention> GetAzureClientConvs(int id)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<Convention> list = new List<Convention>();

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM ConvsTable WHERE ClientID = '{id}'";
                SqliteCommand command = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    double valeur3 = (double)reader[3];
                    double valeur4 = (double)reader[4];
                    double valeur5 = (double)reader[5];
                    float v3 = Convert.ToSingle(valeur3);
                    float v4 = Convert.ToSingle(valeur4);
                    float v5 = Convert.ToSingle(valeur5);

                    list.Add(new Convention(get_parse(reader.GetString(0)), get_parse(reader.GetString(1)), get_parse(reader.GetString(2)), v3,
                                            v4, v5, reader.GetInt32(6), reader.GetInt32(7)));
                }
                con.Close();
            }
            return list;
        }

        // MANAGE PAYMENT
        /*
        public static void CreatePaymentTable()
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string INIT = "CREATE TABLE IF NOT EXISTS " +
                                "PaymentTable(PayDate NVARCHAR(15), " +
                                "PayAmount NVARCHAR(10), " +
                                "BillRef NVARCHAR(20), " +
                                "ClientID INT NOT NULL)";
                SqliteCommand cm = new SqliteCommand(INIT, con);
                cm.ExecuteReader();
                con.Close();
            }
        }

        public static void RemovePayments(string bill, int id)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string DEL = $"DELETE FROM PaymentTable WHERE BillRef LIKE '{ft_parse(bill)}' AND ClientID LIKE '{id}'";
                SqliteCommand com = new SqliteCommand(DEL, con);
                com.ExecuteReader();
                con.Close();
            }
        }

        public static void RemoveClientsPayments(int ID)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string DEL = $"DELETE FROM PaymentTable WHERE ClientID LIKE '{ID}'";
                SqliteCommand com = new SqliteCommand(DEL, con);
                com.ExecuteReader();
                con.Close();
            }
        }

        public static void RemovePayment(Payment current)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string DEL = $"DELETE FROM PaymentTable WHERE PayDate LIKE '{ft_parse(current.Date)}' AND PayAmount LIKE '{current.Amount}' " +
                    $"AND BillRef LIKE '{ft_parse(current.BillRef)}' AND ClientID LIKE '{current.ClientID}'";
                SqliteCommand com = new SqliteCommand(DEL, con);
                com.ExecuteReader();
                con.Close();
            }
        }

        public static void AddPayment(Payment current)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string INSERT = $"INSERT INTO PaymentTable(PayDate, PayAmount, BillRef, ClientID) " +
                                $"VALUES('{ft_parse(current.Date)}', '{current.Amount}', '{ft_parse(current.BillRef)}', '{current.ClientID}')";
                SqliteCommand com = new SqliteCommand(INSERT, con);
                com.ExecuteReader();
                con.Close();
            }
        }

        public static List<Payment> GetClientPayments(string bill, int ID)
        {
            string pathToDB = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<Payment> list = new List<Payment>();

            using (SqliteConnection con = new SqliteConnection($"Filename={pathToDB}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM PaymentTable WHERE BillRef LIKE '{bill}' AND ClientID LIKE '{ID}'";
                SqliteCommand com = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Payment(get_parse(reader.GetString(0)), reader.GetString(1), get_parse(reader.GetString(2)), reader.GetInt32(3)));
                }
                con.Close();
            }
            return list;
        }
        */
        //AZURE PAYMENTS

        public static void CreateAzurePayments()
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();

                string init =
                              "CREATE TABLE IF NOT EXISTS PaymentTable(" +
                              "[PayDate] [nvarchar](25), " +
                              "[PayAmount] [float], " +
                              "[BillRef] [nvarchar](120), " +
                              "[ClientID] [int] NOT NULL)";
                SqliteCommand command = new SqliteCommand(init, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzurePayments(string bill, int id)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string del = $"DELETE FROM PaymentTable WHERE BillRef LIKE '{ft_parse(bill)}' AND ClientID LIKE '{id}'";
                SqliteCommand command = new SqliteCommand(del, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzureClientsPayments(int id)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string del = $"DELETE FROM PaymentTable WHERE ClientID LIKE '{id}'";
                SqliteCommand command = new SqliteCommand(del, con);
                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void RemoveAzurePayment(Payment current)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string DEL = $"DELETE FROM PaymentTable WHERE PayDate LIKE '{ft_parse(current.Date)}' AND PayAmount LIKE '{current.Amount}' " +
                             $"AND BillRef LIKE '{ft_parse(current.BillRef)}' AND ClientID LIKE '{current.ClientID}'";
                SqliteCommand com = new SqliteCommand(DEL, con);
                com.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void AddAzurePayment(Payment current)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                SqliteCommand command = new SqliteCommand();
                command.Connection = con;
                command.CommandText = "INSERT INTO PaymentTable(Paydate, PayAmount, BillRef, ClientID) " +
                      "VALUES(@date, @amount, @bill, @id)";


                command.Parameters.AddWithValue("@date", ft_parse(current.Date));
                command.Parameters.AddWithValue("@amount", current.Amount);
                command.Parameters.AddWithValue("@bill", ft_parse(current.BillRef));
                command.Parameters.AddWithValue("@id", current.ClientID);

                command.ExecuteNonQuery();
                con.Close();
            }
        }

        public static List<Payment> GetAzurePayments(string bill, int id)
        {
            string connectionString = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db");
            List<Payment> list = new List<Payment>();

            using (SqliteConnection con = new SqliteConnection($"Filename={connectionString}"))
            {
                con.Open();
                string SELECT = $"SELECT * FROM PaymentTable WHERE BillRef LIKE '{bill}' AND ClientID LIKE '{id}'";
                SqliteCommand com = new SqliteCommand(SELECT, con);
                SqliteDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    double valeur = (double)reader[1];
                    float v = Convert.ToSingle(valeur);

                    list.Add(new Payment(get_parse(reader.GetString(0)), v, get_parse(reader.GetString(2)), reader.GetInt32(3)));
                }
                con.Close();
            }
            return list;
        }
    }
}
