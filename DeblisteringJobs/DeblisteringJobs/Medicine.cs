using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeblisteringJobs
{
    internal class Medicine
    {
        // JSON files requierd as input:

        private const string MedicinesFilePath = @"C:\Users\ThomasMacheychuk\Desktop\Power BI\Deblistering updates\Medicines-2022-10-14.json";
        private const string MedContainersFilePath = @"C:\Users\ThomasMacheychuk\Desktop\Power BI\Deblistering updates\MedContainers-2022-10-14.json";
        private const string CanistersFilePath = @"C:\Users\ThomasMacheychuk\Desktop\Power BI\Deblistering updates\Canisters-Status-2022-10-14.json";

        private string identifier; // AMP/VMP/Rowa Drug Code
        public string Identifier { get { return identifier; } set { identifier = value; } }

        private string drugName;
        public string DrugName { get { return drugName; } set { drugName = value; } }

        private string description;
        public string Description { get { return description; } set { description = value; } }

        private float pillsInCanisters; 
        public float PillsInCanisters { get { return pillsInCanisters; } set { pillsInCanisters = value; } }

        private float pillsInPots; 
        public float PillsInPots { get { return pillsInPots; } set { pillsInPots = value; } }

        private int dispensingCounter;
        public int DispensingCounter { get { return dispensingCounter; } set { dispensingCounter = value; } }

        private float usage; // currently 56 days usage from SalesForce robots usages report
        public float Usage { get { return usage; } set { usage = value; } }
        public float Usage28 { get { return usage / 2; } }
        public float Usage14 { get { return usage / 4; } }
        public float Usage7 { get { return usage / 8; } }
        public float Usage1 { get { return usage / 56; } }

        private bool hasCanister;
        public bool HasCanister { get { return hasCanister; } set { hasCanister = value; } }

        private bool active;
        public bool Active { get { return active; } set { active = value; } }

        public float TotalPills { get { return pillsInPots + pillsInCanisters; } }

        private int dispensed1, dispensed7, dispensed14, dispensed28, dispensed56;

        public int Dispensed1 { get { return dispensed1; } set { dispensed1 = value; } }
        public int Dispensed7 { get { return dispensed7; } set { dispensed7 = value; } }
        public int Dispensed14 { get { return dispensed14; } set { dispensed14 = value; } }
        public int Dispensed28 { get { return dispensed28; } set { dispensed28 = value; } }
        public int Dispensed56 { get { return dispensed56; } set { dispensed56 = value; } }



        public Medicine(string identifier, string drugName, string description, bool active)
        {
            this.identifier = identifier;
            this.drugName = drugName;
            this.description = description;
            this.active = active;
            this.usage = 0;
            this.hasCanister = false;
        }

        public override string ToString()
        {
            return "Drug code: " + identifier + "\n" +
                "Drug name: " + drugName + "\n" +
                "Description: " + description + "\n" +
                "Has canister: " + hasCanister + "\n" +
                "Pills in canisters: " + pillsInCanisters + "\n" +
                "Pills in pots: " + pillsInPots + "\n" +
                "Total pills in stock: " + TotalPills + "\n" +
                "56 days usage: " + usage + "\n" +
                "Dispensing counter: " + dispensingCounter + "\n" +
                "Dispensed in last 24 hours: " + dispensed1 + "\n" +
                "Dispensed in last 1-7 days: " + dispensed7 + "\n" +
                "Dispensed in last 7-14 days: " + dispensed14 + "\n" +
                "Dispensed in last 14-28 days: " + dispensed28 + "\n" +
                "Dispensed in last 28-56 days: " + dispensed56;
        }

        public static Dictionary<string, Medicine> GetMedicinesDictionary()
        {
            Dictionary<string, Medicine> medicines = new Dictionary<string, Medicine>();
            Dictionary<string, float> pots = GetPotsDictionary();
            Dictionary<string, float> canisters = GetCanistersDictionary();
            Dictionary<string, DrugUsage> drugUsages = DrugUsage.GetUsagesDictionary();

            StreamReader medicinesJsonFile = System.IO.File.OpenText(MedicinesFilePath);
            
            JsonTextReader reader = new JsonTextReader(medicinesJsonFile);

            JToken medicinesJson = JToken.ReadFrom(reader);

            foreach (JObject medicine in medicinesJson)
            {
                Medicine tempMedicine = new Medicine((string)medicine["Identifier"], (string)medicine["Name"],
                    (string)medicine["Description"], (bool)medicine["Active"]);

                if (pots.ContainsKey(tempMedicine.Identifier))
                {
                    tempMedicine.PillsInPots = pots[tempMedicine.Identifier];
                }

                if(canisters.ContainsKey(tempMedicine.Identifier))
                {
                    tempMedicine.PillsInCanisters = canisters[tempMedicine.Identifier];
                    tempMedicine.HasCanister = true;
                }
                    
                if(drugUsages.ContainsKey(tempMedicine.Identifier))
                {
                    tempMedicine.Usage = drugUsages[tempMedicine.Identifier].Usage56;
                    tempMedicine.DispensingCounter = drugUsages[tempMedicine.Identifier].DispensingCounter;
                    tempMedicine.Dispensed1 = drugUsages[tempMedicine.Identifier].Dispensed1;
                    tempMedicine.Dispensed7 = drugUsages[tempMedicine.Identifier].Dispensed7;
                    tempMedicine.Dispensed14 = drugUsages[tempMedicine.Identifier].Dispensed14;
                    tempMedicine.Dispensed28 = drugUsages[tempMedicine.Identifier].Dispensed28;
                    tempMedicine.Dispensed56 = drugUsages[tempMedicine.Identifier].Dispensed56;
                }

                medicines.Add(tempMedicine.Identifier, tempMedicine);
            }

            return medicines;
        }

        public static Dictionary<string, Medicine> GetDescriptionsDictionary(Dictionary<string, float> pots, Dictionary<string, float> canisters)
        {
            Dictionary<string, Medicine> medicinesDescriptions = new Dictionary<string, Medicine>();

            StreamReader medicinesJsonFile = System.IO.File.OpenText(MedicinesFilePath);

            JsonTextReader reader = new JsonTextReader(medicinesJsonFile);

            JToken medicinesJson = JToken.ReadFrom(reader);

            foreach (JObject medicine in medicinesJson)
            {
                Medicine tempMedicine = new Medicine((string)medicine["Identifier"], (string)medicine["Name"],
                    (string)medicine["Description"], (bool)medicine["Active"]);

                if (pots.ContainsKey(tempMedicine.Identifier))
                {
                    tempMedicine.PillsInPots += pots[tempMedicine.Identifier];                 
                }

                if (canisters.ContainsKey(tempMedicine.Identifier))
                {
                    tempMedicine.PillsInCanisters = canisters[tempMedicine.Identifier];
                    tempMedicine.HasCanister = true;
                }

                if (medicinesDescriptions.ContainsKey(tempMedicine.Description))
                {
                    medicinesDescriptions[tempMedicine.Description].PillsInCanisters += tempMedicine.PillsInCanisters;
                    medicinesDescriptions[tempMedicine.Description].PillsInPots += tempMedicine.PillsInPots;
                }
                else
                {
                    medicinesDescriptions.Add(tempMedicine.Description, tempMedicine);
                }
            }

            return medicinesDescriptions;
        }


        public static Dictionary<string, Medicine> GetMedicinesWithUsagesDictionary(Dictionary<string, Medicine> medicines)
        {
            Dictionary<string, Medicine> medicinesWithUsages = new Dictionary<string, Medicine>();

            foreach(KeyValuePair<string, Medicine> kvp in medicines)
            {
                Medicine tempMedicine = kvp.Value;

                if (tempMedicine.Usage != 0 && tempMedicine.Active == true)
                {
                    medicinesWithUsages.Add(tempMedicine.Identifier, tempMedicine);
                }
            }

            return medicinesWithUsages;
        }

        public static List<Medicine> GetMedicinesWithUsagesWithoutCanisters(Dictionary<string, Medicine> medicines)
        {
            List<Medicine> medicinesWithUsagesWithoutCanisters = new List<Medicine>();

            foreach(KeyValuePair<string, Medicine> kvp in medicines)
            {
                Medicine medicine = kvp.Value;

                if (medicine.Usage > 0 && medicine.HasCanister == false)
                {
                    medicinesWithUsagesWithoutCanisters.Add(medicine);
                }
            }

            medicinesWithUsagesWithoutCanisters = medicinesWithUsagesWithoutCanisters.OrderByDescending(x => x.Usage).ToList();

            return medicinesWithUsagesWithoutCanisters;
        }

        public static Dictionary<string, float> GetPotsDictionary()
        {
            Dictionary<string, float> pots = new Dictionary<string, float>();

            StreamReader medContainersJsonFile = System.IO.File.OpenText(MedContainersFilePath);
            
            JsonTextReader reader = new JsonTextReader(medContainersJsonFile);

            JToken medContainersJson = JToken.ReadFrom(reader);

            foreach (JObject medContainer in medContainersJson)
            {
                string identifier = (string)medContainer["MedicineId"];
                float pillsInPot = (float)medContainer["Quantity"];
                
                if (pots.ContainsKey(identifier))
                {
                    pots[identifier] += pillsInPot;
                } else
                {
                    pots.Add(identifier, pillsInPot);
                }
            }

            return pots;
        }

        public static Dictionary<string, float> GetCanistersDictionary()
        {
            Dictionary<string, float> canisters = new Dictionary<string, float>();

            StreamReader canistersJsonFile = System.IO.File.OpenText(CanistersFilePath);

            JsonTextReader reader = new JsonTextReader(canistersJsonFile);

            JToken canistersJson = JToken.ReadFrom(reader);

            foreach (JObject canister in canistersJson)
            {
                string identifier = (string)canister["MedicineId"];
                float pillsInCanister = (float)canister["AmountLeft"];

                if (identifier != null)
                {
                    if (canisters.ContainsKey(identifier))
                    {
                        canisters[identifier] += pillsInCanister;
                    }
                    else
                    {
                        canisters.Add(identifier, pillsInCanister);
                    }
                }
                
            }

            return canisters;
        }

        public static void GetStockInPotsCsvFile(Dictionary<string, Medicine> medicines)
        {
            List<Medicine> stockInPots = new List<Medicine>();

            foreach (KeyValuePair<string, Medicine> kvp in medicines)
            {
                Medicine medicine = kvp.Value;
                stockInPots.Add(medicine);
            }

            stockInPots = stockInPots.OrderByDescending(x => x.PillsInPots).ToList();

            Console.WriteLine(stockInPots.Count);

            using (FileStream fs = new FileStream("Stock.csv", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("Drug name, Number of pills in pots, Number of pills in canisters, Number of pills in stock");

                    foreach(Medicine medicine in stockInPots)
                    {
                        if (!(medicine.Identifier.Contains("C") || medicine.Identifier.Contains("VIT") || medicine.Identifier.Contains("TicTac")
                            || medicine.Identifier.Contains("Identifier") || medicine.Identifier.Contains("MG")))
                        {
                            sw.WriteLine(medicine.DrugName.Replace(",", ".") + "," + medicine.PillsInPots + "," + medicine.PillsInCanisters
                                + "," + medicine.TotalPills);
                        }
                    }
                    
                }
            }
        }



        public static void GetDeblisteringCsvFiles(
            Dictionary<string, float> pots, Dictionary<string, float> canisters, 
            Dictionary<string, Medicine> medicinesWithUsages, Dictionary<string, Medicine> medicinesDescription)
        {
            List<Medicine> deblisteringJobs1 = new List<Medicine>();
            List<Medicine> deblisteringJobs7 = new List<Medicine>();
            List<Medicine> deblisteringJobs14 = new List<Medicine>();

            foreach (KeyValuePair<string, Medicine> kvp in medicinesWithUsages)
            {
                Medicine medicine = kvp.Value;

                if (medicinesDescription[medicine.Description].TotalPills <= medicine.Usage1)
                {
                    if (medicine.HasCanister == true)
                    {
                        deblisteringJobs1.Add(medicine);
                    }
                }

                else if (medicinesDescription[medicine.Description].TotalPills <= medicine.Usage7)
                {
                    deblisteringJobs7.Add(medicine);
                }

                else if (medicinesDescription[medicine.Description].TotalPills <= medicine.Usage14)
                {
                    deblisteringJobs14.Add(medicine);
                }
            }

            deblisteringJobs1 = deblisteringJobs1.OrderBy(x => x.TotalPills / x.Usage1).ThenByDescending(x => x.Usage1).ToList();
            deblisteringJobs7 = deblisteringJobs7.OrderBy(x => x.TotalPills / x.Usage1).ThenByDescending(x => x.Usage1).ToList();
            deblisteringJobs14 = deblisteringJobs14.OrderBy(x => x.TotalPills / x.Usage1).ThenByDescending(x => x.Usage1).ToList();

            Console.WriteLine("1 day jobs: " + deblisteringJobs1.Count);
            Console.WriteLine("7 days jobs: " + deblisteringJobs7.Count);
            Console.WriteLine("14 days jobs: " + deblisteringJobs14.Count);

            Console.WriteLine();

            using (FileStream fs = new FileStream("RobotsStockSummary.csv", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {

                    sw.WriteLine("MedicineID, Drug name, Pills in pots, Pills in canisters, Total pills, 1-day usage, 56-day usage," +
                        " Quantity to deblister, Has canister?, Dispensing counter, Dispensing counter 0-1 days, Dispensing counter 1-7 days," +
                        " Dispensing counter 7-14 days, Dispensing counter 14-28 days, Dispensing counter 28-56 days");

                    foreach (Medicine job in deblisteringJobs1)
                    {   sw.WriteLine(job.Identifier + "," + job.DrugName + "," + job.PillsInPots + "," + job.PillsInCanisters + "," +
                            job.TotalPills + "," + job.Usage1 + "," + job.Usage + "," + (int)job.Usage / 2 + "," + job.HasCanister + "," +
                            job.DispensingCounter + "," + job.Dispensed1 + "," + job.Dispensed7 + "," +
                            job.Dispensed14 + "," + job.Dispensed28 + "," + job.Dispensed56);
                    }

                    foreach (Medicine job in deblisteringJobs7)
                    {
                        sw.WriteLine(job.Identifier + "," + job.DrugName + "," + job.PillsInPots + "," + job.PillsInCanisters + "," +
                            job.TotalPills + "," + job.Usage1 + "," + job.Usage + "," + (int)job.Usage / 2 + "," + job.HasCanister + "," +
                            job.DispensingCounter + "," + job.Dispensed1 + "," + job.Dispensed7 + "," +
                            job.Dispensed14 + "," + job.Dispensed28 + "," + job.Dispensed56);
                    }

                    foreach (Medicine job in deblisteringJobs14)
                    {
                        sw.WriteLine(job.Identifier + "," + job.DrugName + "," + job.PillsInPots + "," + job.PillsInCanisters + "," +
                            job.TotalPills + "," + job.Usage1 + "," + job.Usage + "," + (int)job.Usage / 2 + "," + job.HasCanister + "," +
                            job.DispensingCounter + "," + job.Dispensed1 + "," + job.Dispensed7 + "," +
                            job.Dispensed14 + "," + job.Dispensed28 + "," + job.Dispensed56);
                    }
                }
            }


            using (FileStream fs = new FileStream("DeblisteringJobs1.csv", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {

                    sw.WriteLine("MedName, Quantity to deblister, Status (TO DO/WIP/DONE), 1 day usage, Pills in pots, " +
                        "Pills in canisters, Total pills, Has canister?");

                    foreach (Medicine job in deblisteringJobs1)
                    {
                        sw.WriteLine(job.DrugName.Replace(",", ".") + "," + (int)job.Usage / 2 + "," + "," + (int)job.Usage1 + "," + job.PillsInPots +
                            "," + job.PillsInCanisters + "," + job.TotalPills + "," + job.HasCanister);
                    }
                }
            }

            using (FileStream fs = new FileStream("DeblisteringJobs7.csv", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {

                    sw.WriteLine("MedName, Quantity to deblister, Status (TO DO/WIP/DONE), 1 day usage, Pills in pots, " +
                        "Pills in canisters, Total pills, Has canister?");

                    foreach (Medicine job in deblisteringJobs7)
                    {
                        sw.WriteLine(job.DrugName.Replace(",", ".") + "," + (int)job.Usage / 2 + "," + "," + (int)job.Usage1 + "," + job.PillsInPots +
                            "," + job.PillsInCanisters + "," + job.TotalPills + "," + job.HasCanister);
                    }
                }
            }

            using (FileStream fs = new FileStream("DeblisteringJobs14.csv", FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {

                    sw.WriteLine("MedName, Quantity to deblister, Status (TO DO/WIP/DONE), 1 day usage, Pills in pots, " +
                        "Pills in canisters, Total pills, Has canister?");

                    foreach (Medicine job in deblisteringJobs14)
                    {
                        sw.WriteLine(job.DrugName.Replace(",", ".") + "," + (int)job.Usage / 2 + "," + "," + (int)job.Usage1 + "," + job.PillsInPots +
                            "," + job.PillsInCanisters + "," + job.TotalPills + "," + job.HasCanister);
                    }
                }
            }
        }
    }
}
