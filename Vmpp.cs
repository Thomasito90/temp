using System.Collections.Generic;
using System.IO;

namespace splits
{
    public class Vmpp
    {
        // [x] -> x = index in line read from f_vmpp_VmppType.csv (DM+D TRUD database)
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
        private string vmp; // [4]
        public string Vmp
        {
            get { return vmp; }
            set { this.vmp = value; }
        }
        private string qtyval; // [5]
        public string Qtyval
        {
            get { return qtyval; }
            set { this.qtyval = value; }
        }
        public Vmpp(string drugCode, string invalid, string name, string vmp, string qtyval)
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
            this.vmp = vmp;
            this.qtyval = qtyval;
        }

        public override string ToString()
        {
            return "Name: " + name + " VMPP: " + drugCode + " Invalid: " + invalid + " VMP: " + vmp + " QTY: " + qtyval;
        }

        public static List<Vmpp> GetVmpps()
        {
            List<Vmpp> vmpps = new List<Vmpp>();

            using (var reader = new StreamReader(@"C:\Users\Tomasz\source\repos\HelloWorld\splits\f_vmpp_VmppType.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    Vmpp tempVmpp = new Vmpp(values[0], values[1], values[2], values[4], values[5]);
                    if(tempVmpp.Invalid == "No")
                    {
                        vmpps.Add(tempVmpp);
                    }                     
                }
                reader.Close();
            }
            return vmpps;
        }

        public static Dictionary<string, List<Vmpp>> GetVmpToVmppsDict()
        {
            Dictionary<string, List<Vmpp>> vmpToVmppsDict = new Dictionary<string, List<Vmpp>>();

            using (var reader = new StreamReader(@"C:\Users\Tomasz\source\repos\HelloWorld\splits\f_vmpp_VmppType.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    Vmpp tempVmpp = new Vmpp(values[0], values[1], values[2], values[4], values[5]);
                    string vmp = values[4];

                    if (!(vmpToVmppsDict.ContainsKey(vmp)))
                    {
                        List<Vmpp> tempVmppList = new List<Vmpp>();
                        if (tempVmpp.Invalid == "No")
                        {
                            tempVmppList.Add(tempVmpp);
                            vmpToVmppsDict.Add(vmp, tempVmppList);
                        }
                                                
                    }
                    else
                    {
                        if (tempVmpp.Invalid == "No")
                        {
                            vmpToVmppsDict[vmp].Add(tempVmpp);
                        }
                        
                    }

                }


            }
            return vmpToVmppsDict;
        }

        public static Dictionary<string, string> GetVmppToQtyDict()
        {
            Dictionary<string, string> vmppToQtyDict = new Dictionary<string, string>();

            using (var reader = new StreamReader(@"C:\Users\Tomasz\source\repos\HelloWorld\splits\f_vmpp_VmppType.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    Vmpp tempVmpp = new Vmpp(values[0], values[1], values[2], values[4], values[5]);
                    string vmpp = values[0];

                    if (!(vmppToQtyDict.ContainsKey(vmpp)))
                    {
                        string tempVmppQty = "";
                        tempVmppQty = tempVmpp.Qtyval;
                        vmppToQtyDict.Add(vmpp, tempVmppQty);
                    }
                }


            }
            return vmppToQtyDict;
        }

    }
}
