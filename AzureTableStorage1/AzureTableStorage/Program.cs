using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureTableStorage
{
    class Program
    {
        static CloudStorageAccount StorageAccount;
        static CloudTableClient tableClient;
        static CloudTable table;
        static void Main(string[] args)
        {
            int i;
            try
            {
                CreateAzureStorageTable();
                AddGuestEntity();
                RetrieveGuestEntity();
                UpdateGuestEntity();
                DeleteGuestEntity();
                DeleteAzureStorageTable();
            }
            catch(StorageException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void CreateAzureStorageTable()
        {
            StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            tableClient = StorageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference("guests");
            table.CreateIfNotExists();
            Console.WriteLine("Table Created");
        }
        private static void AddGuestEntity()
        {
            GuestEntity guestEntity = new GuestEntity("IND", "K001");
            guestEntity.Name = "Hemendu";
            guestEntity.ContactNumber = "1234567890";
            TableOperation insertOperation = TableOperation.Insert(guestEntity);
            table.Execute(insertOperation);
            Console.WriteLine("Entity Added");
        }
        private static void RetrieveGuestEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                var guest = retrievedResult.Result as GuestEntity;
                Console.WriteLine($"Name: {guest.Name} ContactNumber:{guest.ContactNumber}");
            }
            else
            {
                Console.WriteLine("Details could not be retrieved");
            }
        }
        public static void UpdateGuestEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                var guest = retrievedResult.Result as GuestEntity;
                guest.ContactNumber = "7832823855";
                TableOperation updateOperation = TableOperation.Replace(guest);
                table.Execute(updateOperation);
                Console.WriteLine("Entity Updated");
            }
            else
            {
                Console.WriteLine("Details could not be retrieved");
            }
        }
        private static void DeleteAzureStorageTable()
        {
            table.DeleteIfExists();
            Console.WriteLine("Table Deleted");
        }
        private static void DeleteGuestEntity()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<GuestEntity>("IND", "K001");
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                var guest = retrievedResult.Result as GuestEntity;
                TableOperation deleteOperation = TableOperation.Delete(guest);
                table.Execute(deleteOperation);
                Console.WriteLine("Entity Deleted");
            }
            else
            {
                Console.WriteLine("Details could not be retrieved");
            }
        }
    }

    class GuestEntity : TableEntity
    {
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public GuestEntity() { }
        public GuestEntity(string partitionKey,string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }
    }
}
