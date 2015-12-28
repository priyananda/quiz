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

    public class Gender
    {
        public static GenderType Of(Person p)
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
    }
}
