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

            GenerateMatrixArray generateMatrixArray = new GenerateMatrixArray();

            GenerateMatrixGUI generateMatrixGUI = new GenerateMatrixGUI(size, GridQR);

        }
    }
}