using WinFormsApp.Models;
using WinFormsApp.Services;

namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void btnOpenUml_Click(
            object sender,
            EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Selecione o modelo UML";

            dialog.Filter =
                "Arquivos UML (*.uml)|*.uml|" +
                "Arquivos XMI (*.xmi)|*.xmi|" +
                "Todos os arquivos (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtUmlFile.Text = dialog.FileName;

                lblStatus.Text =
                    "Modelo UML selecionado.";
            }
        }


        private void btnOpenOcl_Click(
            object sender,
            EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "Selecione a expressão OCL";

            dialog.Filter =
                "Arquivos OCL (*.ocl)|*.ocl|" +
                "Arquivos de texto (*.txt)|*.txt|" +
                "Todos os arquivos (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtOclFile.Text = dialog.FileName;

                try
                {
                    txtOclExpression.Text =
                        File.ReadAllText(dialog.FileName);

                    lblStatus.Text =
                        "Expressão OCL carregada.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erro ao carregar OCL:\n\n{ex.Message}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        private void btnExecute_Click(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUmlFile.Text))
            {
                MessageBox.Show(
                    "Selecione primeiro o arquivo UML.",
                    "Modelo UML",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            if (string.IsNullOrWhiteSpace(
                    txtOclExpression.Text))
            {
                MessageBox.Show(
                    "Selecione um arquivo OCL ou informe uma expressão.",
                    "Expressão OCL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }


            try
            {
                lblStatus.Text =
                    "Carregando modelo UML...";

                Cursor = Cursors.WaitCursor;

                // ================================================
                // 1. Carregar UML
                // ================================================

                UmlModelLoader loader =
                    new UmlModelLoader();

                UmlModel model =
                    loader.Load(txtUmlFile.Text);


                // ================================================
                // 2. Executar OCL
                // ================================================

                lblStatus.Text =
                    "Executando expressão OCL...";


                OclEvaluator evaluator =
                    new OclEvaluator();


                List<UmlElement> result =
                    evaluator.Evaluate(
                        model,
                        txtOclExpression.Text);


                // ================================================
                // 3. Mostrar resultado
                // ================================================

                ShowResult(result);


                lblStatus.Text =
                    $"{result.Count} elementos encontrados " +
                    $"no modelo \"{model.Name}\".";
            }
            catch (Exception ex)
            {
                dgvResult.DataSource = null;

                lblStatus.Text =
                    "Erro durante a execução.";

                MessageBox.Show(
                    ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }


        private void ShowResult(
            List<UmlElement> elements)
        {
            var data = elements
                .Select(element => new
                {
                    Tipo = element.Type,
                    Nome = element.Name,
                    XmiId = element.Id,
                    Pai = element.Parent?.Name
                })
                .ToList();


            dgvResult.DataSource = data;


            if (dgvResult.Columns["Tipo"] != null)
                dgvResult.Columns["Tipo"].FillWeight = 25;


            if (dgvResult.Columns["Nome"] != null)
                dgvResult.Columns["Nome"].FillWeight = 30;


            if (dgvResult.Columns["XmiId"] != null)
                dgvResult.Columns["XmiId"].FillWeight = 30;


            if (dgvResult.Columns["Pai"] != null)
                dgvResult.Columns["Pai"].FillWeight = 25;
        }
    }
}