using System.Text;
using System.Text.RegularExpressions;
using MTJR.API.PairingService.Model;
using MTJR.API.PairingService.Model.Messages;
using Networking.Native;
using Newtonsoft.Json;
using Pairing;

namespace MTJR.API.PairingService.Handler
{
    public class PairingSession
    {
        private SPCApiBridge _spcApi;
        private string _serverAck;
        private string _serverHello;
        private bool _parseClientAck;
        public string Id { get; }
        public string Pin { get; private set; }
        public HandshakeResourceType Step { get; private set; } = HandshakeResourceType.None;
        public string Duid { get; private set; }
        public byte[] Key { get; private set; }

        public PairingSession(string id)
        {
            Id = id;
            _spcApi = new SPCApiBridge("654321");
        }

        public string GenerateServerAck()
        {
            Step = HandshakeResourceType.ClientAck;
            return _serverAck == null ? _serverAck = _spcApi.GenerateServerAck() : _serverAck;
        }

        public string GenerateServerHello(string pin)
        {
            if (Step == HandshakeResourceType.None)
            {
                Pin = pin;
                Step = HandshakeResourceType.ClientHello;
                return _serverHello == null ? _serverHello = _spcApi.GenerateServerHello(pin) : _serverHello;
            }

            return null;
        }

        public bool ParseClientHello(string clientHelloData)
        {
            if (Step == HandshakeResourceType.ClientHello)
            {
                var regexPattern = "[{\\\"\\w:]GeneratorClientHello[\\\\\\\":]*([\\d\\w]*)";
                var requestData = Regex.Match(clientHelloData, regexPattern).Groups[1].Value;

                var parsed = _spcApi.ParseClientHello(Pin, clientHelloData);
                Step = HandshakeResourceType.ServerAck;
                return parsed;
            }

            return false;
        }

        public bool ParseClientAck(string clientAckMsgData)
        {
            if (Step == HandshakeResourceType.ClientAck)
            {
                var parsed = _spcApi.ParseClientAck(clientAckMsgData);
                Step = HandshakeResourceType.Session;
                return parsed;
            }

            return false;
        }

        public byte[] GetKey()
        {
            if (Step == HandshakeResourceType.Session)
            {
                return Key = _spcApi.GetKey();
            }

            return null;
        }

        public string Decrypt(string data, int sessionId)
        {
            if (Step == HandshakeResourceType.Session)
            {
                var securityProvider = new AesSecurityProvider(Key, sessionId);
                var decrypted = securityProvider.DecryptData(data);

                CheckDuidMessage(decrypted);
                return decrypted;
            }
            return null;
        }

        private void CheckDuidMessage(string body)
        {
            ReceivedMessage receivedMessage = null;

            try
            {
                receivedMessage = JsonConvert.DeserializeObject<ReceivedMessage>(body);
            }
            catch { }

            if (receivedMessage != null)
            {
                if (receivedMessage.Plugin == "NNavi" && receivedMessage.Api == "GetDUID")
                {
                    Duid = receivedMessage.Result.ToString();
                }
            }
        }
    }
}
