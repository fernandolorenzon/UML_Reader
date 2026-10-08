namespace WinFormsApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblUml;
        private TextBox txtUmlFile;
        private Button btnOpenUml;

        private Label lblOcl;
        private TextBox txtOclFile;
        private Button btnOpenOcl;

        private Label lblExpression;
        private TextBox txtOclExpression;

        private Button btnExecute;

        private Label lblResult;
        private DataGridView dgvResult;

        private Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblUml = new Label();
            txtUmlFile = new TextBox();
            btnOpenUml = new Button();

            lblOcl = new Label();
            txtOclFile = new TextBox();
            btnOpenOcl = new Button();

            lblExpression = new Label();
            txtOclExpression = new TextBox();

            btnExecute = new Button();

            lblResult = new Label();
            dgvResult = new DataGridView();

            lblStatus = new Label();

            ((System.ComponentModel.ISupportInitialize)dgvResult).BeginInit();

            SuspendLayout();

            // lblUml
            lblUml.AutoSize = true;
            lblUml.Location = new Point(20, 20);
            lblUml.Name = "lblUml";
            lblUml.Size = new Size(74, 15);
            lblUml.Text = "Modelo UML:";

            // txtUmlFile
            txtUmlFile.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            txtUmlFile.Location = new Point(20, 40);
            txtUmlFile.Name = "txtUmlFile";
            txtUmlFile.ReadOnly = true;
            txtUmlFile.Size = new Size(650, 23);

            // btnOpenUml
            btnOpenUml.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            btnOpenUml.Location = new Point(680, 39);
            btnOpenUml.Name = "btnOpenUml";
            btnOpenUml.Size = new Size(100, 25);
            btnOpenUml.Text = "Abrir...";
            btnOpenUml.UseVisualStyleBackColor = true;
            btnOpenUml.Click += btnOpenUml_Click;

            // lblOcl
            lblOcl.AutoSize = true;
            lblOcl.Location = new Point(20, 80);
            lblOcl.Name = "lblOcl";
            lblOcl.Size = new Size(85, 15);
            lblOcl.Text = "Arquivo OCL:";

            // txtOclFile
            txtOclFile.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            txtOclFile.Location = new Point(20, 100);
            txtOclFile.Name = "txtOclFile";
            txtOclFile.ReadOnly = true;
            txtOclFile.Size = new Size(650, 23);

            // btnOpenOcl
            btnOpenOcl.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            btnOpenOcl.Location = new Point(680, 99);
            btnOpenOcl.Name = "btnOpenOcl";
            btnOpenOcl.Size = new Size(100, 25);
            btnOpenOcl.Text = "Abrir...";
            btnOpenOcl.UseVisualStyleBackColor = true;
            btnOpenOcl.Click += btnOpenOcl_Click;

            // lblExpression
            lblExpression.AutoSize = true;
            lblExpression.Location = new Point(20, 140);
            lblExpression.Name = "lblExpression";
            lblExpression.Size = new Size(91, 15);
            lblExpression.Text = "Expressão OCL:";

            // txtOclExpression
            txtOclExpression.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            txtOclExpression.Location = new Point(20, 160);
            txtOclExpression.Multiline = true;
            txtOclExpression.ScrollBars = ScrollBars.Vertical;
            txtOclExpression.Font =
                new Font("Consolas", 10F, FontStyle.Regular);

            txtOclExpression.Size = new Size(760, 100);

            // btnExecute
            btnExecute.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            btnExecute.Location = new Point(650, 275);
            btnExecute.Name = "btnExecute";
            btnExecute.Size = new Size(130, 35);
            btnExecute.Text = "Executar OCL";
            btnExecute.UseVisualStyleBackColor = true;
            btnExecute.Click += btnExecute_Click;

            // lblResult
            lblResult.AutoSize = true;
            lblResult.Location = new Point(20, 325);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(62, 15);
            lblResult.Text = "Resultado:";

            // dgvResult
            dgvResult.AllowUserToAddRows = false;
            dgvResult.AllowUserToDeleteRows = false;
            dgvResult.AllowUserToOrderColumns = true;

            dgvResult.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvResult.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvResult.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvResult.Location = new Point(20, 345);
            dgvResult.Name = "dgvResult";
            dgvResult.ReadOnly = true;
            dgvResult.RowHeadersVisible = false;
            dgvResult.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvResult.Size = new Size(760, 250);

            // lblStatus
            lblStatus.Anchor =
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            lblStatus.Location = new Point(20, 610);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(760, 20);
            lblStatus.Text = "Selecione um modelo UML e um arquivo OCL.";

            // Form1
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;

            ClientSize = new Size(800, 650);

            Controls.Add(lblUml);
            Controls.Add(txtUmlFile);
            Controls.Add(btnOpenUml);

            Controls.Add(lblOcl);
            Controls.Add(txtOclFile);
            Controls.Add(btnOpenOcl);

            Controls.Add(lblExpression);
            Controls.Add(txtOclExpression);

            Controls.Add(btnExecute);

            Controls.Add(lblResult);
            Controls.Add(dgvResult);

            Controls.Add(lblStatus);

            MinimumSize = new Size(700, 550);

            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "UML / OCL Join Point Selector";

            ((System.ComponentModel.ISupportInitialize)dgvResult).EndInit();

            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}