using Linear_Programming_381.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Linear_Programming_381.Forms
{
    public partial class AddConstraintForm : Form
    {
        private readonly LPModel model;

        private readonly List<TextBox> coefficientTextBoxes = new();

        public List<double> Coefficients{get;private set;} = new();

        public string Relation{get;private set;} = "<=";

        public double RightHandSide{get;private set;}

        private FlowLayoutPanel pnlCoefficients = null!;
        private ComboBox cmbRelation = null!;
        private TextBox txtRhs = null!;

        private Button btnAdd = null!;
        private Button btnCancel = null!;

        public AddConstraintForm(
            LPModel model)
        {
            InitializeComponent();

            this.model =model ??throw new ArgumentNullException(nameof(model));

            SetupConstraintForm();
        }

        private void SetupConstraintForm()
        {
            Text ="Add New Constraint";

            StartPosition =FormStartPosition.CenterParent;

            ClientSize =new Size(500,570);

            AutoScroll = true;

            Controls.Clear();

            Label lblTitle =
                new Label
                {
                    Text = "Add New Constraint",
                    Font = new Font(
                        Font,
                        FontStyle.Bold),
                    AutoSize = true,
                    Left = 20,
                    Top = 20
                };

            Label lblCoefficients =
                new Label
                {
                    Text =
                        "Decision Variable Coefficients:",
                    AutoSize = true,
                    Left = 20,
                    Top = 65
                };

            pnlCoefficients =
                new FlowLayoutPanel
                {
                    Left = 20,
                    Top = 95,
                    Width = 440,
                    Height = 280,

                    FlowDirection =
                        FlowDirection.TopDown,

                    WrapContents = false,
                    AutoScroll = true
                };

            CreateVariableInputs();

            Label lblRelation =
                new Label
                {
                    Text = "Relation:",
                    AutoSize = true,
                    Left = 20,
                    Top = 405
                };

            cmbRelation =
                new ComboBox
                {
                    Left = 180,
                    Top = 400,
                    Width = 150,

                    DropDownStyle =
                        ComboBoxStyle.DropDownList
                };

            cmbRelation.Items.Add("<=");
            cmbRelation.Items.Add(">=");
            cmbRelation.Items.Add("=");

            cmbRelation.SelectedIndex = 0;

            Label lblRhs =
                new Label
                {
                    Text = "Right-Hand-Side:",
                    AutoSize = true,
                    Left = 20,
                    Top = 445
                };

            txtRhs =
                new TextBox
                {
                    Left = 180,
                    Top = 440,
                    Width = 150
                };

            btnAdd =
                new Button
                {
                    Text = "Add Constraint",
                    Left = 200,
                    Top = 490,
                    Width = 130,
                    Height = 35
                };

            btnAdd.Click +=
                btnAdd_Click;

            btnCancel =
                new Button
                {
                    Text = "Cancel",
                    Left = 340,
                    Top = 490,
                    Width = 100,
                    Height = 35
                };

            btnCancel.Click +=
                btnCancel_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblCoefficients);
            Controls.Add(pnlCoefficients);
            Controls.Add(lblRelation);
            Controls.Add(cmbRelation);
            Controls.Add(lblRhs);
            Controls.Add(txtRhs);
            Controls.Add(btnAdd);
            Controls.Add(btnCancel);

            AcceptButton =
                btnAdd;

            CancelButton =
                btnCancel;
        }

        private void CreateVariableInputs()
        {
            coefficientTextBoxes.Clear();

            for (int i = 0;
                 i < model.Variables.Count;
                 i++)
            {
                Panel rowPanel =
                    new Panel
                    {
                        Width = 400,
                        Height = 40
                    };

                Variable variable =
                    model.Variables[i];

                Label label =
                    new Label
                    {
                        Text =
                            $"{variable.Name} coefficient:",

                        AutoSize = true,
                        Left = 0,
                        Top = 10
                    };

                TextBox textBox =
                    new TextBox
                    {
                        Left = 200,
                        Top = 5,
                        Width = 150,
                        Text = "0"
                    };

                coefficientTextBoxes.Add(
                    textBox);

                rowPanel.Controls.Add(
                    label);

                rowPanel.Controls.Add(
                    textBox);

                pnlCoefficients.Controls.Add(
                    rowPanel);
            }
        }

        private void btnAdd_Click(
            object? sender,
            EventArgs e)
        {
            List<double> coefficients =
                new();

            for (int i = 0;
                 i < coefficientTextBoxes.Count;
                 i++)
            {
                TextBox textBox =
                    coefficientTextBoxes[i];

                if (!double.TryParse(
                        textBox.Text,
                        out double coefficient))
                {
                    MessageBox.Show(
                        $"Please enter a valid coefficient " +
                        $"for {model.Variables[i].Name}.",
                        "Invalid Constraint",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    textBox.Focus();

                    return;
                }

                coefficients.Add(
                    coefficient);
            }

            if (cmbRelation.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a constraint relation.",
                    "Invalid Constraint",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!double.TryParse(
                    txtRhs.Text,
                    out double rhs))
            {
                MessageBox.Show(
                    "Please enter a valid right-hand-side value.",
                    "Invalid Constraint",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtRhs.Focus();

                return;
            }

            Coefficients =
                coefficients;

            Relation =
                cmbRelation.SelectedItem.ToString()!;

            RightHandSide =
                rhs;

            DialogResult =
                DialogResult.OK;

            Close();
        }

        private void btnCancel_Click(
            object? sender,
            EventArgs e)
        {
            DialogResult =
                DialogResult.Cancel;

            Close();
        }
    }
}
