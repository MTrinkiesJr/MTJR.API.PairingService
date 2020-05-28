namespace MTJR.API.PairingService.Model.Messages
{
    public class RegisterSecondTvMessage : Message
    {
        public RegisterSecondTvMessage()
        {
            EventType = "EMP";
            Plugin = "SecondTv";
        }
    }
}
