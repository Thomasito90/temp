using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

// C:\Users\Tomasz\Desktop\deblistering_jobs

// report1663415533689.csv

// csv columns:
// 0: Dispatched On     1: Product Name     2:Quantity	    3: Rowa Drug Code  
// 4: Product Code  5: Drug Code    6: Stock Level ID   7: Prescription Token Item

// Using: Product Name, Quantity, Rowa Drug Code, Stock Level ID

namespace DeblisteringJobs
{
    public class DrugUsage
    {
        // SalesForce usages csv report required as input: 

        private const string UsagesFilePath = @"C:\Users\ThomasMacheychuk\Desktop\Power BI\Deblistering updates\usages.csv";

        private string productName, rowaDrugCode;
        private float usage56;
        private int dispensingCounter, dispensed1, dispensed7, dispensed14, dispensed28, dispensed56;
        private DateTime timeStamp;
        public string ProductName { get { return productName; } set { productName = value; } }
        public string RowaDrugCode { get { return rowaDrugCode; } set { rowaDrugCode = value; } }
        public float Usage56 { get { return usage56; } set { usage56 = value; } }
        public float Usage28 { get { return usage56 / 2; } }
        public float Usage14 { get { return usage56 / 4; } }
        public float Usage7 { get { return usage56 / 8; } }
        public float Usage1 { get { return usage56 / 56; } }
        public int DispensingCounter { get { return dispensingCounter; } set { dispensingCounter = value; } }
        public DateTime TimeStamp { get { return timeStamp; } set { timeStamp = value; } }
        public int Dispensed1 { get { return dispensed1; } set { dispensed1 = value; } }
        public int Dispensed7 { get { return dispensed7; } set { dispensed7 = value; } }
        public int Dispensed14 { get { return dispensed14; } set { dispensed14 = value; } }
        public int Dispensed28 { get { return dispensed28; } set { dispensed28 = value; } }
        public int Dispensed56 { get { return dispensed56; } set { dispensed56 = value; } }

        public DrugUsage(string productName, float usage56, string rowaDrugCode, DateTime timeStamp)
        {
            this.productName = productName;
            this.usage56 = usage56;
            this.rowaDrugCode = rowaDrugCode;
            this.dispensingCounter = 1;
            this.timeStamp = timeStamp;
        }
        public override string ToString()
        {
            return "Drug name: " + productName + " Rowa drug code: " + rowaDrugCode + 
                " 56 days usage: " + usage56 + " 1 day usage: " + Usage1;
        }
        public static Dictionary<string, DrugUsage> GetUsagesDictionary()
        {
            Dictionary<string, DrugUsage> usages = new Dictionary<string, DrugUsage>();
            DateTime timeNow = DateTime.Now;

            using (var reader = new StreamReader(UsagesFilePath))
            {
                int counter = 0;
                while (!reader.EndOfStream)
                {
                    counter += 1;

                    var line = reader.ReadLine();

                    line = line.Replace(",000unit", ".000unit");
                    line = line.Replace(",200unit", ".200unit");
                    line = line.Replace("3,2", "3.2");
                    line = line.Replace("\"", "");

                    var values = line.Split(',');

                    if (counter == 1) { continue; } // skipping .csv header

                    DrugUsage tempDrug = new DrugUsage(values[1], float.Parse(values[2]), values[3], DateTime.Parse(values[0]));

                    TimeSpan howManyDays = timeNow.Subtract(tempDrug.TimeStamp);

                    if (howManyDays.Days <= 1)
                    {
                        tempDrug.Dispensed1 = 1;
                    }

                    else if (howManyDays.Days > 1 && howManyDays.Days <= 7)
                    {
                        tempDrug.Dispensed7 = 1;
                    }

                    else if (howManyDays.Days > 7 && howManyDays.Days <= 14)
                    {
                        tempDrug.Dispensed14 = 1;
                    }

                    else if (howManyDays.Days > 14 && howManyDays.Days <= 28)
                    {
                        tempDrug.Dispensed28 = 1;
                    }

                    else if (howManyDays.Days > 28 && howManyDays.Days <= 56)
                    {
                        tempDrug.Dispensed56 = 1;
                    }

                    if (usages.ContainsKey(tempDrug.RowaDrugCode))
                    {

                        try
                        {
                            usages[tempDrug.RowaDrugCode].Usage56 += tempDrug.Usage56;
                            usages[tempDrug.RowaDrugCode].DispensingCounter += 1;
                            usages[tempDrug.RowaDrugCode].Dispensed1 += tempDrug.Dispensed1;
                            usages[tempDrug.RowaDrugCode].Dispensed7 += tempDrug.Dispensed7;
                            usages[tempDrug.RowaDrugCode].Dispensed14 += tempDrug.Dispensed14;
                            usages[tempDrug.RowaDrugCode].Dispensed28 += tempDrug.Dispensed28;
                            usages[tempDrug.RowaDrugCode].Dispensed56 += tempDrug.Dispensed56;
                        }
                        catch (FormatException e)
                        {
                            WriteLine(e.Message);
                            ReadLine();
                        }

                    }
                    else
                    {
                        usages.Add(tempDrug.RowaDrugCode, tempDrug);
                    }


                }
                reader.Close();

            }
            return usages;
        }


        public static List<DrugUsage> SortedUsages()
        {
            List<DrugUsage> usages = new List<DrugUsage>();
            List<DrugUsage> usagesSorted = new List<DrugUsage>();

            Dictionary<string, DrugUsage> usagesDict = GetUsagesDictionary();

            foreach (KeyValuePair<string, DrugUsage> kvp in usagesDict)
            {
                DrugUsage tempDrug = kvp.Value;
                usages.Add(tempDrug);
            }

            usagesSorted = usages.OrderByDescending(x => x.Usage56).ToList();

            return usagesSorted;
        }

    }
}
