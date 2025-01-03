﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NEA_QRCODE
{
    public class ErrorCorrection
    {
        private const int GFSize = 256;
        private const int GPSize = 15;
        private const int maskNumber = 0;
        public string fString;
        public string fGeneratorPolynomial = "10100110111";
        public string fMaskString = "101010000010010";
        public List<int> GeneratorPolynomial = new List<int> {0, 87, 229, 146, 149, 238, 102, 21};
        public List<int> MessagePolynomial;
        public int[] exAlphaToInt = new int[GFSize];
        public int[] intToExAlpha = new int[GFSize];

        public ErrorCorrection()
        {
            InitializeTable();
            fString = "01" + Convert.ToString(maskNumber, 2).PadLeft(3, '0');
            MessagePolynomial = new List<int> ();
            
        }

        

        public string CreateFormatString()
        {
            string tempString = fString;

            string fGeneratorPolynomialPadded = fGeneratorPolynomial;

            tempString = tempString.PadRight(15, '0');

            while (tempString.Substring(0, 1) == "0")
            {
                tempString = tempString.Substring(1);
            }

            while (tempString.Length > 10)
            {
                
                fGeneratorPolynomialPadded = fGeneratorPolynomial.PadRight(tempString.Length, '0');
                tempString = XORBinaryStrings(tempString, fGeneratorPolynomialPadded);
                while (tempString.Substring(0, 1) == "0")
                {
                    tempString = tempString.Substring(1);
                } 
                
            }

            tempString = tempString.PadLeft(10, '0');

            return XORBinaryStrings(fString + tempString, fMaskString);
        }

        private string XORBinaryStrings(string str1, string str2)
        {
            char[] result = new char[str1.Length];
            for (int i = 0; i < str1.Length; i++)
            {
                // XOR operation for each character
                result[i] = str1[i] == str2[i] ? '0' : '1';
            }
            return new string(result);
        }


        public string CreateECCodewords(string encodedData)
        {
            CreateGeneratorPolynomial(GPSize);
            CreateMessagePolynomial(encodedData);
            CalcDivisionEC();
            return ECBinConversion();
        }

        void CalcDivisionEC()
        {
            // tempList to hold multiplied generator polynomial
            List<int> tempList = new List<int>();

            // counter for how many times to divide 
            int c = 0;

            // times to divide
            int divisionCount = MessagePolynomial.Count;

            // Prepare for division
            for (int j = 0; j < GPSize; j++)
            {
                    MessagePolynomial.Add(0);
            }

            // Coeff of x generator polynomial needs to be multiplied by
            int coeff = MessagePolynomial.Count - GeneratorPolynomial.Count;

            // Prepare for division
            for (int j = 0; j < coeff; j++)
            {
                    GeneratorPolynomial.Add(0);
            }

            // division process
            while (c < divisionCount) 
            {
                

                // Multipler in alpha notation for generator polynomial
                int m = intToExAlpha[MessagePolynomial[0]];
                
                // Multiply generator polynomial
                for (int j = 0; j < GeneratorPolynomial.Count - coeff; j++)
                {

                    tempList.Add(GeneratorPolynomial[j] + m);

                    if (tempList[j] > 255)
                    {
                        tempList[j] %= 255;
                    }

                    tempList[j] = exAlphaToInt[tempList[j]];

                }

                // Add ending 0s
                for (int j = 0; j < MessagePolynomial.Count; j++)
                {
                    tempList.Add(0);
                }

                // XOR polynomials together
                for (int j = 0; j < MessagePolynomial.Count; j++)
                {
                    MessagePolynomial[j] ^= tempList[j];
                }

                
                // removing 0s at beginning of polynomial
                while (MessagePolynomial[0] == 0)
                {
                    MessagePolynomial.RemoveAt(0);
                    c++;
                }
      
                
                // Clear for next use
                tempList.Clear();
            }
        }

        public string ECBinConversion()
        {
            // Holds value of EC Codewords in binary
            string ECBin = "";

            // Convert divided message polynomial to binary 
            for (int i = 0; i < MessagePolynomial.Count; i++)
            {
                ECBin += Convert.ToString(MessagePolynomial[i], 2).PadLeft(8, '0');
            }

            // Clear for next use
            MessagePolynomial.Clear();
            
            return ECBin;
        }

        public void CreateMessagePolynomial(string encodedData)
        {
            // Each byte of data string is converted to decimal and added to message polynomial
            for (int i = 0; i < (encodedData.Length / 8); i++)
            {
                int dataByte = Convert.ToInt32(encodedData.Substring(i * 8, 8), 2);
                MessagePolynomial.Add(dataByte);
            }
        }

        private void CreateGeneratorPolynomial(int degree)
        {
            GeneratorPolynomial = new List<int> { 0, 87, 229, 146, 149, 238, 102, 21 };
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
                for (int j = 0; j < tempList1.Count - 1; j++)
                {
                    tempList1[j] = exAlphaToInt[tempList1[j]];
                    GeneratorPolynomial.Add(intToExAlpha[tempList1[j] ^ tempList2[j]]);
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
            intToExAlpha[0] = -1;
            intToExAlpha[1] = 0;
        }


    }
}
