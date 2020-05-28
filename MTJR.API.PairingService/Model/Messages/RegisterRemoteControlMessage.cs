namespace MTJR.API.PairingService.Model.Messages
{
    public class RegisterRemoteControlMessage : Message
    {
        public RegisterRemoteControlMessage()
        {
            EventType = "EMP";
            Plugin = "RemoteControl";
        }

    }
}
