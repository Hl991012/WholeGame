namespace ByteDance.Union.Mediation
{
    using System.Collections.Generic;

    public sealed class MediationAdEcpmInfo
    {

        public Dictionary<string, string> customData { set; get; }
        public string sdkName { set; get; }
        public string customSdkName { set; get; }
        public string slotId { set; get; }
        public string levelTag { set; get; }
        public string ecpm { set; get; }
        public int reqBiddingType { set; get; }
        public string errorMsg { set; get; }
        public string requestId { set; get; }
        public string ritType { set; get; }
        public string segmentId { set; get; }
        public string channel { set; get; }
        public string subChannel { set; get; }
        public string abTestId { set; get; }
        public string scenarioId { set; get; }

        public override string ToString()
        {
            string customDataStr = "";
            if (customData != null)
            {
                foreach (KeyValuePair<string,string> pair in customData)
                {
                    customDataStr += (pair.Key + ": " + pair.Value + " ");
                }
            }
            return "MediationAdEcpmInfo: sdkName:" + sdkName + ", customSdkName:" + customSdkName
                + ", slotId:" + slotId + ", levelTag:" + levelTag + ", ecpm:" + ecpm + 
                ", reqBiddingType:" + reqBiddingType + ", errorMsg:" + errorMsg
                + ", requestId:" + requestId + ", ritType:" + ritType + ", segmentId:" + segmentId +
                ", channel:" + channel + ", subChannel:" + subChannel
                + ", abTestId:" + abTestId + ", scenarioId:" + scenarioId + ", customData:" + customDataStr;
        }
    }
}