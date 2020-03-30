using Microsoft.WindowsAzure.Storage.Table;

namespace MyAzureWebJob
{
    public class Registration : TableEntity
    {
        public string fullname { get; set; }

        public string image { get; set; }

        public string thumb { get; set; }
    }
}
