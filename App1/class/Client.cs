using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Services.Maps;
using Windows.Storage.AccessCache;

namespace App1
{
    public class Client
    {
        public string LastName;
        public string FirstName;
        public string Address;
        public string PostalCode;
        public string City;
        public string Country;
        public string Phone;
        public string Email;

        public string Reference;

        public int ClientID;

        public List<Convention> ClientConvs;
        public List<Bill> ClientBills;
        public List<Payment> payments;

        public Client()
        {
            LastName = null;
            FirstName = null;
            Address = null;
            PostalCode = null;
            City = null;
            Country = null;
            Phone = null;
            Email = null;

            Reference = null;

            ClientID = 0;

            ClientConvs = new List<Convention>();
            ClientBills = new List<Bill>();
        }

        public Client(string lastname, string firstname, string address, string postalcode, string city, string country, string phone, string email, string reference, int id)
        {
            LastName = lastname;
            FirstName = firstname;
            Address = address;
            PostalCode = postalcode;
            City = city;
            Country = country;
            Phone = phone;
            Email = email;
            Reference = reference;
            ClientID = id;
            ClientConvs = new List<Convention>();
            ClientBills = new List<Bill>();
            payments = new List<Payment>();
        }

        public override string ToString()
        {
            return this.LastName + " " + this.FirstName;
        }
    }
}
