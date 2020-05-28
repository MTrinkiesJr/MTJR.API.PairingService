namespace MTJR.API.PairingService
{
    public static class Constants
    {
        public static string Step1Url => "http://{IpOfTv}:8080/ws/pairing?step=1&app_id=com.samsung.companion&device_id={SameIdOfYourChoice}&type=1";
        public static string Step2Url => "http://{IpOfTv}:8080/ws/pairing?step=2&app_id=com.samsung.companion&device_id={SameIdOfYourChoice}&type=1";

        public static string DuidUrl => "pairing/data/encrypted?pairingId={pairingId}&resource=GetDuidMessage";
        public static string DecryptUrl => "pairing/data/decrypt?pairingId={pairingId}";
    }
}
