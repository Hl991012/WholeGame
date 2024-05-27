namespace ByteDance.Union
{
    public class AdConst
    {
        public const string PangleSdkVersion = "6.0.0.0";

        // 网络状态常量
        public const int NETWORK_STATE_MOBILE = 1;
        public const int NETWORK_STATE_2G = 2;
        public const int NETWORK_STATE_3G = 3;
        public const int NETWORK_STATE_WIFI = 4;
        public const int NETWORK_STATE_4G = 5;
        // adn name常量
        public const string ADN_GDT = "gdt";
        public const string ADN_PANGLE = "pangle";
        public const string ADN_ADMOB = "admob";
        public const string ADN_MINTEGRAL = "mintegral";
        public const string ADN_UNITY = "unity";
        public const string ADN_BAIDU = "baidu";
        public const string ADN_KS = "ks";
        public const string ADN_SIGMOB = "sigmob";
        public const string ADN_KLEVIN = "klevin";

        // 流量分组性别常量
        public const string GENDER_MALE = "male";
        public const string GENDER_FEMALE = "female";
        public const string GENDER_UNKNOWN = "unknown";

        // 以下是开启gromore服务验证时的参数
        public const string KEY_GROMORE_EXTRA = "gromoreExtra"; // gromore服务验证的参数，开发者通过adslot传入
        
        // 抖音授权成功状态回调, 媒体可以通过map获取抖音openuid用以判断是否下发奖励
        public const int AD_EVENT_AUTH_DOUYIN = 1;
    }
}
