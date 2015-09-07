using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Shenoy.Question.Model;

namespace Shenoy.Question.Model
{
    public class MediaManager
    {
        public static BitmapImage GetSlide(int slideid)
        {
            if (!m_slides.ContainsKey(slideid))
                m_slides[slideid] = LoadImage("data\\Slide" + slideid + ".png", false);
            return m_slides[slideid];
        }
        public static BitmapImage GetSlide(ObjectWithSlide ows)
        {
            return GetSlide(ows.SlideId);
        }
        public static BitmapImage UnknownImage
        {
            get
            {
                if (bmpUnknown == null)
                    bmpUnknown = LoadImage("data\\unknown.png", false);
                return bmpUnknown;
            }
        }
        public static Uri VideoURL(string filename)
        {
            string imagePath = System.IO.Path.Combine(Environment.CurrentDirectory, filename);
            return new Uri(imagePath, UriKind.Absolute);
        }

        public static BitmapImage LoadImage(string name, bool fRel)
        {
            try {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (fRel)
                {
                    bi.UriSource = new Uri(name, UriKind.Relative);
                }
                else
                {
                    string imagePath = System.IO.Path.Combine(Environment.CurrentDirectory, name);
                    bi.UriSource = new Uri(imagePath, UriKind.Absolute);
                }
                bi.EndInit();
                return bi;
            }
            catch
            {
                return UnknownImage;
            }
        }

        private static Dictionary<int, BitmapImage> m_slides = new Dictionary<int,BitmapImage>();
        private static BitmapImage bmpUnknown = null;
    }
}
