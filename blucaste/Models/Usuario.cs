using Google.Cloud.Firestore;
using Microsoft.WindowsAzure.Storage.Table;

namespace blucaste.Models
{
    [FirestoreData]
    public class Usuario
    {
        [FirestoreProperty]
        public string display_name { get; set; }

        [FirestoreProperty]
        public string identificador { get; set; }

        [FirestoreProperty]
        public bool adminUser { get; set; }

        [FirestoreProperty]
        public bool log { get; set; }

        [FirestoreProperty]
        public int quantidadeUso { get; set; }

        [FirestoreProperty]
        public List<string> serial { get; set; }

        [FirestoreProperty]
        public bool status { get; set; }

        [FirestoreProperty]
        public Timestamp validade { get; set; }

        [FirestoreProperty]
        public int tipoUso { get; set; }

        [IgnoreProperty]
        public string Uid { get; set; }
    }
}
