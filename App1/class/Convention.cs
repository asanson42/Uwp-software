using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text.Core;

namespace App1
{
    public class Convention
    {
        public string Date;
        public string Reference;

        public string Service;

        public float Fees;
        public float Amount;
        public float Tva;

        public int sign;

        public Client current;

        public int ClientID;

        public Convention()
        {
            Date = null;
            Reference = null;
            Service = null;
            Fees = 0;
            Amount = 0;
            Tva = 0;
            sign = -1;
            current = null;
            ClientID = 0;
        }

        public Convention(string date, string reference, string service, float fees, float amount, float tva, int _sign, int ID)
        {
            Date = date;
            Reference = reference;
            Service = service;
            Fees = fees;
            Amount = amount;
            Tva = tva;
            if (_sign > 0)
                sign = 1;
            else
                sign = -1;
            ClientID = ID;
        }

        public override string ToString()
        {
            return this.Date + " - ref. " + this.Reference;
        }
    }
}
