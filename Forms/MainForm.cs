using System;
using System.Drawing;
using System.Windows.Forms;

using Linear_Programming_381.Algorithms.Primal_Simplex;
using Linear_Programming_381.Algorithms.Revised_Simplex;
using Linear_Programming_381.Algorithms.Cutting_Plane;
using Linear_Programming_381.Algorithms.Branch_and_Bound_Simplex;
using Linear_Programming_381.Algorithms.Branch_and_Bound_Knapsack;
using Linear_Programming_381.Core;
using Linear_Programming_381.File_Handling;
using Linear_Programming_381.Models;
using Linear_Programming_381.Algorithms.Sensitivity_Analysis;
using Linear_Programming_381.Algorithms.Duality;
using System.Collections.Generic;

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


        private void btnBranchBoundSimplex_Click(
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
                    "BRANCH AND BOUND SIMPLEX\r\n");
                txtOutput.AppendText(
                    "========================================\r\n\r\n");

                BranchandBoundSimplexSolver solver =
                    new BranchandBoundSimplexSolver();

                currentSolution = solver.Solve(currentModel);

                foreach (string iteration in currentSolution.Iterations)
                {
                    txtOutput.AppendText(iteration);
                    txtOutput.AppendText("\r\n");
                }

                DisplaySolution();
                lblStatus.Text =
                    "Branch and Bound Simplex completed.";
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

        private void btnBranchBoundKnapsack_Click(
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
                    "BRANCH AND BOUND KNAPSACK\r\n");
                txtOutput.AppendText(
                    "========================================\r\n\r\n");

                BranchAndBoundKnapsackSolver solver =
                    new BranchAndBoundKnapsackSolver();

                currentSolution = solver.Solve(currentModel);

                foreach (string iteration in currentSolution.Iterations)
                {
                    txtOutput.AppendText(iteration);
                    txtOutput.AppendText("\r\n");
                }

                DisplaySolution();
                lblStatus.Text =
                    "Branch and Bound Knapsack completed.";
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

        private void btnSensitivity_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentModel == null || currentSolution == null || currentTableau == null)
                {
                    MessageBox.Show(
                        "Please solve the model using Primal Simplex first.",
                        "Sensitivity Analysis",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                SensitivityAnalysisSolver solver = new SensitivityAnalysisSolver();

                solver.ValidateForSensitivity(currentModel, currentSolution, currentTableau);

                List<string> basic = solver.GetBasicDecisionVariables(currentModel, currentTableau);

                List<string> nonBasic = solver.GetNonBasicDecisionVariables(currentModel, currentTableau);

                txtOutput.AppendText(
                    "\r\n\r\n" +
                    "SENSITIVITY ANALYSIS\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText("\r\nBasic Decision Variables:\r\n");

                if (basic.Count == 0)
                {
                    txtOutput.AppendText("None\r\n");
                }
                else
                {
                    foreach (string variable in basic)
                    {
                        SensitivityRange range = solver.GetBasicVariableRange(currentModel, currentTableau, variable);

                        txtOutput.AppendText(range.GetFormattedRange());

                        txtOutput.AppendText("\r\n");
                    }
                }

                txtOutput.AppendText("\r\nNon-Basic Decision Variables:\r\n" + "========================================\r\n");

                if (nonBasic.Count == 0)
                {
                    txtOutput.AppendText("None\r\n");
                }
                else
                {
                    foreach (string variable in nonBasic)
                    {

                        SensitivityRange range = solver.GetNonBasicVariableRange(currentModel, currentTableau, variable);

                        txtOutput.AppendText(range.GetFormattedRange());

                        txtOutput.AppendText("\r\n");
                    }
                }

                txtOutput.AppendText("\r\nConstraint RHS Ranges:\r\n" +
                    "========================================\r\n");

                for (int i = 0; i < currentModel.Constraints.Count; i++)
                {
                    SensitivityRange range =
                        solver.GetConstraintRhsRange(
                            currentModel,
                            currentTableau,
                            i);

                    txtOutput.AppendText(
                        range.GetFormattedRange());

                    txtOutput.AppendText(
                        "\r\n");
                }

                txtOutput.AppendText("\r\nSHADOW PRICES\r\n" + "========================================\r\n");

                Dictionary<string, double> shadowPrices = solver.GetShadowPrices(currentModel, currentTableau);

                foreach (KeyValuePair<string, double> item in shadowPrices)
                {
                    txtOutput.AppendText($"{item.Key}: {item.Value:0.###}\r\n");
                }

                txtOutput.AppendText("\r\nNON-BASIC COLUMN COEFFICIENT RANGES\r\n" + "========================================\r\n");

                foreach (string variable in nonBasic)
                {
                    for (int i = 0; i < currentModel.Constraints.Count; i++)
                    {
                        TechnologicalCoefficientRange range = solver.GetNonBasicColumnCoefficientRange(currentModel,
                                currentTableau, variable, i);

                        txtOutput.AppendText(range.GetFormattedRange());

                        txtOutput.AppendText("\r\n");
                    }
                }

                lblStatus.Text = "Sensitivity Analysis completed.";

                lblStatus.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sensitivity Analysis Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddActivity_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentModel == null || currentSolution == null || currentTableau == null)
                {
                    MessageBox.Show("Please solve the model using Primal Simplex first.",
                        "Add New Activity",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                SensitivityAnalysisSolver solver = new SensitivityAnalysisSolver();

                solver.ValidateForSensitivity(currentModel, currentSolution, currentTableau);

                using AddActivityForm form = new AddActivityForm(currentModel);

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var result =
                    solver.AddNewActivityAndResolve(currentModel, currentTableau, form.VariableName,
                        form.ObjectiveCoefficient, form.ConstraintCoefficients);

                currentTableau = result.Tableau;

                currentSolution = result.Solution;

                txtOutput.AppendText("\r\n\r\nADD NEW ACTIVITY\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(result.Analysis.GetFormattedResult());

                txtOutput.AppendText("\r\nNEW OPTIMAL SOLUTION\r\n" +
                    "========================================\r\n");

                if (currentSolution.IsInfeasible)
                {
                    txtOutput.AppendText("The expanded model is infeasible.\r\n");

                    lblStatus.Text = "Expanded model is infeasible.";

                    lblStatus.ForeColor =
                        Color.DarkRed;

                    return;
                }

                if (currentSolution.IsUnbounded)
                {
                    txtOutput.AppendText("The expanded model is unbounded.\r\n");

                    lblStatus.Text = "Expanded model is unbounded.";

                    lblStatus.ForeColor = Color.DarkRed;

                    return;
                }

                foreach (KeyValuePair<string, double> item in currentSolution.VariableValues)
                {
                    txtOutput.AppendText($"{item.Key} = " + $"{item.Value:0.###}\r\n");
                }

                txtOutput.AppendText($"Objective Value = {currentSolution.ObjectiveValue:0.###}\r\n");

                lblStatus.Text = "New activity added and model re-solved.";

                lblStatus.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add New Activity Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentModel == null || currentSolution == null || currentTableau == null)
                {
                    MessageBox.Show("Please solve the model using Primal Simplex first.", "Add New Constraint",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                SensitivityAnalysisSolver solver = new SensitivityAnalysisSolver();

                solver.ValidateForSensitivity(currentModel, currentSolution, currentTableau);

                using AddConstraintForm form = new AddConstraintForm(currentModel);

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                var result = solver.AddNewConstraintAndResolve(currentModel, currentSolution, form.Coefficients,
                        form.Relation, form.RightHandSide);

                currentTableau = result.Tableau;

                currentSolution = result.Solution;

                txtOutput.AppendText(
                    "\r\n\r\n" +
                    "ADD NEW CONSTRAINT\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(result.Analysis.GetFormattedResult());

                txtOutput.AppendText("\r\nNEW OPTIMAL SOLUTION\r\n" + "========================================\r\n");

                if (currentSolution.IsInfeasible)
                {
                    txtOutput.AppendText("The expanded model is infeasible.\r\n");

                    lblStatus.Text = "Expanded model is infeasible.";

                    lblStatus.ForeColor = Color.DarkRed;

                    return;
                }

                if (currentSolution.IsUnbounded)
                {
                    txtOutput.AppendText("The expanded model is unbounded.\r\n");

                    lblStatus.Text = "Expanded model is unbounded.";

                    lblStatus.ForeColor = Color.DarkRed;

                    return;
                }

                foreach (KeyValuePair<string, double> item in currentSolution.VariableValues)
                {
                    txtOutput.AppendText(
                        $"{item.Key} = " +
                        $"{item.Value:0.###}\r\n");
                }

                txtOutput.AppendText($"Objective Value = {currentSolution.ObjectiveValue:0.###}\r\n");

                lblStatus.Text = "New constraint added and model re-solved.";

                lblStatus.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add New Constraint Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDuality_Click(object sender, EventArgs e)
        {
            try
            {


                if (currentModel == null ||
                    currentSolution == null)
                {
                    MessageBox.Show(
                        "Please solve the primal model first.",
                        "Duality",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }


                DualitySolver solver =
                    new DualitySolver();

                DualityResult result =
                    solver.SolveAndVerify(
                        currentModel,
                        currentSolution);


                txtOutput.AppendText(
                    "\r\n\r\n" +
                    "DUAL PROGRAMMING MODEL\r\n" +
                    "========================================\r\n\r\n");

                txtOutput.AppendText(
                    result.DualModel.GetObjectiveString());

                txtOutput.AppendText(
                    "\r\n\r\n");

                foreach (Constraint constraint
                         in result.DualModel.Constraints)
                {
                    txtOutput.AppendText(
                        result.DualModel
                            .GetConstraintString(
                                constraint));

                    txtOutput.AppendText(
                        "\r\n");
                }

                txtOutput.AppendText(
                    "\r\nVariable Restrictions:\r\n");

                txtOutput.AppendText(
                    result.DualModel
                        .GetRestrictionsString());

                txtOutput.AppendText(
                    "\r\n");


                txtOutput.AppendText(
                    "\r\n\r\n" +
                    "DUAL SOLUTION\r\n" +
                    "========================================\r\n");

                foreach (KeyValuePair<string, double> item
                         in result.DualSolution.VariableValues)
                {
                    txtOutput.AppendText(
                        $"{item.Key} = " +
                        $"{item.Value:0.###}\r\n");
                }



                txtOutput.AppendText(
                    "\r\nDUALITY VERIFICATION\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(
                    $"Primal Objective Value = " +
                    $"{result.PrimalObjectiveValue:0.###}\r\n");

                txtOutput.AppendText(
                    $"Dual Objective Value = " +
                    $"{result.DualObjectiveValue:0.###}\r\n");



                txtOutput.AppendText(
                    $"Weak Duality Satisfied = " +
                    $"{(result.SatisfiesWeakDuality
                        ? "Yes"
                        : "No")}\r\n");



                txtOutput.AppendText(
                    $"Strong Duality = " +
                    $"{(result.HasStrongDuality
                        ? "Yes"
                        : "No")}\r\n");


                txtOutput.AppendText(
                    "\r\n");

                if (result.HasStrongDuality)
                {
                    txtOutput.AppendText(
                        "Strong Duality holds because the " +
                        "optimal primal objective value and " +
                        "optimal dual objective value are equal.\r\n");
                }
                else if (result.SatisfiesWeakDuality)
                {
                    txtOutput.AppendText(
                        "Weak Duality holds because the primal " +
                        "objective value does not exceed the " +
                        "dual objective value.\r\n");
                }
                else
                {
                    txtOutput.AppendText(
                        "The expected duality relationship " +
                        "was not satisfied.\r\n");
                }


                lblStatus.Text =
                    "Dual model solved and duality verified.";

                lblStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Duality Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text =
                    "Duality operation failed.";

                lblStatus.ForeColor =
                    Color.DarkRed;
            }
        }

        private void btnObjectiveChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentModel == null ||
                    currentSolution == null ||
                    currentTableau == null)
                {
                    MessageBox.Show(
                        "Please solve the model using Primal Simplex first.",
                        "Objective Coefficient Change",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                SensitivityAnalysisSolver sensitivitySolver =
                    new SensitivityAnalysisSolver();

                sensitivitySolver.ValidateForSensitivity(
                    currentModel,
                    currentSolution,
                    currentTableau);

                using ObjectiveCoefficientChangeForm form =
                    new ObjectiveCoefficientChangeForm(
                        currentModel);

                if (form.ShowDialog(this) !=
                    DialogResult.OK)
                {
                    return;
                }

                string variableName =
                    form.SelectedVariable;

                double newCoefficient =
                    form.NewCoefficient;

                bool isBasic =
                    currentTableau.BasicVariables.Contains(
                        variableName);

                SensitivityRange range;

                if (isBasic)
                {
                    range =
                        sensitivitySolver.GetBasicVariableRange(
                            currentModel,
                            currentTableau,
                            variableName);
                }
                else
                {
                    range =
                        sensitivitySolver.GetNonBasicVariableRange(
                            currentModel,
                            currentTableau,
                            variableName);
                }

                double oldCoefficient =
                    range.CurrentValue;


                txtOutput.AppendText(
                    "\r\n\r\n" +
                    "OBJECTIVE COEFFICIENT CHANGE\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(
                    $"Variable: {variableName}\r\n");

                txtOutput.AppendText(
                    $"Variable Type: " +
                    $"{(isBasic ? "Basic" : "Non-Basic")}\r\n");

                txtOutput.AppendText(
                    $"Old Coefficient: " +
                    $"{oldCoefficient:0.###}\r\n");

                txtOutput.AppendText(
                    $"Requested New Coefficient: " +
                    $"{newCoefficient:0.###}\r\n\r\n");

                txtOutput.AppendText(
                    "ALLOWABLE RANGE BEFORE CHANGE\r\n");

                txtOutput.AppendText(
                    range.GetFormattedRange());

                txtOutput.AppendText(
                    "\r\n");


                if (isBasic)
                {
                    sensitivitySolver
                        .ApplyBasicVariableCoefficientChange(
                            currentModel,
                            currentTableau,
                            variableName,
                            newCoefficient);
                }
                else
                {
                    sensitivitySolver
                        .ApplyNonBasicVariableCoefficientChange(
                            currentModel,
                            currentTableau,
                            variableName,
                            newCoefficient);
                }


                CanonicalConverter canonicalConverter =
                    new CanonicalConverter();

                currentTableau =
                    canonicalConverter.Convert(
                        currentModel);

                PrimalSimplexSolver simplexSolver =
                    new PrimalSimplexSolver();

                currentSolution =
                    simplexSolver.Solve(
                        currentModel,
                        currentTableau);


                txtOutput.AppendText(
                    "\r\nCHANGE APPLIED SUCCESSFULLY\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(
                    $"{variableName}: " +
                    $"{oldCoefficient:0.###} -> " +
                    $"{newCoefficient:0.###}\r\n");

                txtOutput.AppendText(
                    "\r\nSOLUTION AFTER CHANGE\r\n");

                foreach (KeyValuePair<string, double> item
                         in currentSolution.VariableValues)
                {
                    txtOutput.AppendText(
                        $"{item.Key} = " +
                        $"{item.Value:0.###}\r\n");
                }

                txtOutput.AppendText(
                    $"Objective Value = " +
                    $"{currentSolution.ObjectiveValue:0.###}\r\n");

                lblStatus.Text =
                    "Objective coefficient change applied.";

                lblStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Sensitivity Change Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text =
                    "Objective coefficient change failed.";

                lblStatus.ForeColor =
                    Color.DarkRed;
            }
        }

        private void btnRHSChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentModel == null ||
                    currentSolution == null ||
                    currentTableau == null)
                {
                    MessageBox.Show(
                        "Please solve the model using Primal Simplex first.",
                        "RHS Change",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                SensitivityAnalysisSolver sensitivitySolver =
                    new SensitivityAnalysisSolver();

                sensitivitySolver.ValidateForSensitivity(
                    currentModel,
                    currentSolution,
                    currentTableau);

                using RhsChangeForm form =
                    new RhsChangeForm(
                        currentModel);

                if (form.ShowDialog(this) !=
                    DialogResult.OK)
                {
                    return;
                }

                int constraintIndex =
                    form.SelectedConstraintIndex;

                double newRhs =
                    form.NewRhs;

                SensitivityRange range =
                    sensitivitySolver.GetConstraintRhsRange(
                        currentModel,
                        currentTableau,
                        constraintIndex);

                double oldRhs =
                    range.CurrentValue;

                txtOutput.AppendText(
                    "\r\n\r\n" +
                    "CONSTRAINT RHS CHANGE\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(
                    $"Constraint: {constraintIndex + 1}\r\n");

                txtOutput.AppendText(
                    $"Relation: " +
                    $"{currentModel.Constraints[constraintIndex].Relation}\r\n");

                txtOutput.AppendText(
                    $"Old RHS: {oldRhs:0.###}\r\n");

                txtOutput.AppendText(
                    $"Requested New RHS: {newRhs:0.###}\r\n\r\n");

                txtOutput.AppendText(
                    "ALLOWABLE RANGE BEFORE CHANGE\r\n");

                txtOutput.AppendText(
                    range.GetFormattedRange());

                txtOutput.AppendText(
                    "\r\n");

                sensitivitySolver.ApplyConstraintRhsChange(
                    currentModel,
                    currentTableau,
                    constraintIndex,
                    newRhs);

                CanonicalConverter canonicalConverter =
                    new CanonicalConverter();

                currentTableau =
                    canonicalConverter.Convert(
                        currentModel);

                PrimalSimplexSolver simplexSolver =
                    new PrimalSimplexSolver();

                currentSolution =
                    simplexSolver.Solve(
                        currentModel,
                        currentTableau);

                txtOutput.AppendText(
                    "\r\nCHANGE APPLIED SUCCESSFULLY\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(
                    $"Constraint {constraintIndex + 1}: " +
                    $"{oldRhs:0.###} -> " +
                    $"{newRhs:0.###}\r\n");

                txtOutput.AppendText(
                    "\r\nSOLUTION AFTER CHANGE\r\n");

                foreach (KeyValuePair<string, double> item
                         in currentSolution.VariableValues)
                {
                    txtOutput.AppendText(
                        $"{item.Key} = " +
                        $"{item.Value:0.###}\r\n");
                }

                txtOutput.AppendText(
                    $"Objective Value = " +
                    $"{currentSolution.ObjectiveValue:0.###}\r\n");

                lblStatus.Text =
                    "RHS change applied.";

                lblStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "RHS Change Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text =
                    "RHS change failed.";

                lblStatus.ForeColor =
                    Color.DarkRed;
            }
        }

        private void btnColumnChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentModel == null ||
                    currentSolution == null ||
                    currentTableau == null)
                {
                    MessageBox.Show(
                        "Please solve the model using Primal Simplex first.",
                        "Column Coefficient Change",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                SensitivityAnalysisSolver sensitivitySolver =
                    new SensitivityAnalysisSolver();

                sensitivitySolver.ValidateForSensitivity(
                    currentModel,
                    currentSolution,
                    currentTableau);

                using NonBasicColumnChangeForm form =
                    new NonBasicColumnChangeForm(
                        currentModel,
                        currentTableau);

                if (form.ShowDialog(this) !=
                    DialogResult.OK)
                {
                    return;
                }

                string variableName =
                    form.SelectedVariable;

                int constraintIndex =
                    form.SelectedConstraintIndex;

                double newCoefficient =
                    form.NewCoefficient;

                TechnologicalCoefficientRange range =
                    sensitivitySolver
                        .GetNonBasicColumnCoefficientRange(
                            currentModel,
                            currentTableau,
                            variableName,
                            constraintIndex);

                double oldCoefficient =
                    range.CurrentValue;

                txtOutput.AppendText(
                    "\r\n\r\n" +
                    "NON-BASIC COLUMN COEFFICIENT CHANGE\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(
                    $"Variable: {variableName}\r\n");

                txtOutput.AppendText(
                    $"Constraint: {constraintIndex + 1}\r\n");

                txtOutput.AppendText(
                    $"Old Coefficient: " +
                    $"{oldCoefficient:0.###}\r\n");

                txtOutput.AppendText(
                    $"Requested New Coefficient: " +
                    $"{newCoefficient:0.###}\r\n\r\n");

                txtOutput.AppendText(
                    "ALLOWABLE RANGE BEFORE CHANGE\r\n");

                txtOutput.AppendText(
                    range.GetFormattedRange());

                txtOutput.AppendText(
                    "\r\n");

                sensitivitySolver
                    .ApplyNonBasicColumnCoefficientChange(
                        currentModel,
                        currentTableau,
                        variableName,
                        constraintIndex,
                        newCoefficient);

                CanonicalConverter canonicalConverter =
                    new CanonicalConverter();

                currentTableau =
                    canonicalConverter.Convert(
                        currentModel);

                PrimalSimplexSolver simplexSolver =
                    new PrimalSimplexSolver();

                currentSolution =
                    simplexSolver.Solve(
                        currentModel,
                        currentTableau);

                txtOutput.AppendText(
                    "\r\nCHANGE APPLIED SUCCESSFULLY\r\n" +
                    "========================================\r\n");

                txtOutput.AppendText(
                    $"Constraint {constraintIndex + 1}, " +
                    $"{variableName}: " +
                    $"{oldCoefficient:0.###} -> " +
                    $"{newCoefficient:0.###}\r\n");

                txtOutput.AppendText(
                    "\r\nSOLUTION AFTER CHANGE\r\n");

                foreach (KeyValuePair<string, double> item
                         in currentSolution.VariableValues)
                {
                    txtOutput.AppendText(
                        $"{item.Key} = " +
                        $"{item.Value:0.###}\r\n");
                }

                txtOutput.AppendText(
                    $"Objective Value = " +
                    $"{currentSolution.ObjectiveValue:0.###}\r\n");

                lblStatus.Text =
                    "Non-basic column coefficient change applied.";

                lblStatus.ForeColor =
                    Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Column Coefficient Change Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                lblStatus.Text =
                    "Column coefficient change failed.";

                lblStatus.ForeColor =
                    Color.DarkRed;
            }
        }
    }
}
