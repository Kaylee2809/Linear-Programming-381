namespace Linear_Programming_381.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            lblInputFile = new Label();
            txtInputPath = new TextBox();
            btnBrowse = new Button();
            btnLoad = new Button();
            grpAlgorithms = new GroupBox();
            btnRHSChange = new Button();
            btnObjectiveChange = new Button();
            btnDuality = new Button();
            button1 = new Button();
            btnAddActivity = new Button();
            btnSensitivity = new Button();
            btnBranchBoundKnapsack = new Button();
            btnBranchBoundSimplex = new Button();
            btnClear = new Button();
            btnCuttingPlane = new Button();
            btnExport = new Button();
            btnRevisedSimplex = new Button();
            btnSolve = new Button();
            lblModel = new Label();
            txtModel = new TextBox();
            lblOutput = new Label();
            txtOutput = new TextBox();
            lblStatusCaption = new Label();
            lblStatus = new Label();
            btnColumnChange = new Button();
            grpAlgorithms.SuspendLayout();
            SuspendLayout();
            // 
            // lblInputFile
            // 
            lblInputFile.AutoSize = true;
            lblInputFile.Location = new Point(18, 18);
            lblInputFile.Name = "lblInputFile";
            lblInputFile.Size = new Size(57, 15);
            lblInputFile.TabIndex = 0;
            lblInputFile.Text = "Input file:";
            // 
            // txtInputPath
            // 
            txtInputPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtInputPath.Location = new Point(84, 14);
            txtInputPath.Name = "txtInputPath";
            txtInputPath.Size = new Size(833, 23);
            txtInputPath.TabIndex = 1;
            // 
            // btnBrowse
            // 
            btnBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBrowse.Location = new Point(923, 13);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(86, 25);
            btnBrowse.TabIndex = 2;
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // btnLoad
            // 
            btnLoad.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLoad.Location = new Point(1015, 13);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(86, 25);
            btnLoad.TabIndex = 3;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // grpAlgorithms
            // 
            grpAlgorithms.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpAlgorithms.Controls.Add(btnColumnChange);
            grpAlgorithms.Controls.Add(btnRHSChange);
            grpAlgorithms.Controls.Add(btnObjectiveChange);
            grpAlgorithms.Controls.Add(btnDuality);
            grpAlgorithms.Controls.Add(button1);
            grpAlgorithms.Controls.Add(btnAddActivity);
            grpAlgorithms.Controls.Add(btnSensitivity);
            grpAlgorithms.Controls.Add(btnBranchBoundKnapsack);
            grpAlgorithms.Controls.Add(btnBranchBoundSimplex);
            grpAlgorithms.Controls.Add(btnClear);
            grpAlgorithms.Controls.Add(btnCuttingPlane);
            grpAlgorithms.Controls.Add(btnExport);
            grpAlgorithms.Controls.Add(btnRevisedSimplex);
            grpAlgorithms.Controls.Add(btnSolve);
            grpAlgorithms.Location = new Point(18, 52);
            grpAlgorithms.Name = "grpAlgorithms";
            grpAlgorithms.Size = new Size(1083, 102);
            grpAlgorithms.TabIndex = 4;
            grpAlgorithms.TabStop = false;
            grpAlgorithms.Text = "Algorithms";
            // 
            // btnRHSChange
            // 
            btnRHSChange.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRHSChange.Location = new Point(719, 59);
            btnRHSChange.Name = "btnRHSChange";
            btnRHSChange.Size = new Size(137, 29);
            btnRHSChange.TabIndex = 16;
            btnRHSChange.Text = "Apply RHS Change";
            btnRHSChange.UseVisualStyleBackColor = true;
            btnRHSChange.Click += btnRHSChange_Click;
            // 
            // btnObjectiveChange
            // 
            btnObjectiveChange.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnObjectiveChange.Location = new Point(542, 59);
            btnObjectiveChange.Name = "btnObjectiveChange";
            btnObjectiveChange.Size = new Size(171, 30);
            btnObjectiveChange.TabIndex = 15;
            btnObjectiveChange.Text = "Apply Objective Change";
            btnObjectiveChange.UseVisualStyleBackColor = true;
            btnObjectiveChange.Click += btnObjectiveChange_Click;
            // 
            // btnDuality
            // 
            btnDuality.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDuality.Location = new Point(461, 58);
            btnDuality.Name = "btnDuality";
            btnDuality.Size = new Size(75, 28);
            btnDuality.TabIndex = 14;
            btnDuality.Text = "Duality";
            btnDuality.UseVisualStyleBackColor = true;
            btnDuality.Click += btnDuality_Click;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.Location = new Point(314, 58);
            button1.Name = "button1";
            button1.Size = new Size(141, 30);
            button1.TabIndex = 13;
            button1.Text = "Add New Constraint";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // btnAddActivity
            // 
            btnAddActivity.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddActivity.Location = new Point(177, 56);
            btnAddActivity.Name = "btnAddActivity";
            btnAddActivity.Size = new Size(131, 30);
            btnAddActivity.TabIndex = 12;
            btnAddActivity.Text = "Add New Activity";
            btnAddActivity.UseVisualStyleBackColor = true;
            btnAddActivity.Click += btnAddActivity_Click;
            // 
            // btnSensitivity
            // 
            btnSensitivity.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSensitivity.Location = new Point(10, 56);
            btnSensitivity.Name = "btnSensitivity";
            btnSensitivity.Size = new Size(164, 30);
            btnSensitivity.TabIndex = 11;
            btnSensitivity.Text = "Sensitivity Analysis";
            btnSensitivity.UseVisualStyleBackColor = true;
            btnSensitivity.Click += btnSensitivity_Click;
            // 
            // btnBranchBoundKnapsack
            // 
            btnBranchBoundKnapsack.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBranchBoundKnapsack.Location = new Point(696, 22);
            btnBranchBoundKnapsack.Name = "btnBranchBoundKnapsack";
            btnBranchBoundKnapsack.Size = new Size(116, 30);
            btnBranchBoundKnapsack.TabIndex = 4;
            btnBranchBoundKnapsack.Text = "Knapsack";
            btnBranchBoundKnapsack.UseVisualStyleBackColor = true;
            btnBranchBoundKnapsack.Click += btnBranchBoundKnapsack_Click;
            // 
            // btnBranchBoundSimplex
            // 
            btnBranchBoundSimplex.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBranchBoundSimplex.Location = new Point(500, 22);
            btnBranchBoundSimplex.Name = "btnBranchBoundSimplex";
            btnBranchBoundSimplex.Size = new Size(190, 30);
            btnBranchBoundSimplex.TabIndex = 3;
            btnBranchBoundSimplex.Text = "Branch && Bound Simplex";
            btnBranchBoundSimplex.UseVisualStyleBackColor = true;
            btnBranchBoundSimplex.Click += btnBranchBoundSimplex_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClear.Location = new Point(946, 22);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(86, 30);
            btnClear.TabIndex = 10;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // btnCuttingPlane
            // 
            btnCuttingPlane.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCuttingPlane.Location = new Point(347, 22);
            btnCuttingPlane.Name = "btnCuttingPlane";
            btnCuttingPlane.Size = new Size(147, 30);
            btnCuttingPlane.TabIndex = 2;
            btnCuttingPlane.Text = "Cutting Plane";
            btnCuttingPlane.UseVisualStyleBackColor = true;
            btnCuttingPlane.Click += btnCuttingPlane_Click;
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExport.Location = new Point(844, 22);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(86, 30);
            btnExport.TabIndex = 9;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += btnExport_Click;
            // 
            // btnRevisedSimplex
            // 
            btnRevisedSimplex.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRevisedSimplex.Location = new Point(177, 22);
            btnRevisedSimplex.Name = "btnRevisedSimplex";
            btnRevisedSimplex.Size = new Size(164, 30);
            btnRevisedSimplex.TabIndex = 1;
            btnRevisedSimplex.Text = "Revised Simplex";
            btnRevisedSimplex.UseVisualStyleBackColor = true;
            btnRevisedSimplex.Click += btnRevisedSimplex_Click;
            // 
            // btnSolve
            // 
            btnSolve.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSolve.Location = new Point(10, 22);
            btnSolve.Name = "btnSolve";
            btnSolve.Size = new Size(161, 30);
            btnSolve.TabIndex = 0;
            btnSolve.Text = "Primal Simplex";
            btnSolve.UseVisualStyleBackColor = true;
            btnSolve.Click += btnSolve_Click;
            // 
            // lblModel
            // 
            lblModel.AutoSize = true;
            lblModel.Location = new Point(18, 141);
            lblModel.Name = "lblModel";
            lblModel.Size = new Size(44, 15);
            lblModel.TabIndex = 5;
            lblModel.Text = "Model:";
            // 
            // txtModel
            // 
            txtModel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            txtModel.Font = new Font("Consolas", 9F);
            txtModel.Location = new Point(18, 160);
            txtModel.Multiline = true;
            txtModel.Name = "txtModel";
            txtModel.ReadOnly = true;
            txtModel.ScrollBars = ScrollBars.Both;
            txtModel.Size = new Size(359, 491);
            txtModel.TabIndex = 6;
            txtModel.WordWrap = false;
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Location = new Point(394, 141);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(48, 15);
            lblOutput.TabIndex = 7;
            lblOutput.Text = "Output:";
            // 
            // txtOutput
            // 
            txtOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtOutput.Font = new Font("Consolas", 9F);
            txtOutput.Location = new Point(394, 160);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = ScrollBars.Both;
            txtOutput.Size = new Size(707, 491);
            txtOutput.TabIndex = 8;
            txtOutput.WordWrap = false;
            // 
            // lblStatusCaption
            // 
            lblStatusCaption.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatusCaption.AutoSize = true;
            lblStatusCaption.Location = new Point(18, 673);
            lblStatusCaption.Name = "lblStatusCaption";
            lblStatusCaption.Size = new Size(42, 15);
            lblStatusCaption.TabIndex = 11;
            lblStatusCaption.Text = "Status:";
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(66, 673);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(42, 15);
            lblStatus.TabIndex = 12;
            lblStatus.Text = "Ready.";
            // 
            // btnColumnChange
            // 
            btnColumnChange.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnColumnChange.Location = new Point(862, 60);
            btnColumnChange.Name = "btnColumnChange";
            btnColumnChange.Size = new Size(170, 30);
            btnColumnChange.TabIndex = 17;
            btnColumnChange.Text = "Apply Column Change";
            btnColumnChange.UseVisualStyleBackColor = true;
            btnColumnChange.Click += btnColumnChange_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1120, 712);
            Controls.Add(lblStatus);
            Controls.Add(lblStatusCaption);
            Controls.Add(txtOutput);
            Controls.Add(lblOutput);
            Controls.Add(txtModel);
            Controls.Add(lblModel);
            Controls.Add(grpAlgorithms);
            Controls.Add(btnLoad);
            Controls.Add(btnBrowse);
            Controls.Add(txtInputPath);
            Controls.Add(lblInputFile);
            MinimumSize = new Size(1028, 751);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Linear Programming 381";
            grpAlgorithms.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }



        private Label lblInputFile;
        private TextBox txtInputPath;
        private Button btnBrowse;
        private Button btnLoad;
        private GroupBox grpAlgorithms;
        private Button btnSolve;
        private Button btnRevisedSimplex;
        private Button btnCuttingPlane;
        private Button btnBranchBoundSimplex;
        private Button btnBranchBoundKnapsack;
        private Label lblModel;
        private TextBox txtModel;
        private Label lblOutput;
        private TextBox txtOutput;
        private Button btnExport;
        private Button btnClear;
        private Label lblStatusCaption;
        private Label lblStatus;
        private Button btnSensitivity;
        private Button btnAddActivity;
        private Button button1;
        private Button btnDuality;
        private Button btnObjectiveChange;
        private Button btnRHSChange;
        private Button btnColumnChange;
    }
}
