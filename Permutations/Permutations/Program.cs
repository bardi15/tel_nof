using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permutations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(findPerms(readstring()));
            Console.ReadKey();
        }

        public static string readstring()
        {
            return Console.ReadLine();
        }

        public static int findPerms (string str)
        {
            string newstring = "";
            List<string> allStrings = new List<string>();

            char[] arr = str.ToArray();


            Recursion rec = new Recursion();
            rec.InputSet = arr;
            rec.CalcPermutation(0);
            Console.Write("# of Permutations: " + rec.PermutationCount);


            //for (int z = 0; z < arr.Length-1; z++)
            //{
            //    var tmp = arr[z];
            //    arr[z] = arr[z + 1];
            //    arr[z + 1] = tmp;

            //    for (int x = 0; x < arr.Length - 1; x++)
            //    {
            //        newstring = "";
            //        var tmp2 = arr[x];
            //        arr[x] = arr[x + 1];
            //        arr[x + 1] = tmp2;

            //        foreach (var i in arr)
            //        {
            //            newstring += i;
            //        }

            //        if (!allStrings.Contains(newstring))
            //        {
            //            allStrings.Add(newstring);
            //            //Console.WriteLine(newstring);
            //        }


            //    }
            //}

            //allStrings.Sort();

            //foreach (var item in allStrings)
            //{
            //    Console.WriteLine(item);
            //}

            //for (int k = 0; k < str.Length; k++)
            //{
            //    newstring = str[k].ToString();
            //    for (int l = k + 1; l < str.Length; l++)
            //    {
            //        newstring += str[l];
            //    }

            //    Console.WriteLine(newstring);
            //}


            return allStrings.Count;
        }

    }

    public class Recursion
    {
        private int elementLevel = -1;
        private int numberOfElements;
        private int[] permutationValue = new int[0];

        private char[] inputSet;
        public char[] InputSet
        {
            get { return inputSet; }
            set { inputSet = value; }
        }

        private int permutationCount = 0;
        public int PermutationCount
        {
            get { return permutationCount; }
            set { permutationCount = value; }
        }

        public char[] MakeCharArray(string InputString)
        {
            char[] charString = InputString.ToCharArray();
            Array.Resize(ref permutationValue, charString.Length);
            numberOfElements = charString.Length;
            return charString;
        }

        public void CalcPermutation(int k)
        {
            elementLevel++;
            permutationValue.SetValue(elementLevel, k);

            if (elementLevel == numberOfElements)
            {
                OutputPermutation(permutationValue);
            }
            else
            {
                for (int i = 0; i < numberOfElements; i++)
                {
                    if (permutationValue[i] == 0)
                    {
                        CalcPermutation(i);
                    }
                }
            }
            elementLevel--;
            permutationValue.SetValue(0, k);
        }

        private void OutputPermutation(int[] value)
        {
            foreach (int i in value)
            {
                Console.Write(inputSet.GetValue(i - 1));
            }
            Console.WriteLine();
            PermutationCount++;
        }
    }
}
