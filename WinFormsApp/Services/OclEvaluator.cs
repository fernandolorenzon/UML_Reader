using WinFormsApp.Models;

namespace WinFormsApp.Services
{
    public class OclEvaluator
    {
        public List<UmlElement> Evaluate(
            UmlModel model,
            string expression)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException(
                    "A expressão OCL está vazia.");


            // ====================================================
            // 1. Extrair contexto e corpo da expressão
            // ====================================================

            OclQuery query =
                ParseQuery(expression);


            // ====================================================
            // 2. Executar a expressão
            // ====================================================

            return EvaluateBody(
                model,
                query);
        }


        // ========================================================
        // PARSER INICIAL
        // ========================================================

        private OclQuery ParseQuery(
            string expression)
        {
            string[] lines =
                expression.Split(
                    new[]
                    {
                        "\r\n",
                        "\n",
                        "\r"
                    },
                    StringSplitOptions.None);


            List<string> cleanLines =
                new List<string>();


            foreach (string line in lines)
            {
                string clean =
                    line.Trim();


                // Ignora linhas vazias
                if (string.IsNullOrWhiteSpace(clean))
                    continue;


                // Comentário OCL
                if (clean.StartsWith("--"))
                    continue;


                cleanLines.Add(clean);
            }


            if (cleanLines.Count == 0)
            {
                throw new InvalidOperationException(
                    "A expressão OCL não contém código.");
            }


            string? context = null;

            List<string> bodyLines =
                new List<string>();


            // ====================================================
            // Procura:
            //
            // context Element
            // ====================================================

            foreach (string line in cleanLines)
            {
                if (line.StartsWith(
                    "context ",
                    StringComparison.OrdinalIgnoreCase))
                {
                    context =
                        line.Substring(
                            "context ".Length)
                        .Trim();

                    continue;
                }


                bodyLines.Add(line);
            }


            string body =
                string.Join(
                    " ",
                    bodyLines)
                .Trim();


            if (string.IsNullOrWhiteSpace(body))
            {
                throw new InvalidOperationException(
                    "Nenhuma expressão foi encontrada " +
                    "após a declaração de contexto.");
            }


            return new OclQuery(
                context,
                body);
        }


        // ========================================================
        // AVALIADOR
        // ========================================================

        private List<UmlElement> EvaluateBody(
            UmlModel model,
            OclQuery query)
        {
            string expression =
                Normalize(
                    query.Body);


            // ====================================================
            // self.allOwnedElements()
            // ====================================================

            if (expression.Equals(
                "self.allOwnedElements()",
                StringComparison.OrdinalIgnoreCase))
            {
                return model
                    .AllOwnedElements()
                    .ToList();
            }


            // ====================================================
            // self.ownedElement
            // ====================================================

            if (expression.Equals(
                "self.ownedElement",
                StringComparison.OrdinalIgnoreCase))
            {
                return model.Children.ToList();
            }


            // ====================================================
            // self.ownedElements
            //
            // Deixamos também essa forma porque alguns modelos
            // ou consultas podem utilizar pluralização.
            // ====================================================

            if (expression.Equals(
                "self.ownedElements",
                StringComparison.OrdinalIgnoreCase))
            {
                return model.Children.ToList();
            }


            throw new NotSupportedException(
                "A expressão OCL ainda não é suportada.\n\n" +

                "Contexto: " +
                (query.Context ?? "(não especificado)") +

                "\n\nExpressão:\n" +
                query.Body +

                "\n\nExpressões disponíveis nesta versão:\n" +

                "self.ownedElement\n" +
                "self.ownedElements\n" +
                "self.allOwnedElements()");
        }


        // ========================================================
        // NORMALIZAÇÃO
        // ========================================================

        private string Normalize(
            string expression)
        {
            return string.Join(
                    " ",
                    expression
                        .Split(
                            (char[])null!,
                            StringSplitOptions.RemoveEmptyEntries))
                .Trim();
        }
    }


    // ============================================================
    // REPRESENTAÇÃO DE UMA CONSULTA OCL
    // ============================================================

    public class OclQuery
    {
        public string? Context { get; }

        public string Body { get; }


        public OclQuery(
            string? context,
            string body)
        {
            Context = context;

            Body = body;
        }
    }
}