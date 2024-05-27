using ByteDance.Union.Mediation;

namespace ByteDance.Union
{
    public sealed class SDKConfiguration
    {
        internal string appId;// 应用ID
        internal string appName;// 应用name
        internal bool useMediation;//是否使用聚合维度功能，开启聚合维度广告加载
        internal bool useTextureView;//使用TextureView控件播放视频,默认为SurfaceView,当有SurfaceView冲突的场景，可以使用TextureView
        internal bool allowShowNotify;//是否允许sdk展示通知栏提示
        internal bool debug;//测试阶段打开，可以通过日志排查问题，上线时去除该调用
        internal int themeStatus;//设置主题类型，0：正常模式；1：夜间模式；默认为0；传非法值，按照0处理
        internal bool supportMultiProcess;//是否支持多进程
        internal int[] directDownloadNetworkType; //允许直接下载的网络状态集合
        internal string data; // jsonArr格式的字符串，透传数据
        internal PrivacyConfiguration privacyConfiguration;//
        internal MediationConfig mediationConfig;//聚合广告维度相关配置


        public class Builder
        {
            private SDKConfiguration configuration = new SDKConfiguration();

            public Builder SetAppId(string appId)
            {
                this.configuration.appId = appId;
                return this;
            }

            public Builder SetAppName(string appName)
            {
                this.configuration.appName = appName;
                return this;
            }

            public Builder SetUseMediation(bool useMediation)
            {
                this.configuration.useMediation = useMediation;
                return this;
            }

            public Builder SetUseTextureView(bool useTextureView)
            {
                this.configuration.useTextureView = useTextureView;
                return this;
            }

            public Builder SetAllowShowNotify(bool allowShowNotify)
            {
                this.configuration.allowShowNotify = allowShowNotify;
                return this;
            }

            public Builder SetDebug(bool debug)
            {
                this.configuration.debug = debug;
                return this;
            }

            public Builder SetThemeStatus(int themeStatus)
            {
                this.configuration.themeStatus = themeStatus;
                return this;
            }

            public Builder SetSupportMultiProcesse(bool supportMultiProcess)
            {
                this.configuration.supportMultiProcess = supportMultiProcess;
                return this;
            }

            public Builder SetPrivacyConfigurationn(PrivacyConfiguration configuration)
            {
                this.configuration.privacyConfiguration = configuration;
                return this;
            }

            public Builder SetMediationConfig(MediationConfig mediationConfig)
            {
                this.configuration.mediationConfig = mediationConfig;
                return this;
            }

            public Builder SetDirectDownloadNetworkType(int[] directDownloadNetworkType)
            {
                this.configuration.directDownloadNetworkType = directDownloadNetworkType;
                return this;
            }

            public Builder SetData(string data)
            {
                this.configuration.data = data;
                return this;
            }

            public SDKConfiguration Build()
            {
                return this.configuration;
            }
        }
    }
}

