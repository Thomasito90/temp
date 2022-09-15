using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace splits
{
    public class Amp
    {
        // [x] -> x = index in line read from f_amp_AmpType.csv (DM+D TRUD database)
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
        private string vmp; // [2]
        public string Vmp
        {
            get { return vmp; }
            set { this.vmp = value; }
        }
        private string name; // [5]
        public string Name
        {
            get { return name; }
            set { this.name = value; }
        }
        private List<Ampp> ampps;
        public List<Ampp> Ampps
        {
            get { return ampps; }
            set { ampps = value; }
        }

        public Amp(string drugCode, string invalid, string vmp, string name)
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

            this.vmp = vmp;
            this.name = name;
            this.ampps = new List<Ampp>();
        }

        public override string ToString()
        {
            List<string> amppsList = new List<string>();
            foreach (Ampp tempAmpp in this.ampps)
            {
                amppsList.Add(tempAmpp.DrugCode);
                amppsList.Add(tempAmpp.Qtyval);
            }
            return "Name: " + name + " AMP: " + drugCode + " Invalid: " + invalid + " VMP: " + vmp + " AMPPs: " + string.Join(" ", amppsList);
        }

        public static Dictionary<string, Amp> GetAmps()
        {
            Dictionary<string, List<Ampp>> amppDict = Ampp.GetAmpToAmppsDict();
            Dictionary<string, Amp> amps = new Dictionary<string, Amp>();

            using (var reader = new StreamReader(@"C:\Users\Tomasz\source\repos\HelloWorld\splits\f_amp_AmpType.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('|');

                    Amp tempAmp = new Amp(values[0], values[1], values[2], values[5]);

                    if (amppDict.ContainsKey(tempAmp.DrugCode))
                    {
                        List<Ampp> tempAmppList = amppDict[tempAmp.DrugCode];

                        foreach (Ampp tempAmpp in tempAmppList)
                        {
                            tempAmp.ampps.Add(tempAmpp);
                        }
                        amps.Add(tempAmp.DrugCode, tempAmp);

                    }

                }
                reader.Close();
            }
            return amps;
        }

    }
}


