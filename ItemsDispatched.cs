using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace splits
{
    public class ItemsDispatched
    {
        // [x] -> x = index in line read from itemsDispatched.csv (SalesForce items dispatched report - last 90 days, no pouches, only extras)
        private string name;// [0]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string quantity; // [1]
        public string Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        private string drugCode; // [6]
        public string DrugCode {
            get { return drugCode; }
            set { drugCode = value; }
        }

        public ItemsDispatched(string name, string quantity, string drugCode)
        {
            this.name = name;

            if (quantity == "")
            {
                this.quantity = "0";
            } else
            {
                this.quantity = quantity;
            }

            this.drugCode = drugCode;
        }

        public override string ToString()
        {
            return "Name: " + name + " Quantity: " + quantity + " Drug code: " + drugCode;
        }

        public static List<ItemsDispatched> GetItemsDispatched()
        {
            List<ItemsDispatched> itemsDispatched = new List<ItemsDispatched>();

            using (var reader = new StreamReader(@"C:\Users\Tomasz\source\repos\HelloWorld\splits\itemsDispatched.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    line = line.Replace("\"", "");
                    var values = line.Split(',');

                    ItemsDispatched tempItem = new ItemsDispatched(values[0], values[1], values[6]);
                    itemsDispatched.Add(tempItem);

                }
                reader.Close();
            }
            return itemsDispatched;
        }
    }
}
