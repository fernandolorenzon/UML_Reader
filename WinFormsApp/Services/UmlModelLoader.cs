using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using WinFormsApp.Models;

namespace WinFormsApp.Services
{
    public class UmlModelLoader
    {
        private static readonly XNamespace Xmi =
            "http://www.omg.org/spec/XMI/20131001";

        private static readonly XNamespace Uml =
            "http://www.eclipse.org/uml2/5.0.0/UML";

        public UmlModel Load(string filePath)
        {
            XDocument document = XDocument.Load(filePath);

            XElement modelElement =
                document.Descendants(Uml + "Model").FirstOrDefault();

            if (modelElement == null)
                throw new Exception(
                    "Não foi encontrado um elemento uml:Model no arquivo UML.");

            var model = new UmlModel(
                GetXmiId(modelElement),
                "uml:Model",
                modelElement.Attribute("name")?.Value);

            // Primeiro constrói a árvore UML.
            foreach (var child in modelElement.Elements())
            {
                if (IsUmlElement(child))
                {
                    LoadElement(child, model);
                }
            }

            // Depois processa os estereótipos aplicados.
            LoadAppliedStereotypes(document, model);

            return model;
        }

        private void LoadElement(
            XElement xmlElement,
            UmlElement parent)
        {
            string type = GetXmiType(xmlElement);

            if (string.IsNullOrWhiteSpace(type))
                return;

            // Só carregamos elementos UML.
            if (!type.StartsWith("uml:", StringComparison.OrdinalIgnoreCase))
                return;

            string id = GetXmiId(xmlElement);
            string name = xmlElement.Attribute("name")?.Value;

            var element = new UmlElement(
                id,
                type,
                name);

            element.Parent = parent;

            parent.Children.Add(element);

            foreach (var child in xmlElement.Elements())
            {
                if (IsUmlElement(child))
                {
                    LoadElement(child, element);
                }
            }
        }

        private void LoadAppliedStereotypes(
            XDocument document,
            UmlModel model)
        {
            var elementsById = model
                .AllOwnedElements()
                .Append(model)
                .Where(x => !string.IsNullOrWhiteSpace(x.Id))
                .GroupBy(x => x.Id)
                .ToDictionary(
                    g => g.Key,
                    g => g.First());

            foreach (var element in document.Root.Elements())
            {
                ProcessStereotypeElement(
                    element,
                    elementsById);
            }
        }

        private void ProcessStereotypeElement(
            XElement element,
            Dictionary<string, UmlElement> elementsById)
        {
            string localName = element.Name.LocalName;

            // Elementos uml:* já foram tratados pela árvore.
            if (element.Name.Namespace == Uml)
                return;

            string xmiId = GetXmiId(element);

            if (!string.IsNullOrWhiteSpace(xmiId))
            {
                // Procura uma referência do tipo:
                //
                // base_Classifier="..."
                // base_Operation="..."
                // base_Class="..."
                // base_TimeEvent="..."
                //
                // O alvo dessa referência é o elemento UML que recebeu
                // o estereótipo.

                foreach (var attribute in element.Attributes())
                {
                    if (!attribute.Name.LocalName.StartsWith(
                            "base_",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string targetId = attribute.Value;

                    if (elementsById.TryGetValue(
                            targetId,
                            out UmlElement target))
                    {
                        target.AppliedStereotypes.Add(localName);

                        target.References[attribute.Name.LocalName] =
                            targetId;
                    }
                }
            }

            // Alguns elementos de estereótipo podem possuir conteúdo
            // interno relevante.
            foreach (var child in element.Elements())
            {
                ProcessStereotypeElement(
                    child,
                    elementsById);
            }
        }

        private bool IsUmlElement(XElement element)
        {
            string type = GetXmiType(element);

            return !string.IsNullOrWhiteSpace(type)
                   && type.StartsWith(
                       "uml:",
                       StringComparison.OrdinalIgnoreCase);
        }

        private string GetXmiType(XElement element)
        {
            return element.Attribute(Xmi + "type")?.Value;
        }

        private string GetXmiId(XElement element)
        {
            return element.Attribute(Xmi + "id")?.Value;
        }
    }
}