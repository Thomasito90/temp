using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Security;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string siteCollectionUrl = "https://****.sharepoint.com/sites/sharepointallaccess";
            // string siteCollectionUrl = "https://****.sharepoint.com/"; <- tried this one too
            string userName = "thomas.****@*****.co.uk";
            string password = "*********";
            Program obj = new Program();
            try
            {
                obj.ConnectToSharePointOnline(siteCollectionUrl, userName, password);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                Console.WriteLine("Error" + msg);
            }

        }

        public void ConnectToSharePointOnline(string siteCollUrl, string userName, string password)
        {
            //Namespace: It belongs to Microsoft.SharePoint.Client
            ClientContext ctx = new ClientContext(siteCollUrl);
            // Namespace: It belongs to System.Security
            SecureString secureString = new SecureString();
            password.ToList().ForEach(secureString.AppendChar);
            // Namespace: It belongs to Microsoft.SharePoint.Client
            ctx.Credentials = new SharePointOnlineCredentials(userName, secureString);
            // Namespace: It belongs to Microsoft.SharePoint.Client
            Site mySite = ctx.Site;
            ctx.Load(mySite);
            ctx.ExecuteQuery();
            Console.WriteLine(mySite.Url.ToString());
        }
    }
}
