namespace Shenoy.Quiz.Model
{
    // If I were a C# enum ...
    public enum Person
    {
        Adarsh,
        AjayBhat,
        AmitB,
        Anirudh,
        Antariksh,
        Aswath,
        Avinash,
        BooBoo,
        Gayathri,
        Gautam,
        Hannibal,
        Harish,
        MaiDang,
        Maitrey,
        Mythreyi,
        Michael,
        Mihir,
        Nandini,
        Pallavi,
        Parth,
        Prabirendra,
        Pranav,
        Priyananda,
        Ruchi,
        RohitSud,
        Samarth,
        Sankalp,
        Shashank,
        Siddharth,
        SriramS,
        Suchitra,
        SudhirAP,
        SuhasRao,
        Surinderjeet,
        Vashutosh,
        Yogesh
    }

    public enum GenderType
    {
        Male,
        Female
    }

    public class PersonTraits
    {
        public static GenderType GenderOf(Person p)
        {
            switch(p)
            {
                case Person.Gayathri:
                case Person.MaiDang:
                case Person.Mythreyi:
                case Person.Nandini:
                case Person.Pallavi:
                case Person.Ruchi:
                case Person.Suchitra:
                    return GenderType.Female;
                default:
                    return GenderType.Male;
            }
        }

        public static string GetWhitifiedName(Person p)
        {
            switch(p)
            {
                case Person.Adarsh: return "Adrian";
                case Person.AjayBhat: return "AJ";
                case Person.AmitB: return "";
                case Person.Anirudh: return "Andy";
                case Person.Antariksh: return "Ant";
                case Person.Aswath: return "Ash";
                case Person.Avinash: return "Avi";
                case Person.BooBoo: return "";
                case Person.Gayathri: return "Guy";
                case Person.Gautam: return "";
                case Person.Hannibal: return "";
                case Person.Harish: return "Harry";
                case Person.MaiDang: return "May";
                case Person.Maitrey: return "";
                case Person.Mythreyi: return "";
                case Person.Michael: return "Michael";
                case Person.Mihir: return "";
                case Person.Nandini: return "";
                case Person.Pallavi: return "";
                case Person.Parth: return "";
                case Person.Prabirendra: return "";
                case Person.Pranav: return "";
                case Person.Priyananda: return "Pete";
                case Person.Ruchi: return "";
                case Person.RohitSud: return "";
                case Person.Samarth: return "";
                case Person.Sankalp: return "";
                case Person.Shashank: return "Shawshank";
                case Person.Siddharth: return "Darth Sid";
                case Person.SriramS: return "";
                case Person.Suchitra: return "Sue";
                case Person.SudhirAP: return "";
                case Person.SuhasRao: return "";
                case Person.Surinderjeet: return "";
                case Person.Vashutosh: return "";
                case Person.Yogesh: return "Yogi Berra";
            }
            return "Donald";
        }
    }
}
