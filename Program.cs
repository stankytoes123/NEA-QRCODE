using Microsoft.VisualBasic;
using System.Windows.Forms;
namespace NEA_QRCODE
{
    internal static class Program
    {
        [STAThread]

        static void Main()
        {
            ApplicationConfiguration.Initialize();

            const int width = 25;
            const int height = 25;

            Form form = new Form1();

            InitializeGrid(width, height, form);
            Application.Run(form);
        }
        static String CharToBinary(char character)
        {
            return Convert.ToString(Convert.ToInt32(character), 2);
        }

        static void InitializeGrid(int width, int height, Form form)
        {
            // Create a 2D grid of boolean values to represent QR code grid
            bool[,] GridQR = new bool[width, height];

            // Create a TableLayoutPanel to hold the panels
            var tableLayoutPanel = new TableLayoutPanel()
            {
                RowCount = width,
                ColumnCount = height,
                AutoSize = true,
                Location = new Point(
                    form.ClientSize.Width / 2 - 200, // Horizontally center the TableLayoutPanel
                    form.ClientSize.Height / 2 - 200 // Vertically center the TableLayoutPanel
                ),
            };

            // Loop to add panels to the TableLayoutPanel
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // Create a new panel with a black background
                    Panel colourPanel = new Panel()
                    {
                        BackColor = Color.Black, // Set panel color to black
                        Size = new Size(16, 16), // Set panel size
                        Margin = new Padding(0), // Remove margins
                        Padding = new Padding(0) // Remove padding
                    };

                    // Add the panel to the table layout at the appropriate row and column
                    tableLayoutPanel.Controls.Add(colourPanel, col, row);
                }
            }

            // Add the TableLayoutPanel to the form
            form.Controls.Add(tableLayoutPanel);
        }
    }
}