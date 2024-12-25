using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
namespace NEA_QRCODE
{
    internal static class Program
    {
        [STAThread]
        // https://www.youtube.com/watch?v=w5ebcowAJD8&t=0s
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            int version = 3;
          
            int size = 17 + version * 4;

            // Size of QR Code + 2 for whitespace
            int whiteSpace = 2;

            // Create a 2D grid of boolean values to represent QR code grid
            // 0 = White, 1 = Black, 2 = Reserved White, 3 = Reserved Black
            int[,] GridQR = new int[size + whiteSpace, size + whiteSpace];



            ErrorCorrection errorCorrection = new ErrorCorrection();

            MatrixArray generateMatrixArray = new MatrixArray();

            MatrixGUI generateMatrixGUI = new MatrixGUI(size, whiteSpace, GridQR);
           
        }
    }
}