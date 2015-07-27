using System;

namespace RK.Wpf3DSampleBrowser
{
    public enum SampleType
    {
        WpfSample,

        SharpDXSample
    }

    public class SampleAttribute : Attribute
    {
        public SampleAttribute(SampleType sampleType, int orderValue, string previewImageUrl)
        {
            this.SampleType = sampleType;
            this.OrderValue = orderValue;
            this.PreviewImageUrl = previewImageUrl;
        }

        public int OrderValue
        {
            get;
            private set;
        }

        public string PreviewImageUrl
        {
            get;
            private set;
        }

        public SampleType SampleType
        {
            get;
            private set;
        }
    }
}