using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenoy.Quiz.Model
{
    class ConnectMatrixRepository
    {
        public static ConnectMatrix GetMatrix(int iset)
        {
            return new ConnectMatrix(m_set0);
        }

        private static string[] m_set0 = new string[]
        {
            "Apple", "Acorn", "Azure", "Adam",
            "Banana", "Baby", "Beer", "Bottle",
            "Cat", "Cow", "Can", "C++",
            "Dog", "Dead", "Doofus", "Ding"
        };
    }
}
