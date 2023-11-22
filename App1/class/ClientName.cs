using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace App1
{
    public class ClientName
    {
        public string nom { get; set; }
        public string prenom { get; set; }

        public ClientName()
        {
            nom = null;
            prenom = null;
        }
        public ClientName(string last, string first)
        {
            nom = last;
            prenom = first;
        }

        public override string ToString()
        {
            return this.nom + " " + this.prenom;
        }
    }
}
