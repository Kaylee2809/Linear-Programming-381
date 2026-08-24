using System;
using System.Drawing;
using System.Windows.Forms;

using Linear_Programming_381.Algorithms.Primal_Simplex;
using Linear_Programming_381.Algorithms.Revised_Simplex;
using Linear_Programming_381.Algorithms.Cutting_Plane;
using Linear_Programming_381.Core;
using Linear_Programming_381.File_Handling;
using Linear_Programming_381.Models;

namespace Linear_Programming_381.Forms
{
    public partial class MainForm : Form
    {
        private LPModel? currentModel;
        private Solution? currentSolution;
        private Tableau? currentTableau;
        private readonly FileReader fileReader;
        private readonly FileWriter fileWriter;
        public MainForm()
        {
            InitializeComponent();
            fileReader = new FileReader();
            fileWriter = new FileWriter();
        }
        private void btnBrowse_Click(
            object sender,
            EventArgs e)
        {
            using OpenFileDialog dialog =
                new OpenFileDialog();
            dialog.Title =
                "Select LP Input File";
            dialog.Filter =
                "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (dialog.ShowDialog() ==
                DialogResult.OK)
            {
                txtInputPath.Text =
                    dialog.FileName;
            }
        }
        private void btnLoad_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                        txtInputPath.Text))
                {
                    MessageBox.Show(
                        "Please select an input file.",
                        "Input Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                currentModel =
                    fileReader.ReadModel(
                        txtInputPath.Text);
                currentSolution = null;
                currentTableau = null;
                DisplayModel();
                lblStatus.Text =
                    "Model loaded successfully.";
                lblStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Input Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblStatus.Text =
                    "Failed to load model.";
                lblStatus.ForeColor =
                    Color.DarkRed;
            }
        }
        private void btnSolve_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (currentModel == null)
                {
                    MessageBox.Show(
                        "Please load a programming model first.",
                        "No Model",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                CanonicalConverter converter =
                    new CanonicalConverter();
                currentTableau =
                    converter.Convert(
                        currentModel);
                txtOutput.Clear();
                txtOutput.AppendText(
                    "CANONICAL FORM / INITIAL TABLEAU\r\n");
                txtOutput.AppendText(
                    "========================================\r\n\r\n");
                txtOutput.AppendText(
                    currentTableau.ToFormattedString());
                txtOutput.AppendText("\r\n");
                PrimalSimplexSolver solver =
                    new PrimalSimplexSolver();
                currentSolution =
                    solver.Solve(
                        currentModel,
                        currentTableau);
                foreach (string iteration
                         in currentSolution.Iterations)
                {
                    txtOutput.AppendText(
                        iteration);
                    txtOutput.AppendText(
                        "\r\n");
                }
                DisplaySolution();
                lblStatus.Text =
                    "Primal Simplex completed.";
                lblStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Solver Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblStatus.Text =
                    "Unable to solve model.";
                lblStatus.ForeColor =
                    Color.DarkRed;
            }
        }

        private void btnRevisedSimplex_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (currentModel == null)
                {
                    MessageBox.Show(
                        "Please load a programming model first.",
                        "No Model",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                currentTableau = null;
                txtOutput.Clear();
                txtOutput.AppendText(
                    "REVISED PRIMAL SIMPLEX (PRODUCT FORM / PRICE-OUT)\r\n");
                txtOutput.AppendText(
                    "========================================\r\n\r\n");
                RevisedSimplexSolver solver =
                    new RevisedSimplexSolver();
                currentSolution =
                    solver.Solve(
                        currentModel);
                foreach (string iteration
                         in currentSolution.Iterations)
                {
                    txtOutput.AppendText(
                        iteration);
                    txtOutput.AppendText(
                        "\r\n");
                }
                DisplaySolution();
                lblStatus.Text =
                    "Revised Simplex completed.";
                lblStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Solver Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblStatus.Text =
                    "Unable to solve model.";
                lblStatus.ForeColor =
                    Color.DarkRed;
            }
        }
        private void btnCuttingPlane_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (currentModel == null)
                {
                    MessageBox.Show(
                        "Please load a programming model first.",
                        "No Model",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                currentTableau = null;
                txtOutput.Clear();
                txtOutput.AppendText(
                    "CUTTING PLANE ALGORITHM (GOMORY CUTS)\r\n");
                txtOutput.AppendText(
                    "========================================\r\n\r\n");
                CuttingPlaneSolver solver =
                    new CuttingPlaneSolver();
                currentSolution =
                    solver.Solve(
                        currentModel);
                foreach (string iteration
                         in currentSolution.Iterations)
                {
                    txtOutput.AppendText(
                        iteration);
                    txtOutput.AppendText(
                        "\r\n");
                }
                DisplaySolution();
                lblStatus.Text =
                    "Cutting Plane completed.";
                lblStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Solver Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                lblStatus.Text =
                    "Unable to solve model.";
                lblStatus.ForeColor =
                    Color.DarkRed;
            }
        }

        private void btnExport_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (currentModel == null ||
                    currentSolution == null)
                {
                    MessageBox.Show(
                        "Please load and solve a model first.",
                        "Nothing to Export",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                using SaveFileDialog dialog =
                    new SaveFileDialog();
                dialog.Title =
                    "Export LP Results";
                dialog.Filter =
                    "Text Files (*.txt)|*.txt";
                dialog.FileName =
                    "output.txt";
                if (dialog.ShowDialog() ==
                    DialogResult.OK)
                {
                    fileWriter.Write(
                        dialog.FileName,
                        currentModel,
                        currentSolution);
                    MessageBox.Show(
                        "Results exported successfully.",
                        "Export Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void btnClear_Click(
            object sender,
            EventArgs e)
        {
            txtOutput.Clear();

            txtModel.Clear();

            lblStatus.Text =
                "Ready.";
            lblStatus.ForeColor =
                Color.Black;
        }
        private void DisplayModel()
        {
            if (currentModel == null)
            {
                return;
            }
            txtModel.Clear();
            txtModel.AppendText(
                "PROGRAMMING MODEL\r\n");
            txtModel.AppendText(
                "============================\r\n\r\n");
            txtModel.AppendText(
                currentModel.GetObjectiveString());
            txtModel.AppendText(
                "\r\n\r\n");
            foreach (Constraint constraint
                     in currentModel.Constraints)
            {
                txtModel.AppendText(
                    currentModel.GetConstraintString(
                        constraint));
                txtModel.AppendText(
                    "\r\n");
            }
            txtModel.AppendText(
                "\r\nVariable Restrictions:\r\n");
            txtModel.AppendText(
                currentModel.GetRestrictionsString());
        }
        private void DisplaySolution()
        {
            if (currentSolution == null)
            {
                return;
            }
            txtOutput.AppendText(
                "\r\n========================================\r\n");
            txtOutput.AppendText(
                "FINAL SOLUTION\r\n");
            txtOutput.AppendText(
                "========================================\r\n\r\n");
            if (currentSolution.IsUnbounded)
            {
                txtOutput.AppendText(
                    "The model is UNBOUNDED.\r\n");
                return;
            }
            if (currentSolution.IsInfeasible)
            {
                txtOutput.AppendText(
                    "The model is INFEASIBLE.\r\n");
                return;
            }
            txtOutput.AppendText(
                $"Optimal Objective Value = " +
                $"{currentSolution.ObjectiveValue:0.###}\r\n\r\n");

            foreach (var variable
                     in currentSolution.VariableValues)
            {
                txtOutput.AppendText(
                    $"{variable.Key} = " +
                    $"{variable.Value:0.###}\r\n");
            }
        }
    }
}
