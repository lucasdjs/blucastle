using blucaste.Models;
using Google.Cloud.Firestore;

namespace blucaste.Data
{
    public class FirestoreService
    {
        private FirestoreDb _firestoreDb;

        public FirestoreService()
        {
            var pathToJson = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "firestore.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToJson);
            _firestoreDb = FirestoreDb.Create("blucaste-c2bd4");
        }

        public async Task<bool> AtualizarQuantidadeUsoAsync(string uid, int novaQuantidade)
        {
            try
            {
                var docRef = _firestoreDb.Collection("users").Document(uid);

                var updates = new Dictionary<string, object>
        {
            { "quantidadeUso", novaQuantidade }
        };

                await docRef.UpdateAsync(updates);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar quantidade de uso: {ex.Message}");
                return false;
            }
        }


        public async Task<DocumentSnapshot> BuscarUsuarioPorSerialAsync(string serial)
        {
            var usersRef = _firestoreDb.Collection("users");
            var query = usersRef.WhereArrayContains("seriais", serial);
            var snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count > 0)
            {
                return snapshot.Documents[0];
            }

            return null;
        }

        public async Task<bool> AtualizarSerialDoUsuarioAsync(string uid, string serial)
        {
            var docRef = _firestoreDb.Collection("users").Document(uid);

            try
            {
                await docRef.UpdateAsync("seriais", FieldValue.ArrayUnion(serial.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao atualizar serial: " + ex.Message);
                return false;
            }
        }


        public async Task<Usuario?> BuscarUsuarioPorIdentificadorAsync(string identificador)
        {
            var usersRef = _firestoreDb.Collection("users");
            var query = usersRef.WhereEqualTo("identificador", identificador);
            var snapshot = await query.GetSnapshotAsync();

            if (snapshot.Documents.Count > 0)
            {
                var doc = snapshot.Documents[0];
                var usuario = doc.ConvertTo<Usuario>();
                usuario.Uid = doc.Id;
                return usuario;
            }

            return null;
        }

    }
}
