using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NEA_QRCODE
{
    public class ErrorCorrection
    {
        private const int GFSize = 256;
        public List<int> GeneratorPolynomial;
        public List<int> MessagePolynomial;
        public int[] exAlphaToInt = new int[GFSize];
        public int[] intToExAlpha = new int[GFSize];

        public ErrorCorrection()
        {
            InitializeTable();
            GeneratorPolynomial = new List<int> {0, 87, 229, 146, 149, 238, 102, 21};
            MessagePolynomial = new List<int>();
            CreateGeneratorPolynomial(15);
        }

        void CalcCodewordsEC()
        {
            // Prepare for division
            for (int i = 0; i < 15; i++)
            {
                MessagePolynomial.Add(0);
            }
            int m = MessagePolynomial.Count - GeneratorPolynomial.Count;
            for (int i = 0; i < m; i++)
            {
                GeneratorPolynomial.Add(0);
            }


        }

        public void CreateMessagePolynomial(string encodedData)
        {  
            for (int i = 0; i < (encodedData.Length / 8); i++)
            {
                int dataByte = Convert.ToInt32(encodedData.Substring(i * 8, 8), 2);
                MessagePolynomial.Add(dataByte);
            }
            CalcCodewordsEC();
        }

        private void CreateGeneratorPolynomial(int degree)
        {

            // How many times to iterate to create polynomial
            for (int i = 0; i < degree - 7; i++)
            {

                // Temporary lists for holding the multiples of the first generator polynomial
                List<int> tempList1 = new List<int>(GeneratorPolynomial);
                List<int> tempList2 = new List<int>(GeneratorPolynomial);

                // For x * generator polynomial adding 1 at the end for aligning
                tempList1.Add(1);

                // Clear for next use
                GeneratorPolynomial.Clear();

                // Multiplies by alpha^(current degree)
                for (int j = 0; j < tempList2.Count; j++)
                {
                    tempList2[j] += degree - 1 - i;
                    if (tempList2[j] > 255)
                    {
                        tempList2[j] %= 255;
                    }
                    tempList2[j] = exAlphaToInt[tempList2[j]];
                }
                // Aligning
                tempList2.Insert(0, 0);

                // Converting to ints and collecting like terms
                for (int k = 0; k < tempList1.Count - 1; k++)
                {
                    tempList1[k] = exAlphaToInt[tempList1[k]];
                    GeneratorPolynomial.Add(intToExAlpha[tempList1[k] ^ tempList2[k]]);
                }

                //Final polynomial requires this code
                GeneratorPolynomial.Add(intToExAlpha[tempList2[tempList2.Count - 1]]);

                //Clear for next use and efficient memory usage
                tempList1.Clear();
                tempList2.Clear();

            }
            
        }

        public void InitializeTable()
        {
            exAlphaToInt[0] = 1;
            for (int i = 1; i < GFSize; i++)
            {
                exAlphaToInt[i] = exAlphaToInt[i - 1] * 2;
                if (exAlphaToInt[i] > 255)
                {
                    exAlphaToInt[i] ^= 285;
                }
                intToExAlpha[exAlphaToInt[i]] = i;
            }
            intToExAlpha[1] = 0;
        }


    }
}
