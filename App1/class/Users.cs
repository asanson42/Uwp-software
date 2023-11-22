using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class Users
    {
        public float TVA;
        public string BillBegin;
        public string BillEnd;
        public string ConvRules;

        public float ConvNum;
        public float BillNum;
        public float ClientNum;

        public Users()
        {
            TVA = 0;
            BillBegin = "";
            BillEnd = "";
            ConvRules = "";
            ConvNum = 0;
            BillNum = 0;
            ClientNum = 0;
        }

        public Users(
            float tVA, string billBegin, string billEnd, string convRules)
        {
            TVA = tVA;
            BillBegin = billBegin;
            BillEnd = billEnd;
            ConvRules = convRules;
        }

        public Users( float tva, string billbegin, string billend, string convrules, float conv, float cli, float bill)
        {
            TVA = tva;
            BillBegin = billbegin;
            BillEnd = billend;
            ConvRules = convrules;

            ConvNum = conv;
            ClientNum = cli;
            BillNum = bill;
        }

    }
}
