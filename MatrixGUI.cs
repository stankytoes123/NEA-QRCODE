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

        private TableLayoutPanel tableLayoutPanel;
        public MatrixGUI(int size, int whiteSpace, int[,] GridQR)
        {
            Form form = CreateForm();
            TextBox inputBox = CreateInputBox();
            Button generateQRButton = CreateGenerateQRButton();
            AddToForm(form, inputBox, generateQRButton);
            tableLayoutPanel = CreateGrid(size + whiteSpace, form, GridQR);
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

                    ClearTableLayout(form);

                    GenerateQRCode(input, GridQR, size, whiteSpace, form);

                    CreateGrid(size + whiteSpace, form, GridQR);

                }
            };
            Application.Run(form);
        }

        private void ClearTextBox(TextBox inputBox)
        {
            inputBox.Clear();
        }

        private void ClearTableLayout(Form form)
        {
            form.Controls.Remove(tableLayoutPanel);
        }


        void AddToForm(Form form, TextBox inputBox, Button generateQRButton)
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
            return new Form1
            {
                Size = new Size(1000, 800),
                BackColor = Color.Gray,
            };
        }
        public TableLayoutPanel CreateGrid(int whiteSpaceSize, Form form, int[,] GridQR)
        {

            // Create a TableLayoutPanel to hold the panels
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel()
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
                        BorderStyle = BorderStyle.FixedSingle, // Temporary border for debugging
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
