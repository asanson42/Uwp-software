using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class Payment
    {
        public string Date;
        public float Amount;
        public string BillRef;

        public int ClientID;

        public Payment()
        {
            Date = null;
            Amount = 0;
            BillRef = null;
            ClientID = 0;
        }

        public Payment(string date, float amount, string bref, int id)
        {
            Date = date;
            Amount = amount;
            BillRef = bref;
            ClientID = id;
        }

        public override string ToString()
        {
            return this.Date + this.Amount;
        }
    }
}
