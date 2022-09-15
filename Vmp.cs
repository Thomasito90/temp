using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splits
{
    public class Vmp
    {
        // [x] -> x = index in line read from f_vmp_VmpType.csv (DM+D TRUD database)
        private string drugCode; // [0] 
        public string DrugCode
        {
            get { return drugCode; }
            set { drugCode = value; }
        }
        private string invalid; // [4]
        public string Invalid
        {
            get { return invalid; }
            set { invalid = value; }
        }
        private string name; // [5]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private List<Vmpp> vmpps;

        public Vmp(string drugCode, string invalid, string name)
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
            this.vmpps = new List<Vmpp>();
        }

        public override string ToString()
        {
            List<string> vmppsList = new List<string>();
            foreach (Vmpp tempVmpp in this.vmpps)
            {
                vmppsList.Add(tempVmpp.DrugCode);
            }
            return "Name: " + name + " VMP: " + drugCode + " Invalid: " + invalid + " VMPPs: " + string.Join(" ", vmppsList);
        }

        public static Dictionary<string, Vmp> GetVmps()
        {
            Dictionary<string, Vmp> vmps = new Dictionary<string, Vmp>();
            Dictionary<string, List<Vmpp>> vmppDict = Vmpp.GetVmpToVmppsDict();

            using (var reader = new StreamReader(@"C:\Users\Tomasz\source\repos\HelloWorld\splits\f_vmp_VmpType.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    Vmp tempVmp = new Vmp(values[0], values[4], values[5]);
                    if (vmppDict.ContainsKey(tempVmp.DrugCode))
                    {
                        List<Vmpp> tempVmppList = vmppDict[tempVmp.DrugCode];

                        foreach (Vmpp tempVmpp in tempVmppList)
                        {
                            tempVmp.vmpps.Add(tempVmpp);
                        }
                        vmps.Add(tempVmp.DrugCode, tempVmp);

                    }

                }
                reader.Close();
            }
            return vmps;
        }

    }
}

