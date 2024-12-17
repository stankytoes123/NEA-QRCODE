using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_QRCODE
{
    class GenerateMatrixArray
    {
        
        void PlaceConstPatterns(int[,] GridQR, int size)
        {
            PlaceFinderPattern(GridQR, 1, 1);
            PlaceFinderPattern(GridQR, 1, size - 8);
            PlaceFinderPattern(GridQR, size - 8, 1);
            PlaceAlignmentPattern(GridQR, 21, 21);
            PlaceTimingStrips(GridQR);
            PlaceBlackModule(GridQR);
        }

        public void GenerateQRCode(TextBox inputBox, int[,] GridQR, int size, Form form)
        {
            string inputCode = inputBox.Text;
            if (string.IsNullOrEmpty(inputCode))
            {
                MessageBox.Show("Please enter URL before generating");
            }
            else
            {
                string binaryCode = StringToCodeBinary(inputCode);

                inputBox.Clear();

                PlaceConstPatterns(GridQR, size);

                
            }

        }

        void PlaceStringPattern(int size, int[,] GridQR, string binaryCode)
        {

        }

        string StringToISO88591(string input)
        {
            Encoding iso88591 = Encoding.GetEncoding("ISO-8859-1");

            byte[] isoBytes = iso88591.GetBytes(input);

            StringBuilder binaryBuilder = new StringBuilder();

            foreach (byte b in isoBytes)
            {
                binaryBuilder.Append(Convert.ToString(b, 2).PadLeft(8, '0')); // Convert to binary, pad with leading 0s
            }

            return binaryBuilder.ToString();
        }
        string TerminatorStringCalc(string input)
        {
            string temp = "0100" + CharCountIndicator(input) + StringToISO88591(input);
            if (440 - temp.Length > 4)
            {
                return "0000";
            }
            return new string('0', temp.Length);
        }

        string MOf8(string input)
        {
            string temp = "0100" + CharCountIndicator(input) + StringToISO88591(input) + TerminatorStringCalc(input);
            if (temp.Length % 8 == 0)
            {
                return "";
            }
            else
            {
                return new string('0' , 8 - (temp.Length % 8));
            }
            
        }

        string PadBytes(string input)
        {
            StringBuilder binaryBuilder = new StringBuilder();

            string temp = "0100" + CharCountIndicator(input) + StringToISO88591(input) + TerminatorStringCalc(input) + MOf8(input);
            for (int i = 1; i <= (440 - temp.Length) / 8; i++)
            {
                if (i % 2 == 1)
                {
                    binaryBuilder.Append("11101100");
                }
                else
                {
                    binaryBuilder.Append("00010001");
                }
            }
            return binaryBuilder.ToString();
        }

        string CharCountIndicator(string input)
        {
            return Convert.ToString(input.Length, 2).PadLeft(8, '0');
        }


        string StringToCodeBinary(string input)
        {
            return  "0100" + CharCountIndicator(input) + StringToISO88591(input) + 
                     TerminatorStringCalc(input) + MOf8(input) + PadBytes(input);
        }

        void PlaceFinderPattern(int[,] GridQR, int StartX, int StartY)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    bool outerBorder = (i == 0 || i == 6 || j == 0 || j == 6);
                    bool innerBorder = (i >= 2 && i <= 4 && j >= 2 && j <= 4);
                    bool center = (i == 3 && j == 3);

                    if (outerBorder || innerBorder || center)
                    {
                        GridQR[StartX + i, StartY + j] = 3;
                    }
                    else
                    {
                        GridQR[StartX + i, StartY + j] = 2;
                    }
                }
            }
        }

        void PlaceAlignmentPattern(int[,] GridQR, int StartX, int StartY)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    bool outerBorder = (i == 0 || i == 4 || j == 0 || j == 4);
                    bool center = (i == 2 && j == 2);

                    if (outerBorder || center)
                    {
                        GridQR[StartX + i, StartY + j] = 3;
                    }
                    else
                    {
                        GridQR[StartX + i, StartY + j] = 2;
                    }
                }
            }
        }

        void PlaceTimingStrips(int[,] GridQR)
        {
            for (int i = 8; i <= 22; i++)
            {
                if (i % 2 == 1)
                {
                    GridQR[7, i] = 3;
                    GridQR[i, 7] = 3;
                }
                else
                {
                    GridQR[7, i] = 2;
                    GridQR[i, 7] = 2;
                }
            }
        }

        void PlaceBlackModule(int[,] GridQR)
        {
            GridQR[9, 22] = 3;
        }
    }
}
