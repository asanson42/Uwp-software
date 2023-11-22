using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace App1
{
    public class Bill
    {
        public string Date;
        public string Reference;

        public string Service;

        public float Hour;
        public float Tva;
        public float TTC;
        public float THT;

        public int Conv;
        public string convRef;
        public Convention Convention;

        public List<Payment> Payments;

        public int ClientID;

        public Bill()
        {
            Date = null;
            Reference = null;
            Service = null;
            Hour = 0;
            Tva = 0;
            TTC = 0;
            THT = 0;
            Conv = 0;
            Convention = new Convention();
            Payments = new List<Payment>();

            ClientID = 0;
        }

        public Bill(string date, string reference, string services, float hour, float tax, float ttc, float tht, int conv, string crf, int ID)
        {
            Date = date;
            Reference = reference;
            Service = services;
            Hour = hour;
            Tva = tax;
            TTC = ttc;
            THT = tht;
            convRef = crf;
            ClientID = ID;

            if (conv == 1)
            {
                Conv = 1;
                Convention = new Convention();
                foreach(Convention current in App.User.curClient.ClientConvs)
                {
                    if (current.Reference.Equals(convRef))
                        Convention = current;
                }
            }
            else
            {
                Conv = 0;
                Convention = new Convention();
            }
        }

        public override string ToString()
        {
            return this.Date + " - ref. " + this.Reference;
        }
    }
}
