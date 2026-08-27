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
    public partial class RhsChangeForm : Form
    {
        private readonly LPModel model;

        public int SelectedConstraintIndex
        {
            get;
            private set;
        }

        public double NewRhs
        {
            get;
            private set;
        }

        private ComboBox cmbConstraint = null!;
        private TextBox txtCurrentRhs = null!;
        private TextBox txtNewRhs = null!;

        private Button btnApply = null!;
        private Button btnCancel = null!;

        public RhsChangeForm(
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
                "Change Constraint RHS";

            StartPosition =
                FormStartPosition.CenterParent;

            ClientSize =
                new Size(
                    480,
                    300);

            Controls.Clear();

            Label lblTitle =
                new Label
                {
                    Text =
                        "Apply Constraint RHS Change",

                    Font =
                        new Font(
                            Font,
                            FontStyle.Bold),

                    AutoSize = true,
                    Left = 20,
                    Top = 20
                };

            Label lblConstraint =
                new Label
                {
                    Text =
                        "Constraint:",

                    AutoSize = true,
                    Left = 20,
                    Top = 75
                };

            cmbConstraint =
                new ComboBox
                {
                    Left = 170,
                    Top = 70,
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
                cmbConstraint_SelectedIndexChanged;

            Label lblCurrent =
                new Label
                {
                    Text =
                        "Current RHS:",

                    AutoSize = true,
                    Left = 20,
                    Top = 120
                };

            txtCurrentRhs =
                new TextBox
                {
                    Left = 170,
                    Top = 115,
                    Width = 250,
                    ReadOnly = true
                };

            Label lblNew =
                new Label
                {
                    Text =
                        "New RHS:",

                    AutoSize = true,
                    Left = 20,
                    Top = 165
                };

            txtNewRhs =
                new TextBox
                {
                    Left = 170,
                    Top = 160,
                    Width = 250
                };

            btnApply =
                new Button
                {
                    Text =
                        "Apply Change",

                    Left = 200,
                    Top = 220,
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

                    Left = 330,
                    Top = 220,
                    Width = 90,
                    Height = 35
                };

            btnCancel.Click +=
                btnCancel_Click;

            Controls.Add(
                lblTitle);

            Controls.Add(
                lblConstraint);

            Controls.Add(
                cmbConstraint);

            Controls.Add(
                lblCurrent);

            Controls.Add(
                txtCurrentRhs);

            Controls.Add(
                lblNew);

            Controls.Add(
                txtNewRhs);

            Controls.Add(
                btnApply);

            Controls.Add(
                btnCancel);

            AcceptButton =
                btnApply;

            CancelButton =
                btnCancel;

            if (cmbConstraint.Items.Count > 0)
            {
                cmbConstraint.SelectedIndex = 0;
            }
        }

        private void cmbConstraint_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (cmbConstraint.SelectedIndex < 0)
            {
                return;
            }

            Linear_Programming_381.Models.Constraint constraint =
                model.Constraints[
                    cmbConstraint.SelectedIndex];

            txtCurrentRhs.Text =
                constraint.RightHandSide
                    .ToString("0.###");

            txtNewRhs.Text =
                txtCurrentRhs.Text;
        }

        private void btnApply_Click(
            object? sender,
            EventArgs e)
        {
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
                    txtNewRhs.Text,
                    out double newRhs))
            {
                MessageBox.Show(
                    "Please enter a valid RHS value.",
                    "Invalid RHS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtNewRhs.Focus();

                return;
            }

            SelectedConstraintIndex =
                cmbConstraint.SelectedIndex;

            NewRhs =
                newRhs;

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
