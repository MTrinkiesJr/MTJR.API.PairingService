namespace MTJR.API.PairingService.Model
{
    public class Session
    {
        public string EncryptionKey { get; set; }
        public int SessionId { get; set; }

        public Session(string encryptionKey, int sessionId)
        {
            EncryptionKey = encryptionKey;
            SessionId = sessionId;
        }
    }
}
