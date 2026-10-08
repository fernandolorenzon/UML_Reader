using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WinFormsApp.Models;

namespace WinFormsApp.Services
{
    public class OclEvaluator
    {
        public List<UmlElement> Evaluate(
            UmlModel model,
            string ocl)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (string.IsNullOrWhiteSpace(ocl))
                throw new Exception("A expressão OCL está vazia.");

            OclQuery query = ParseQuery(ocl);

            return EvaluateBody(
                model,
                query.Body);
        }

        private OclQuery ParseQuery(string ocl)
        {
            string context = null;

            var bodyLines = new List<string>();

            string[] lines =
                ocl.Replace("\r\n", "\n")
                   .Replace("\r", "\n")
                   .Split('\n');

            foreach (string originalLine in lines)
            {
                string line = originalLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("--"))
                    continue;

                if (line.StartsWith("context ",
                        StringComparison.OrdinalIgnoreCase))
                {
                    context = line
                        .Substring("context ".Length)
                        .Trim();

                    continue;
                }

                bodyLines.Add(line);
            }

            if (string.IsNullOrWhiteSpace(context))
                throw new Exception(
                    "A expressão OCL não possui uma declaração de context.");

            string body = string.Join(
                " ",
                bodyLines);

            return new OclQuery
            {
                Context = context,
                Body = body
            };
        }

        private List<UmlElement> EvaluateBody(
            UmlModel model,
            string body)
        {
            string normalized =
                Regex.Replace(body, @"\s+", " ").Trim();

            // ---------------------------------------------------------
            // 1. self.allOwnedElements()
            // ---------------------------------------------------------

            const string rootExpression =
                "self.allOwnedElements()";

            if (!normalized.StartsWith(
                    rootExpression,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "A expressão inicial não é suportada.\n\n" +
                    "Era esperado:\n" +
                    "self.allOwnedElements()");
            }

            List<UmlElement> current =
                model.AllOwnedElements().ToList();

            normalized =
                normalized.Substring(
                    rootExpression.Length).Trim();

            // ---------------------------------------------------------
            // 2. Processa cada ->select(...)
            // ---------------------------------------------------------

            while (!string.IsNullOrWhiteSpace(normalized))
            {
                normalized = normalized.Trim();

                if (!normalized.StartsWith(
                        "->select(",
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(
                        "Operação OCL não suportada:\n\n" +
                        normalized);
                }

                int openParenthesis =
                    normalized.IndexOf('(');

                int closeParenthesis =
                    FindMatchingParenthesis(
                        normalized,
                        openParenthesis);

                if (closeParenthesis < 0)
                {
                    throw new Exception(
                        "Parênteses não balanceados na operação select.");
                }

                string selectExpression =
                    normalized.Substring(
                        openParenthesis + 1,
                        closeParenthesis - openParenthesis - 1);

                current =
                    ApplySelect(
                        current,
                        selectExpression);

                normalized =
                    normalized.Substring(
                        closeParenthesis + 1).Trim();
            }

            return current;
        }

        private List<UmlElement> ApplySelect(
            List<UmlElement> source,
            string expression)
        {
            // Esperamos algo como:
            //
            // s | s.getAppliedStereotype('SchedulableResource')
            //
            // ou:
            //
            // o | o.getOperations()

            int separator =
                expression.IndexOf('|');

            if (separator < 0)
            {
                throw new Exception(
                    "select inválido:\n" + expression);
            }

            string variable =
                expression.Substring(
                    0,
                    separator)
                .Trim();

            string condition =
                expression.Substring(
                    separator + 1)
                .Trim();

            if (string.IsNullOrWhiteSpace(variable))
                throw new Exception(
                    "Variável inválida no select.");

            return source
                .Where(element =>
                    EvaluateCondition(
                        element,
                        variable,
                        condition))
                .ToList();
        }

        private bool EvaluateCondition(
            UmlElement element,
            string variable,
            string condition)
        {
            condition = condition.Trim();

            string prefix =
                variable + ".";

            if (!condition.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    $"A expressão '{condition}' não começa com " +
                    $"'{variable}.'");
            }

            string operation =
                condition.Substring(
                    prefix.Length)
                .Trim();

            // ---------------------------------------------------------
            // getAppliedStereotype('Nome')
            // ---------------------------------------------------------

            Match stereotypeMatch =
                Regex.Match(
                    operation,
                    @"^getAppliedStereotype\s*\(\s*['""]([^'""]+)['""]\s*\)$",
                    RegexOptions.IgnoreCase);

            if (stereotypeMatch.Success)
            {
                string stereotype =
                    stereotypeMatch.Groups[1].Value;

                return element.HasStereotype(
                    stereotype);
            }

            // ---------------------------------------------------------
            // getOperations()
            // ---------------------------------------------------------

            Match operationsMatch =
                Regex.Match(
                    operation,
                    @"^getOperations\s*\(\s*\)$",
                    RegexOptions.IgnoreCase);

            if (operationsMatch.Success)
            {
                return element
                    .GetOperations()
                    .Any();
            }

            // ---------------------------------------------------------
            // getMethods()
            // ---------------------------------------------------------

            Match methodsMatch =
                Regex.Match(
                    operation,
                    @"^getMethods\s*\(\s*\)$",
                    RegexOptions.IgnoreCase);

            if (methodsMatch.Success)
            {
                return element
                    .GetMethods()
                    .Any();
            }

            // ---------------------------------------------------------
            // getBehavior()
            // ---------------------------------------------------------

            Match behaviorMatch =
                Regex.Match(
                    operation,
                    @"^getBehavior\s*\(\s*\)$",
                    RegexOptions.IgnoreCase);

            if (behaviorMatch.Success)
            {
                return element.GetBehavior() != null;
            }

            throw new Exception(
                $"Operação OCL não suportada:\n\n" +
                condition);
        }

        private int FindMatchingParenthesis(
            string text,
            int openingPosition)
        {
            int level = 0;

            char quote = '\0';

            for (int i = openingPosition;
                 i < text.Length;
                 i++)
            {
                char c = text[i];

                // Trata strings OCL.
                if (c == '\'' || c == '"')
                {
                    if (quote == '\0')
                    {
                        quote = c;
                    }
                    else if (quote == c)
                    {
                        quote = '\0';
                    }

                    continue;
                }

                if (quote != '\0')
                    continue;

                if (c == '(')
                    level++;

                else if (c == ')')
                {
                    level--;

                    if (level == 0)
                        return i;
                }
            }

            return -1;
        }
    }

    public class OclQuery
    {
        public string Context { get; set; }

        public string Body { get; set; }
    }
}