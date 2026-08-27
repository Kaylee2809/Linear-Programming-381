using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Linear_Programming_381.Models;

namespace Linear_Programming_381.Forms
{
    public partial class AddActivityForm : Form
    {
        private readonly LPModel model;

        private readonly List<TextBox>
            coefficientTextBoxes = new();

        public string VariableName
        {
            get;
            private set;
        } = "";

        public double ObjectiveCoefficient
        {
            get;
            private set;
        }

        public List<double> ConstraintCoefficients
        {
            get;
            private set;
        } = new();

        private TextBox txtVariableName = null!;
        private TextBox txtObjectiveCoefficient = null!;

        private FlowLayoutPanel pnlCoefficients = null!;

        private Button btnAdd = null!;
        private Button btnCancel = null!;

        public AddActivityForm(
            LPModel model)
        {
            InitializeComponent();

            this.model =
                model ??
                throw new ArgumentNullException(
                    nameof(model));

            SetupActivityForm();
        }

        private void SetupActivityForm()
        {
            Text =
                "Add New Activity";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(
                    480,
                    550);

            AutoScroll = true;

            Controls.Clear();

            Label lblTitle =
                new Label
                {
                    Text =
                        "Add New Activity",

                    Font =
                        new Font(
                            Font,
                            FontStyle.Bold),

                    AutoSize = true,
                    Left = 20,
                    Top = 20
                };

            Label lblVariableName =
                new Label
                {
                    Text =
                        "Variable Name:",

                    AutoSize = true,
                    Left = 20,
                    Top = 65
                };

            txtVariableName =
                new TextBox
                {
                    Left = 190,
                    Top = 60,
                    Width = 220
                };

            Label lblObjective =
                new Label
                {
                    Text =
                        "Objective Coefficient:",

                    AutoSize = true,
                    Left = 20,
                    Top = 105
                };

            txtObjectiveCoefficient =
                new TextBox
                {
                    Left = 190,
                    Top = 100,
                    Width = 220
                };

            Label lblConstraints =
                new Label
                {
                    Text =
                        "Constraint Coefficients:",

                    AutoSize = true,
                    Left = 20,
                    Top = 145
                };

            pnlCoefficients =
                new FlowLayoutPanel
                {
                    Left = 20,
                    Top = 175,
                    Width = 420,
                    Height = 260,

                    FlowDirection =
                        FlowDirection.TopDown,

                    WrapContents = false,
                    AutoScroll = true
                };

            CreateConstraintInputs();

            btnAdd =
                new Button
                {
                    Text =
                        "Add Activity",

                    Left = 180,
                    Top = 465,
                    Width = 120,
                    Height = 35
                };

            btnAdd.Click +=
                btnAdd_Click;

            btnCancel =
                new Button
                {
                    Text =
                        "Cancel",

                    Left = 310,
                    Top = 465,
                    Width = 100,
                    Height = 35
                };

            btnCancel.Click +=
                btnCancel_Click;

            Controls.Add(
                lblTitle);

            Controls.Add(
                lblVariableName);

            Controls.Add(
                txtVariableName);

            Controls.Add(
                lblObjective);

            Controls.Add(
                txtObjectiveCoefficient);

            Controls.Add(
                lblConstraints);

            Controls.Add(
                pnlCoefficients);

            Controls.Add(
                btnAdd);

            Controls.Add(
                btnCancel);

            AcceptButton =
                btnAdd;

            CancelButton =
                btnCancel;
        }

        private void CreateConstraintInputs()
        {
            coefficientTextBoxes.Clear();

            for (int i = 0;
                 i < model.Constraints.Count;
                 i++)
            {
                Panel rowPanel =
                    new Panel
                    {
                        Width = 390,
                        Height = 40
                    };

                Linear_Programming_381.Models.Constraint constraint =
                    model.Constraints[i];

                Label label =
                    new Label
                    {
                        Text =
                            $"Constraint {i + 1} " +
                            $"({constraint.Relation}):",

                        AutoSize = true,
                        Left = 0,
                        Top = 10
                    };

                TextBox textBox =
                    new TextBox
                    {
                        Left = 210,
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
            string variableName =
                txtVariableName.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    variableName))
            {
                MessageBox.Show(
                    "Please enter a variable name.",
                    "Invalid Activity",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtVariableName.Focus();

                return;
            }

            foreach (Variable variable
                     in model.Variables)
            {
                if (variable.Name.Equals(
                        variableName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        $"Variable {variableName} already exists.",
                        "Invalid Activity",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtVariableName.Focus();

                    return;
                }
            }

            if (!double.TryParse(
                    txtObjectiveCoefficient.Text,
                    out double objectiveCoefficient))
            {
                MessageBox.Show(
                    "Please enter a valid objective coefficient.",
                    "Invalid Activity",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtObjectiveCoefficient.Focus();

                return;
            }

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
                        $"for Constraint {i + 1}.",
                        "Invalid Activity",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    textBox.Focus();

                    return;
                }

                coefficients.Add(
                    coefficient);
            }

            VariableName =
                variableName;

            ObjectiveCoefficient =
                objectiveCoefficient;

            ConstraintCoefficients =
                coefficients;

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
