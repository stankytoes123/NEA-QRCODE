using Microsoft.VisualBasic;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
namespace NEA_QRCODE
{
    internal static class Program
    {
        [STAThread]

        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            
            const int size = 31;
            
            // Create a 2D grid of boolean values to represent QR code grid
            // 0 = White, 1 = Black, 2 = Reserved White, 3 = Reserved Black
            int[,] GridQR = new int[size, size];
            
            Form form = new Form1()
            {
                Size = new Size(1000, 800),
                BackColor = Color.Gray
            };

            // Temporary for debugging
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    GridQR[i, j] = 0;
                }
            }

           
            
            GridQR[9, 22] = 3; // Black module
            PlaceFinderPattern(GridQR, 1, 1);
            PlaceFinderPattern(GridQR, 1, size - 8);
            PlaceFinderPattern(GridQR, size - 8, 1);
            PlaceAlignmentPattern(GridQR, 21, 21);
            PlaceTimingStrips(GridQR);
            CreateGrid(size, form, GridQR);

            Application.Run(form);
            String URL = Inputs(form);
            StringToCodeBinary(URL);
           
        }
        static String Inputs(Form form)
        {
            String URL = "";

            TextBox inputBox = new TextBox()
            {
                Location = new Point(100, 700),
                Width = 530,
                Height = 30
            };

            Button GenerateQR = new Button()
            {
                Location = new Point(650, 693),
                Height = 40,
                Width = 80,
                BackColor = Color.White,
                Text = "Generate"
            };

            GenerateQR.Click += (sender, e) =>
            {
                URL = inputBox.Text;
            };

            form.Controls.Add(inputBox);
            form.Controls.Add(GenerateQR);

            return URL;
        }

        

        static void PlaceFinderPattern(int[,] GridQR, int StartX, int StartY)
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

        static void PlaceAlignmentPattern(int[,] GridQR, int StartX, int StartY)
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

        static void PlaceTimingStrips(int[,] GridQR) 
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


        // Converts a character to it's corresponding ASCII 8-bit binary number
        static String StringToCodeBinary(String URL) 
        {
            Encoding iso88591 = Encoding.GetEncoding("ISO-8859-1");

            byte[] isoBytes = iso88591.GetBytes(URL);

            StringBuilder binaryBuilder = new StringBuilder();
            
            binaryBuilder.Append("0100");

            foreach (byte b in isoBytes)
            {
                binaryBuilder.Append(Convert.ToString(b, 2).PadLeft(8, '0')); // Convert to binary, pad with leading 0s
            }

            MessageBox.Show(binaryBuilder.ToString());

            return binaryBuilder.ToString();
        }

        // Creates a grid
        static void CreateGrid(int size, Form form, int[,] GridQR)
        {

            // Create a TableLayoutPanel to hold the panels
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel()
            {
                RowCount = size,
                ColumnCount = size,
                AutoSize = true,
                Location = new Point(
                    form.ClientSize.Width / 2 - 400, 
                    form.ClientSize.Height / 2 - 279 // Vertically center the TableLayoutPanel
                ),
            };

            // Loop to add panels to the TableLayoutPanel
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // Create a new panel with a black background
                    Panel colourPanel = new Panel()
                    {
                        Size = new Size(18, 18),               // Set panel size
                        BorderStyle = BorderStyle.FixedSingle, // Temporary border for debugging
                        Margin = new Padding(0),               // Remove margins
                        Padding = new Padding(0)               // Remove padding
                    };

                    if (GridQR[i, j] == 0 || GridQR[i, j] == 2)
                    {
                        colourPanel.BackColor = Color.White;
                    }
                    else
                    {
                        colourPanel.BackColor = Color.Black;
                    }

                    // Add the panel to the table layout at the appropriate row and column
                    tableLayoutPanel.Controls.Add(colourPanel, i, j);
                }
            }

            // Add the TableLayoutPanel to the form
            form.Controls.Add(tableLayoutPanel);
        }
    }
}