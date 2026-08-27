using Linear_Programming_381.Core;
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
    public partial class NonBasicColumnChangeForm : Form
    {
        private readonly LPModel model;
        private readonly Tableau tableau;

        public string SelectedVariable
        {
            get;
            private set;
        } = "";

        public int SelectedConstraintIndex
        {
            get;
            private set;
        }

        public double NewCoefficient
        {
            get;
            private set;
        }

        private ComboBox cmbVariable = null!;
        private ComboBox cmbConstraint = null!;

        private TextBox txtCurrentCoefficient = null!;
        private TextBox txtNewCoefficient = null!;

        private Button btnApply = null!;
        private Button btnCancel = null!;

        public NonBasicColumnChangeForm(
            LPModel model,
            Tableau tableau)
        {
            InitializeComponent();

            this.model =
                model ??
                throw new ArgumentNullException(
                    nameof(model));

            this.tableau =
                tableau ??
                throw new ArgumentNullException(
                    nameof(tableau));

            SetupForm();
        }

        private void SetupForm()
        {
            Text =
                "Change Non-Basic Column Coefficient";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(
                    520,
                    340);

            Controls.Clear();

            Label lblTitle =
                new Label
                {
                    Text =
                        "Apply Non-Basic Column Coefficient Change",

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
                        "Non-Basic Variable:",

                    AutoSize = true,
                    Left = 20,
                    Top = 75
                };

            cmbVariable =
                new ComboBox
                {
                    Left = 210,
                    Top = 70,
                    Width = 250,

                    DropDownStyle =
                        ComboBoxStyle.DropDownList
                };

            foreach (Variable variable
                     in model.Variables)
            {
                if (!tableau.BasicVariables.Contains(
                        variable.Name))
                {
                    cmbVariable.Items.Add(
                        variable.Name);
                }
            }

            cmbVariable.SelectedIndexChanged +=
                SelectionChanged;

            Label lblConstraint =
                new Label
                {
                    Text =
                        "Constraint:",

                    AutoSize = true,
                    Left = 20,
                    Top = 120
                };

            cmbConstraint =
                new ComboBox
                {
                    Left = 210,
                    Top = 115,
                    Width = 250,

                    DropDownStyle =
                        ComboBoxStyle.DropDownList
                };

            for (int i = 0;
                 i < model.Constraints.Count;
                 i++)
            {
                Linear_Programming_381.Models.Constraint constraint =
                    model.Constraints[i];

                cmbConstraint.Items.Add(
                    $"Constraint {i + 1} " +
                    $"({constraint.Relation})");
            }

            cmbConstraint.SelectedIndexChanged +=
                SelectionChanged;

            Label lblCurrent =
                new Label
                {
                    Text =
                        "Current Coefficient:",

                    AutoSize = true,
                    Left = 20,
                    Top = 165
                };

            txtCurrentCoefficient =
                new TextBox
                {
                    Left = 210,
                    Top = 160,
                    Width = 250,
                    ReadOnly = true
                };

            Label lblNew =
                new Label
                {
                    Text =
                        "New Coefficient:",

                    AutoSize = true,
                    Left = 20,
                    Top = 210
                };

            txtNewCoefficient =
                new TextBox
                {
                    Left = 210,
                    Top = 205,
                    Width = 250
                };

            btnApply =
                new Button
                {
                    Text =
                        "Apply Change",

                    Left = 240,
                    Top = 265,
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

                    Left = 370,
                    Top = 265,
                    Width = 90,
                    Height = 35
                };

            btnCancel.Click +=
                btnCancel_Click;

            Controls.Add(lblTitle);
            Controls.Add(lblVariable);
            Controls.Add(cmbVariable);
            Controls.Add(lblConstraint);
            Controls.Add(cmbConstraint);
            Controls.Add(lblCurrent);
            Controls.Add(txtCurrentCoefficient);
            Controls.Add(lblNew);
            Controls.Add(txtNewCoefficient);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);

            AcceptButton =
                btnApply;

            CancelButton =
                btnCancel;

            if (cmbVariable.Items.Count > 0)
            {
                cmbVariable.SelectedIndex = 0;
            }

            if (cmbConstraint.Items.Count > 0)
            {
                cmbConstraint.SelectedIndex = 0;
            }
        }

        private void SelectionChanged(
            object? sender,
            EventArgs e)
        {
            if (cmbVariable.SelectedItem == null ||
                cmbConstraint.SelectedIndex < 0)
            {
                return;
            }

            string variableName =
                cmbVariable.SelectedItem.ToString()!;

            int variableIndex =
                model.Variables.FindIndex(
                    v => v.Name == variableName);

            if (variableIndex == -1)
            {
                return;
            }

            Linear_Programming_381.Models.Constraint constraint =
                model.Constraints[
                    cmbConstraint.SelectedIndex];

            double currentCoefficient =
                constraint
                    .GetSignedCoefficients()[
                        variableIndex];

            txtCurrentCoefficient.Text =
                currentCoefficient
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
                    "Please select a non-basic variable.",
                    "Invalid Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (cmbConstraint.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Please select a constraint.",
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

            SelectedConstraintIndex =
                cmbConstraint.SelectedIndex;

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
