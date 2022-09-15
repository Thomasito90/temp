using System.Collections.Generic;
using System.IO;

namespace splits
{
    public class Ampp
    {
        // [x] -> x = index in line read from f_ampp_AmppType.csv (DM+D TRUD database)
        private string drugCode; // [0]
        public string DrugCode
        {
            get { return drugCode; }
            set { this.drugCode = value; }
        }
        private string invalid; // [1]
        public string Invalid
        {
            get { return invalid; }
            set { this.invalid = value; }
        }
        private string name; // [2]
        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }
        private string vmppCode; // [4]
        public string VmppCode
        {
            get { return vmppCode; }
            set { this.vmppCode = value; }
        }
        private string amp; // [5]
        public string Amp
        {
            get { return amp; }
            set { this.amp = value; }
        }
        private string discontinued; // [9]
        public string Discontinued
        {
            get { return discontinued; }
            set { this.discontinued = value; }
        }
        private string qtyval;
        public string Qtyval
        {
            get { return qtyval; }
            set { this.qtyval = value; }
        }

        public Ampp(string drugCode, string invalid, string name, string vmppCode, string amp, string discontinued)
        {
            this.drugCode = drugCode;

            if (invalid == "")
            {
                this.invalid = "No";
            }
            else
            {
                this.invalid = "Yes";
            }

            this.name = name;
            this.vmppCode = vmppCode;
            this.amp = amp;

            if (discontinued == "" || discontinued == "0000")
            {
                this.discontinued = "No";
            }
            else
            {
                this.discontinued = "Yes";
            }
            this.qtyval = "";

        }

        public override string ToString()
        {
            return "Name: " + name + " AMPP: " + drugCode + " Invalid: " + invalid + " VMPP: " + vmppCode + " AMP: " + amp + " Discontinued: " 
                + discontinued + " QTY: " + qtyval;
        }

        public static List<Ampp> GetAmppsList()
        {
            List<Ampp> amppsList = new List<Ampp>();
            Dictionary<string, string> vmppToQtyDict = Vmpp.GetVmppToQtyDict();

            using (var reader = new StreamReader(@"C:\Users\Tomasz\source\repos\HelloWorld\splits\f_ampp_AmppType.csv"))
            {
                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    Ampp tempAmpp = new Ampp(values[0], values[1], values[2], values[4], values[5], values[9]);
                    tempAmpp.Qtyval = vmppToQtyDict[tempAmpp.VmppCode];
                    if (tempAmpp.Invalid == "No")
                    {
                        amppsList.Add(tempAmpp);
                    }
                    
                }
                reader.Close();
            }
            return amppsList;
        }

        public static Dictionary<string, List<Ampp>> GetAmpToAmppsDict()
        {
            Dictionary<string, List<Ampp>> ampToAmppsDict = new Dictionary<string, List<Ampp>>();
            Dictionary<string, string> vmppToQtyDict = Vmpp.GetVmppToQtyDict();

            using (var reader = new StreamReader(@"C:\Users\Tomasz\source\repos\HelloWorld\splits\f_ampp_AmppType.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    Ampp tempAmpp = new Ampp(values[0], values[1], values[2], values[4], values[5], values[9]);
                    string amp = values[5];

                    if (tempAmpp.Invalid == "No")
                    {
                        if (!(ampToAmppsDict.ContainsKey(amp)))
                        {
                            List<Ampp> tempAmppList = new List<Ampp>();
                            tempAmppList.Add(tempAmpp);
                            tempAmpp.Qtyval = vmppToQtyDict[tempAmpp.VmppCode];
                            ampToAmppsDict.Add(amp, tempAmppList);
                        }
                        else
                        {
                            tempAmpp.Qtyval = vmppToQtyDict[tempAmpp.VmppCode];
                            ampToAmppsDict[amp].Add(tempAmpp);
                        }
                    }
                    

                }


            }
            return ampToAmppsDict;
        }
    }
}
