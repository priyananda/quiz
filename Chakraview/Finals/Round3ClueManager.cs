using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Shenoy.Quiz
{
    class Round3ClueManager
    {
        public static BitmapImage GetClue(int set, int clue)
        {
            string filename = String.Format("data\\s{0}c{1}.png", set + 1, clue + 1);
            if (!m_cache.ContainsKey(filename))
                m_cache[filename] = LoadImage(filename);
            return m_cache[filename];
        }

        private static BitmapImage LoadImage(string name)
        {
            try
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                string imagePath = System.IO.Path.Combine(Environment.CurrentDirectory, name);
                bi.UriSource = new Uri(imagePath, UriKind.Absolute);
                bi.EndInit();
                return bi;
            }
            catch
            {
                return null;
            }
        }

        private static Dictionary<string, BitmapImage> m_cache = new Dictionary<string, BitmapImage>();
    }
}
