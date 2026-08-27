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
    public partial class ObjectiveCoefficientChangeForm : Form
    {
        private readonly LPModel model;

        public string SelectedVariable
        {
            get;
            private set;
        } = "";

        public double NewCoefficient
        {
            get;
            private set;
        }

        private ComboBox cmbVariable = null!;
        private TextBox txtCurrentCoefficient = null!;
        private TextBox txtNewCoefficient = null!;

        private Button btnApply = null!;
        private Button btnCancel = null!;

        public ObjectiveCoefficientChangeForm(
            LPModel model)
        {
            InitializeComponent();

            this.model =
                model ??
                throw new ArgumentNullException(
                    nameof(model));

            SetupForm();
        }

        private void SetupForm()
        {
            Text =
                "Change Objective Coefficient";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(
                    450,
                    280);

            Controls.Clear();

            Label lblTitle =
                new Label
                {
                    Text =
                        "Apply Objective Coefficient Change",

                    Font =
                        new Font(
                            Font,
                            FontStyle.Bold),

                    AutoSize = true,
                    Left = 20,
                    Top = 20
                };

            Label lblVariable =
                new Label
                {
                    Text =
                        "Decision Variable:",

                    AutoSize = true,
                    Left = 20,
                    Top = 75
                };

            cmbVariable =
                new ComboBox
                {
                    Left = 190,
                    Top = 70,
                    Width = 200,
                    DropDownStyle =
                        ComboBoxStyle.DropDownList
                };

            foreach (Variable variable
                     in model.Variables)
            {
                cmbVariable.Items.Add(
                    variable.Name);
            }

            cmbVariable.SelectedIndexChanged +=
                cmbVariable_SelectedIndexChanged;

            Label lblCurrent =
                new Label
                {
                    Text =
                        "Current Coefficient:",

                    AutoSize = true,
                    Left = 20,
                    Top = 115
                };

            txtCurrentCoefficient =
                new TextBox
                {
                    Left = 190,
                    Top = 110,
                    Width = 200,
                    ReadOnly = true
                };

            Label lblNew =
                new Label
                {
                    Text =
                        "New Coefficient:",

                    AutoSize = true,
                    Left = 20,
                    Top = 155
                };

            txtNewCoefficient =
                new TextBox
                {
                    Left = 190,
                    Top = 150,
                    Width = 200
                };

            btnApply =
                new Button
                {
                    Text =
                        "Apply Change",

                    Left = 170,
                    Top = 210,
                    Width = 120,
                    Height = 35
                };

            btnApply.Click +=
                btnApply_Click;

            btnCancel =
                new Button
                {
                    Text =
                        "Cancel",

                    Left = 300,
                    Top = 210,
                    Width = 90,
                    Height = 35
                };

            btnCancel.Click +=
                btnCancel_Click;

            Controls.Add(
                lblTitle);

            Controls.Add(
                lblVariable);

            Controls.Add(
                cmbVariable);

            Controls.Add(
                lblCurrent);

            Controls.Add(
                txtCurrentCoefficient);

            Controls.Add(
                lblNew);

            Controls.Add(
                txtNewCoefficient);

            Controls.Add(
                btnApply);

            Controls.Add(
                btnCancel);

            AcceptButton =
                btnApply;

            CancelButton =
                btnCancel;

            if (cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }
        }

        private void cmbVariable_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (cmbVariable.SelectedItem == null)
            {
                return;
            }

            string variableName =
                cmbVariable.SelectedItem.ToString()!;

            Variable? variable =
                model.Variables.Find(
                    v => v.Name == variableName);

            if (variable == null)
            {
                return;
            }

            txtCurrentCoefficient.Text =
                variable
                    .GetSignedObjectiveCoefficient()
                    .ToString("0.###");

            txtNewCoefficient.Text =
                txtCurrentCoefficient.Text;
        }

        private void btnApply_Click(
            object? sender,
            EventArgs e)
        {
            if (cmbVariable.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a decision variable.",
                    "Invalid Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!double.TryParse(
                    txtNewCoefficient.Text,
                    out double newCoefficient))
            {
                MessageBox.Show(
                    "Please enter a valid new coefficient.",
                    "Invalid Coefficient",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNewCoefficient.Focus();

                return;
            }

            SelectedVariable =
                cmbVariable.SelectedItem.ToString()!;

            NewCoefficient =
                newCoefficient;

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
