using Microsoft.VisualBasic;
using System.Runtime.Intrinsics.X86;
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

            int size = 31;

            // Create a 2D grid of boolean values to represent QR code grid
            // 0 = White, 1 = Black, 2 = Reserved White, 3 = Reserved Black
            int[,] GridQR = new int[size, size];

            Form form = new Form()
            {
                Size = new Size(1000, 800),
                BackColor = Color.Gray,
                
                
            };

            //0100

            GenerateMatrixArray generateMatrixArray = new GenerateMatrixArray();

            GenerateMatrixGUI generateMatrixGUI = new GenerateMatrixGUI(size, GridQR, form);

            Application.Run(form);

        }
     
    }
}