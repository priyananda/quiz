using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Shenoy.Quiz.Model;

namespace Shenoy.Quiz.Model
{
    class MediaManager
    {
        public static BitmapImage GetSlide(int slideid)
        {
            if (!m_slides.ContainsKey(slideid))
                m_slides[slideid] = LoadImage("data\\Slide" + slideid + ".png", false);
            return m_slides[slideid];
        }
        public static BitmapImage GetPerson(Person person)
        {
            Tuple<Person, bool> key = new Tuple<Person, bool>(person, false);
            string suffix = key.Item2 ? "_m" : "";
            if (!m_persons.ContainsKey(key))
                m_persons[key] = LoadImage("people\\" + person + suffix + ".jpg", false);
            return m_persons[key];
        }
        public static BitmapImage GetPerson(Celeb celeb)
        {
            if (!m_celebs.ContainsKey(celeb))
                m_celebs[celeb] = LoadImage("celebs\\" + celeb + ".jpg", false);
            return m_celebs[celeb];
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

        private static Dictionary<int, BitmapImage> m_slides = new Dictionary<int,BitmapImage>();
        private static Dictionary<Tuple<Person, bool>, BitmapImage> m_persons = new Dictionary<Tuple<Person, bool>, BitmapImage>();
        private static Dictionary<Celeb, BitmapImage> m_celebs = new Dictionary<Celeb, BitmapImage>();
        private static BitmapImage bmpUnknown = null;
    }
}
