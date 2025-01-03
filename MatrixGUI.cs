using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NEA_QRCODE
{
    class MatrixGUI : MatrixArray
    {
        private Form QRForm;
        private TextBox inputBox;
        private Button generateQRButton;
        private TableLayoutPanel tableLayoutPanel;
        public MatrixGUI(int size, int whiteSpace, int[,] GridQR)
        {
            QRForm = CreateForm();
            inputBox = CreateInputBox();
            generateQRButton = CreateGenerateQRButton();
            AddToForm(QRForm, inputBox, generateQRButton);
            tableLayoutPanel = CreateGrid(size + whiteSpace, QRForm, GridQR);
            generateQRButton.Click += (sender, e) =>
            {
                if (string.IsNullOrEmpty(inputBox.Text))
                {
                    MessageBox.Show("Please enter URL before generating");
                }
                else
                {
                    string input = inputBox.Text;

                    ClearTextBox(inputBox);

                    GenerateQRCode(input, GridQR, size, whiteSpace, QRForm);

                    ClearTableLayout();

                    CreateGrid(size + whiteSpace, QRForm, GridQR);
                }
            };
            Application.Run(QRForm);
        }


        private void ClearTextBox(TextBox inputBox)
        {
            inputBox.Clear();
        }

        private void ClearTableLayout()
        {
            tableLayoutPanel.Dispose();            
        }


        private void AddToForm(Form form, TextBox inputBox, Button generateQRButton)
        {
            form.Controls.Add(inputBox);
            form.Controls.Add(generateQRButton);
        }

        private TextBox CreateInputBox()
        {
            return new TextBox
            {
                Location = new Point(100, 700),
                Width = 530,
                Height = 30
            };
        }

        private Button CreateGenerateQRButton()
        {
            return new Button
            {
                Location = new Point(650, 693),
                Height = 40,
                Width = 80,
                BackColor = Color.White,
                Text = "Generate"
            };
        }
        private Form CreateForm()
        {
            return new Form
            {
                Text = "QR Code Generator",
                Size = new Size(1000, 800),
                BackColor = Color.Gray,
            };
            
        }
        private TableLayoutPanel CreateGrid(int whiteSpaceSize, Form form, int[,] GridQR)
        {
            
            
            // Create a TableLayoutPanel to hold the panels
            tableLayoutPanel = new TableLayoutPanel()
            {
                RowCount = whiteSpaceSize,
                ColumnCount = whiteSpaceSize,
                AutoSize = true,
                Location = new Point(
                    form.ClientSize.Width / 2 - 400,
                    form.ClientSize.Height / 2 - 279 // Vertically center the TableLayoutPanel
                ),
            };

            

            // Loop to add panels to the TableLayoutPanel
            for (int i = 0; i < whiteSpaceSize; i++)
            {
                for (int j = 0; j < whiteSpaceSize; j++)
                {
                    // Create a new panel with a black background
                    Panel colourPanel = new Panel()
                    {
                        Size = new Size(18, 18),               // Set panel size
                        //BorderStyle = BorderStyle.FixedSingle, // Temporary border for debugging
                        Margin = new Padding(0),               // Remove margins
                        Padding = new Padding(0)               // Remove padding
                    };

                    if (GridQR[i, j] == 0 || GridQR[i, j] == 2)
                    {
                        colourPanel.BackColor = Color.White;
                    }
                    else if (GridQR[i, j] == 1 || GridQR[i, j] == 3)
                    {
                        colourPanel.BackColor = Color.Black;
                    }
                    else
                    {
                        colourPanel.BackColor = Color.Blue;
                    }

                    // Add the panel to the table layout at the appropriate row and column
                    tableLayoutPanel.Controls.Add(colourPanel, i, j);
                }
            }



            form.Controls.Add(tableLayoutPanel);

            return tableLayoutPanel;
            
        }
    }
}
