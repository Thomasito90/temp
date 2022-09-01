using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace json_reader
{
    public class Medicine
    {
        public string Identifier { get; set; }
        public JObject objct { get; set; }

        public Medicine(string Identifier, JObject objct)
        {
            this.Identifier = Identifier;
            this.objct = objct;
        }
    }

    internal class Program
    {

        public static bool menu()
        {
            string answer = "";
            Console.WriteLine("\nWould you like to start? Y/N");
            answer = Console.ReadLine().ToLower();
            if (answer == "n")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string getCurrentDir()
        {
            string currentDir = Directory.GetCurrentDirectory();
            currentDir = currentDir + "\\";

            Console.WriteLine("\nCurrent directory is:" +
                "\n{0}\n", currentDir);
            Console.WriteLine("Would you like to change your directory? Y/N");
            if (Console.ReadLine().ToLower() == "y")
            {
                currentDir = setDir() + "\\";
                Console.WriteLine("\nYour new directory is:" +
                "\n{0}\n", currentDir);
            }


            return currentDir;
        }

        public static string setDir()
        {
            string new_path = "";
            Console.WriteLine("\nPlease provide a new path: ");
            new_path = Console.ReadLine();

            return new_path;
        }

        public static string getFileName()
        {
            Console.WriteLine("\nPlease provide a file name: ");
            string fileName = "";
            fileName = Console.ReadLine();

            return fileName;
        }

        public static bool checkFile(string dir, string fileName)
        {
            string filePath = dir + fileName;
            Console.WriteLine("\nYour file path is: {0}", filePath);
            if (File.Exists(filePath))
            {
                Console.WriteLine("File exists in the directory provided");
                return true;
            }
            else
            {
                Console.WriteLine("File does not exist in the directory provided");
                return false;
            }
            
        }

        public static JToken readFile(string dir, string fileName)
        {
            StreamReader file = File.OpenText(dir + fileName);
            JsonTextReader reader = new JsonTextReader(file);
            
            JToken jsonObjects = JToken.ReadFrom(reader);
            return jsonObjects;
            
        }

        public static void createObjects(JToken jsonObjects, string dir)
        {
            List<Medicine> medicinesList = new List<Medicine>();

            foreach (JObject objc in jsonObjects)
            {
                Medicine tempMedicine = new Medicine((string)objc["Identifier"], objc);
                medicinesList.Add(tempMedicine);
            }

            List<Medicine> sortedMedicines = medicinesList.OrderBy(o => o.Identifier).ToList();


            foreach (Medicine med in sortedMedicines)
            {
                using (StreamWriter sw = File.AppendText(dir + "sortedMedicines.json"))
                {
                    sw.WriteLine(med.objct.ToString());
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the JSON reader.\n");

            while (menu())
            {
                string dir = getCurrentDir();
                string fileName = getFileName();
                bool checkFileResult = checkFile(dir, fileName);
                if (checkFileResult)
                {
                    JToken jsonObjects = readFile(dir, fileName);
                    createObjects(jsonObjects, dir);
                }
            }

            System.Environment.Exit(0);
        }
    }
}
