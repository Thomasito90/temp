using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeblisteringJobs
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var medicines = Medicine.GetMedicinesDictionary();
            var medicinesWithUsages = Medicine.GetMedicinesWithUsagesDictionary(medicines);
            var pots = Medicine.GetPotsDictionary();
            var canisters = Medicine.GetCanistersDictionary();
            var medicinesDescription = Medicine.GetDescriptionsDictionary(pots, canisters);

            Medicine.GetStockInPotsCsvFile(medicines);
            Medicine.GetDeblisteringCsvFiles(pots, canisters, medicinesWithUsages, medicinesDescription);

        }
    }
}
