using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nafnateljari
{

    public class Program
    {
        const int STANDARDWITDH = 30;
        const string dbcstr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"J:\\USERFILES\\Documents\\Visual Studio 2015\\Projects\\Mannanofn\\Mannanofn\\nofn.mdf\";Integrated Security=True;Connect Timeout=30";
        static List<string> lnL_M;
        static List<string> lnL_F;

        static void Main(string[] args)
        {
            PrintF("LOADING");
            lnL_M = findLastName(true);
            lnL_F = findLastName(false);


            //Teljari tel = new Teljari();
            //tel.ThousandSep(9876543210);
            //Console.ReadKey();
            menu();


        }

        static void menu()
        {
            Console.Clear();
            char choice;
            for (;;)
            {
                do
                {
                    Console.WriteLine("Búa til nafn:");
                    Console.WriteLine("  1. Karlmannsnafn");
                    Console.WriteLine("  2. Karlmannsnafn + Millinafn");
                    Console.WriteLine("  3. Kvenmannsnafn");
                    Console.WriteLine("  4. Kvenmannsnafn + Millinafn");
                    Console.WriteLine("  5. Prenta öll nöfn");
                    Console.WriteLine("  6. Búa til mikið af nöfnum");
                    Console.Write("veldu q til að hætta: ");
                    do
                    {
                        choice = Console.ReadKey().KeyChar;
                    } while (choice == '\n' | choice == '\r');
                } while (choice < '1' | choice > '8' & choice != 'q');

                if (choice == 'q') break;

                Console.WriteLine("\n");

                var Pple = connectToDatabase();
                var People = (from p in Pple where p.afgreitt == true select p);



                switch (choice)
                {
                    case '1':
                        PrintF(S_KN(People, 1, true));
                        break;
                    case '2':
                        PrintF(S_KN(People, 2, true));
                        break;
                    case '3':
                        PrintF(S_KN(People, 1, false));
                        break;
                    case '4':
                        PrintF(S_KN(People, 2, false));
                        break;
                    case '5':
                        PrintAll(People);
                        break;
                    case '6':
                        ShitLoadOfNames(People);
                        break;
                    case '7':
                        FindForeignName(People);
                        break;
                    default:
                        break;
                }


            }
        }

        static void PrintF(string toPrint)
        {
            Console.Clear();

            Console.WriteLine();

            for (int i = 0; i < toPrint.Length + STANDARDWITDH; i++) { Console.Write("-"); }
            Console.Write("\n");

            Console.WriteLine(toPrint);

            for (int i = 0; i < toPrint.Length + STANDARDWITDH; i++) { Console.Write("-"); }
            Console.Write("\n");
            Console.WriteLine();

        }

        static void FindForeignName(IEnumerable<PeopleNames> People)
        {
            bool gender;
            Console.WriteLine("ENTER YOUR GENDER M/F:");
            var key = Console.ReadKey().ToString().ToUpper();
            if (key == "M")
            {
                gender = true;
            }
            else
            {
                gender = false;
            }


            Console.WriteLine("ENTER YOUR NAME: ");

            string name = Console.ReadLine();
            int newlowest = int.MaxValue;

            var islfemale = from p in People where p.afgreitt == true && p.tegund == NameTypes.ST && (p.nafn[0] == name[0]) select p.nafn;

            var islmale = from p in People where p.afgreitt == true && p.tegund == NameTypes.DR && (p.nafn[0] == name[0]) select p.nafn;

            IEnumerable<string> testbin;

            if (gender)
            {
                testbin = islmale;
            }
            else
            {
                testbin = islfemale;
            }

            foreach (var k in testbin)
            {
                int val = LevenshteinDistance.Compute(name, k);
                if (val < newlowest)
                {
                    newlowest = val;
                    Console.WriteLine("NEW LOW:" + newlowest + " name is: " + k);
                }
            }

            Console.ReadKey();

        }

        static void ShitLoadOfNames(IEnumerable<PeopleNames> People)
        {
            Console.WriteLine("HOW MANY UNIQUE NAMES WOULD YOU LIKE?: ");
            int number = Int32.Parse(Console.ReadLine());

            List<string> PeopleNam = new List<string>();

            Random rand = new Random();

            while (PeopleNam.Count <= number)
            {
                string name = S_KN(People, rand.Next(1, 3), rand.Next(99) % 2 == 1);
                if (!PeopleNam.Contains(name)) { PeopleNam.Add(name); }
            }

            var nm = (from N in PeopleNam orderby N ascending select N).ToList();

            if (nm.Count > 1000)
            {
                for (int y = 0; y < nm.Count; y++)
                {

                    //if (y % 6 == 0) { Console.Write("\n"); }
                    string wostr = nm.ElementAt(y).PadRight(33);
                    Console.Write(wostr + "\t");
                }
            }
            else
            {
                for (int y = 0; y < nm.Count; y++)
                {
                    Console.Write(y + "\t" + nm.ElementAt(y) + "\n");
                }
            }


            Console.ReadKey();
        }

        static string S_KN(IEnumerable<PeopleNames> People, int fjoldi, bool gender)
        {

            IEnumerable<string> people;
            List<string> lastName;

            if (gender)
            {
                people = (from p in People where p.tegund == NameTypes.DR || p.tegund == NameTypes.RDR select p.nafn);
                lastName = lnL_M;

            }
            else
            {
                people = (from p in People where p.tegund == NameTypes.RST || p.tegund == NameTypes.ST select p.nafn);
                lastName = lnL_F;
            }

            Random rand = new Random();
            string name = "";

            for (int i = 0; i < fjoldi; i++)
            {
                name += people.ElementAt(rand.Next(1, people.Count())) + " ";
            }

            name += lastName.ElementAt(rand.Next(0, lastName.Count));

            return name;
        }

        //static string S_KN_M(IEnumerable<PeopleNames> People)
        //{
        //    var menn = (from p in People where p.tegund == NameTypes.DR || p.tegund == NameTypes.RDR select p.nafn);
        //    Random rand = new Random();
        //    string name = menn.ElementAt(rand.Next(1, menn.Count())) + " "
        //        + menn.ElementAt(rand.Next(1, menn.Count())) + " "
        //        + FodurNafnFix( menn.ElementAt(rand.Next(1, menn.Count())) );
        //    return name;
        //}

        static string FodurNafnFix(string fodurnafn)
        {
            //fodurnafn = "Ketilbjörn";

            int len = fodurnafn.Length;

            if (fodurnafn.EndsWith("ur")) { return fodurnafn.Substring(0, len - 2) + "son"; }
            else if (fodurnafn.EndsWith("ar")) { return fodurnafn + "sson"; }
            else if (fodurnafn.EndsWith("i")) { return fodurnafn.Substring(0, len - 1) + "ason"; }



            return fodurnafn + "son";
        }

        static void PrintAll(IEnumerable<PeopleNames> People)
        {
            Console.WriteLine("============================= DRENGIR =============================");

            var dr = from der in People where der.tegund == NameTypes.DR select der.nafn;
            foreach (var item in dr) { Console.WriteLine(item); }
            Console.WriteLine();

            Console.WriteLine("============================= ÆTTARNÖFN =============================");
            var mi = from der in People where der.tegund == NameTypes.MI select der.nafn;
            foreach (var item in mi) { Console.WriteLine(item); }
            Console.WriteLine();

            Console.WriteLine("============================= DRENGIR ERLEND =============================");
            var rdr = from der in People where der.tegund == NameTypes.RDR select der.nafn;
            foreach (var item in rdr) { Console.WriteLine(item); }
            Console.WriteLine();

            Console.WriteLine("============================= STÚLKUR ERLEND =============================");
            var rst = from der in People where der.tegund == NameTypes.RST select der.nafn;
            foreach (var item in rst) { Console.WriteLine(item); }
            Console.WriteLine();

            Console.WriteLine("============================= STÚLKUR =============================");
            var st = from der in People where der.tegund == NameTypes.ST select der.nafn;
            foreach (var item in st) { Console.WriteLine(item); }
            Console.WriteLine();

            Console.ReadKey();
        }


        static List<string> findLastName(bool gender)
        {
            List<string> NameList = new List<string>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbcstr;
                conn.Open();

                //SqlCommand command = new SqlCommand("SELECT DISTINCT S.nafn from skra S WHERE S.NAFN LIKE '%son' AND(S.KYN = '3' OR S.KYN = '1')", conn);
                SqlCommand command;
                if (gender)
                {
                    command = new SqlCommand("SELECT * from skra WHERE NAFN LIKE '%son'", conn);
                }
                else { command = new SqlCommand("SELECT * from skra WHERE NAFN LIKE '%dóttir'", conn); }
                int counter = 0;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        counter++;
                        var k = reader[4].ToString();
                        //Console.Write(k);

                        string lName = reader[4].ToString().Split(' ').Last();
                        if (!NameList.Contains(lName))
                        {
                            NameList.Add(lName);
                            //Console.Write("Add..");
                        }

                        if (counter % 4000 == 0) { Console.Write("."); };

                    }
                }
            }

            //Console.WriteLine("FOUND: " + NameList.Count + " ELEMENTS");

            var nm = (from N in NameList orderby N ascending select N).ToList();

            Random rand = new Random();

            return nm;

            //return nm.ElementAt(rand.Next(0, nm.Count));

            //foreach (var item in nm)
            //{
            //    Console.WriteLine(item);
            //}

            //Console.ReadKey();
        }

        static List<PeopleNames> connectToDatabase()
        {

            List<PeopleNames> ppl = new List<PeopleNames>();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbcstr;
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM Nofn", conn);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<string> PersonColumns = new List<string>();
                        PersonColumns.Add(reader[0].ToString()); PersonColumns.Add(reader[1].ToString()); PersonColumns.Add(reader[2].ToString());
                        PersonColumns.Add(reader[3].ToString()); PersonColumns.Add(reader[4].ToString()); PersonColumns.Add(reader[5].ToString());
                        PersonColumns.Add(reader[6].ToString());

                        ppl.Add(MakePerson(PersonColumns));

                    }
                }
            }

            return ppl;

        }

        static PeopleNames MakePerson(List<string> personColumns)
        {

            PeopleNames Person = new PeopleNames();

            Person.nafn = personColumns.ElementAt(0);

            string afgrtt = personColumns.ElementAt(1);
            if (afgrtt == "Sam") { Person.afgreitt = true; }
            else if (afgrtt == "Haf") { Person.afgreitt = false; }
            else {
                // Console.WriteLine("ERROR IN -AFGREITT-");
            }

            int brta = int.MaxValue;
            try
            {
                brta = Int32.Parse(personColumns.ElementAt(2).Substring(0, 1));
            }
            catch (Exception e) { Console.WriteLine("{0} Exception caught.", e); };
            Person.birta = brta;

            string tegnd = personColumns.ElementAt(3).ToUpper();
            NameTypes n = new NameTypes();
            switch (tegnd)
            {
                case "DR":
                    n = NameTypes.DR; break;
                case "MI":
                    n = NameTypes.MI; break;
                case "RDR":
                    n = NameTypes.RDR; break;
                case "ST":
                    n = NameTypes.ST; break;
                case "RST":
                    n = NameTypes.RST; break;
                default:
                    Console.WriteLine("ERROR IN TEGUND"); break;
            }
            Person.tegund = n;

            Person.skyring = personColumns.ElementAt(4);

            Person.urskurdur = personColumns.ElementAt(5);

            Person.ID = int.Parse(personColumns.ElementAt(6).Split(',')[0]);
            return Person;
        }



    }

    public partial class PeopleNames
    {
        public String nafn { get; set; }

        public bool afgreitt { get; set; }

        public int birta { get; set; }

        public NameTypes tegund { get; set; }

        public string skyring { get; set; }
        public string urskurdur { get; set; }
        public int ID { get; set; }
    }

    public enum NameTypes
    {
        DR,
        MI,
        RDR,
        ST,
        RST
    }


    public class Teljari
    {
        public string[] FyrstuTuttugu = new string[21]
        { "núll", "einn", "tveir", "þrír", "fjórir", "fimm", "sex", "sjö", "átta", "níu", "tíu", "ellefu",
        "tólf", "þrettán", "fjórtán", "fimmtán", "sextán", "sautján", "átján", "nítján", "tuttugu"};

        //suffix -tíu
        public string[] FyrstuHundrad = new string[10]
        { "núll", "tíu", "tuttugu", "þrjá", "fjöru", "fimm", "sex", "sjö", "átta", "níu" };

        //suffix - hundruð
        public string[] FyrstuThusund = new string[10]
        { "núll", "hundrað", "tvö", "þrjú", "fjögur", "fimm", "sex", "sjö", "átta", "níu"};

        //Þúsund = hundruðir + þúsund

        public string faTolur(int tala)
        {

            return null;

            if (tala < 20)
            {
                return FyrstuTuttugu[tala];
            }
            else if (tala < 100)
            {
                string numberstring;



            }
            else if (tala < 1000)
            {

            }
            else
            {

            }
            //string tolustrengur = tala.ToString();

            //foreach (var item in tolustrengur)
            //{

            //}

        }

        public List<int> ThousandSep(long number)
        {

            const int chSize = 3;

            string num = number.ToString();

            while (!(num.Length % chSize == 0))
            {
                num = "0" + num;
                Console.WriteLine(num);
            }

            IEnumerable<string> List = Enumerable.Range(0, num.Length / chSize)
            .Select(i => num.Substring(i * chSize, chSize));

            List<long> rell = new List<long>();

            int reverseCounter = 0;
            for (int x = List.Count() - 1 ; x >= 0; x--)
            {
                string stringstring = "";
                stringstring = List.ElementAt(x) + ZeroAdder(reverseCounter);
                reverseCounter++;
                //Console.WriteLine(stringstring);
                rell.Add(long.Parse(stringstring));
            }

            foreach ( var iee in rell)
            {
                Console.WriteLine(iee);
            }

            return null;

        }

        public string ZeroAdder (int number)
        {
            string returnstring = "";
            for (int x = 0; x < number; x++)
            {
                returnstring += "000";
            }

            return returnstring;
        }

    }


    static class LevenshteinDistance
    {
        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }

}
