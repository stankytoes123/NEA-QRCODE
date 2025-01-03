using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_QRCODE
{
    class MatrixArray : ErrorCorrection
    {

        // Place the reserved QR Code Patterns
        void PlaceReservedPatterns(int[,] GridQR, int size, int whiteSpace)
        {
            PlaceWhiteSpace(GridQR, size + whiteSpace / 2);
            PlaceFinderPattern(GridQR, 0 + whiteSpace / 2, 0 + whiteSpace / 2);
            PlaceFinderPattern(GridQR, 0 + whiteSpace / 2, size - 8 + whiteSpace);
            PlaceFinderPattern(GridQR, size - 8 + whiteSpace, 0 + whiteSpace / 2);
            PlaceSeperators(GridQR, 0 + whiteSpace / 2, size - whiteSpace / 2);
            PlaceAlignmentPattern(GridQR, 19 + whiteSpace, 19 + whiteSpace);
            PlaceTimingStrips(GridQR, 7 + whiteSpace, size - 5 - whiteSpace);
            PlaceBlackModule(GridQR, 8 + whiteSpace / 2, size - 6 - whiteSpace / 2);
            PlaceFormatStrips(GridQR, 0 + whiteSpace / 2, size + whiteSpace / 2);
        }

        public void GenerateQRCode(string input, int[,] GridQR, int size, int whiteSpace, Form form)
        {

            string binaryCodeWords = FinalString(input);

            int a = binaryCodeWords.Length;

            PlaceReservedPatterns(GridQR, size, whiteSpace);

            PlaceDataAndECCodewords(binaryCodeWords, GridQR, size, size, size + whiteSpace / 2);

            Mask0(size + whiteSpace, GridQR);

        }

        void Mask0(int size, int[,] GridQR)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if ((i + j) % 2 == 0 && GridQR[i, j] != 2 && GridQR[i, j] != 3)
                    {
                        GridQR[i, j] = (GridQR[i, j] == 0) ? 1 : 0;
                    }
                }
            }
        }

        void Mask1(int size, int[,] GridQR)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if ((i % 2 == 0) && GridQR[i, j] != 2 && GridQR[i, j] != 3 )                  
                    {
                        GridQR[i, j] = (GridQR[i, j] == 0) ? 1 : 0;
                    }
                }
            }
        }

        void Mask7(int size, int[,] GridQR)
        {
            for (int i = 1; i < size - 1; i++)
            {
                for (int j = 1; j < size - 1; j++)
                {
                    int col = i - 1;
                    int row = j - 1;

                    
                    if (((((col + row) % 2) +((col * row) % 3)) % 2) == 0 && GridQR[i, j] != 2 && GridQR[i, j] != 3)
                    {
                        GridQR[i, j] = (GridQR[i, j] == 0) ? 1 : 0;
                    }
                }
            }
        }




        void PlaceDataAndECCodewords(string input, int[,] GridQR, int currentX, int currentY, int border)
        {
            int c = 0;
            bool upOrDown = true;

            while (c < input.Length)     
            {
                if (upOrDown == true & currentY > 0)
                {
                    if (GridQR[currentX, currentY] == 0 || GridQR[currentX, currentY] == 1)
                    {
                        GridQR[currentX, currentY] = Convert.ToInt32(input.Substring(c, 1));
                        c++;
                    }

                    if (GridQR[currentX - 1, currentY] == 0 || GridQR[currentX - 1, currentY] == 1)
                    {
                        GridQR[currentX - 1, currentY] = Convert.ToInt32(input.Substring(c, 1));
                        c++;
                    }
                    currentY--;
                }
                else if (upOrDown == false & currentY < border)
                {
                    if (GridQR[currentX, currentY] == 0 || GridQR[currentX, currentY] == 1)
                    {
                        GridQR[currentX, currentY] = Convert.ToInt32(input.Substring(c, 1));
                        c++;
                    }

                    if (GridQR[currentX - 1, currentY] == 0 || GridQR[currentX - 1, currentY] == 1)
                    {
                        GridQR[currentX - 1, currentY] = Convert.ToInt32(input.Substring(c, 1));
                        c++;
                    }
                    currentY++;
                }
                else
                {
                    if (upOrDown == true && currentX == 9)
                    {
                        upOrDown = false;
                        currentX -= 3;
                        currentY++;
                    }
                    else if (upOrDown)
                    {
                        upOrDown = false;
                        currentX -= 2;
                        currentY++;
                    }
                    else
                    {
                        upOrDown = true;
                        currentX -= 2;
                        currentY--;
                    }
                }
            }


        }
        
    


        string FinalString(string input)
        {
            return StringToCodeBinary(input) + CreateECCodewords(StringToCodeBinary(input)) + RemainderBits(3);
        }

        string StringToCodeBinary(string input)
        {

            return  "0100" + CharCountIndicator(input) + StringToISO88591(input) + 
                     TerminatorStringCalc(input) + MOf8(input) + PadBytes(input);
        }

        string RemainderBits(int version)
        {
            if (version > 1)
            {
                return new string('0', 7);
            }
            else
            {
                return "";
            }
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
            for (int i = 0; i < (440 - temp.Length) / 8; i++)
            {
                if (i % 2 == 0)
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

        void PlaceWhiteSpace(int[,] GridQR, int border)
        {
            for (int i = 0; i <= border; i++)
            {
                GridQR[0, border - i] = 2;
                GridQR[border - i, 0] = 2;
                GridQR[0 + i, border] = 2;
                GridQR[border, 0 + i] = 2;
            }
        }

        void PlaceSeperators(int[,] GridQR, int startLoc, int endLoc)
        {
            for (int i = 0; i < 8; i++)
            {
                GridQR[startLoc + i, startLoc + 7] = 2;
                GridQR[startLoc + i, endLoc - 6] = 2;
                GridQR[endLoc - 6 + i, startLoc + 7] = 2;
                GridQR[startLoc + 7, startLoc + i] = 2;
                GridQR[endLoc - 6, startLoc + i] = 2;
                GridQR[startLoc + 7 , endLoc - 6 + i] = 2;
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

        void PlaceTimingStrips(int[,] GridQR, int startLoc, int endLoc)
        {
            for (int i = startLoc; i < endLoc; i++)
            {
               if (i % 2 == 1)
               {
                    GridQR[7, i] = 3; // 3
                    GridQR[i, 7] = 3;
               }
               else 
               {
                    GridQR[7, i] = 2;
                    GridQR[i, 7] = 2;
               }
            }
        }

        void PlaceFormatStrips(int[,] GridQR, int startLoc, int endLoc)
        {

            string fString = CreateFormatString();
            int xCounter = 0;
            int yCounter = 0;

            for (int i = startLoc; i < endLoc; i++)
            {

                bool validX = ((i >= startLoc && i <= startLoc + 7) || i >= endLoc - 8) && i != 7;
                bool validY = ((i >= startLoc && i <= startLoc + 6) || i >= endLoc - 9) && i != endLoc - 7;

                if (validX)
                {
                    GridQR[i, 9] = (fString.Substring(xCounter, 1) == "1") ? 3 : 2; 
                    xCounter++;
                }

                if (validY)
                {
                    GridQR[9, endLoc - i] = (fString.Substring(yCounter, 1) == "1") ? 3 : 2;
                    yCounter++;
                }

            }
        }

        void PlaceBlackModule(int[,] GridQR, int x, int y)
        {
            GridQR[x, y] = 3;
        }
    }
}
