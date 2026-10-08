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


        public UmlModel Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(
                    "Arquivo UML não encontrado.",
                    fileName);
            }


            XDocument document =
                XDocument.Load(fileName);


            XElement? modelElement =
                document
                    .Descendants(Uml + "Model")
                    .FirstOrDefault();


            if (modelElement == null)
            {
                throw new InvalidOperationException(
                    "O arquivo não possui um elemento uml:Model.");
            }


            string? id =
                GetAttribute(
                    modelElement,
                    Xmi,
                    "id");


            string? name =
                GetAttribute(
                    modelElement,
                    null,
                    "name");


            UmlModel model =
                new UmlModel(
                    id,
                    "uml:Model",
                    name);


            foreach (
                XElement child
                in modelElement.Elements())
            {
                LoadElement(
                    child,
                    model);
            }


            return model;
        }


        private void LoadElement(
            XElement xmlElement,
            UmlElement parent)
        {
            XAttribute? typeAttribute =
                xmlElement.Attribute(
                    Xmi + "type");


            // Elementos sem xmi:type podem ser apenas
            // contêineres XML. Continuamos procurando
            // elementos UML dentro deles.

            if (typeAttribute == null)
            {
                foreach (
                    XElement child
                    in xmlElement.Elements())
                {
                    LoadElement(
                        child,
                        parent);
                }

                return;
            }


            string type =
                typeAttribute.Value;


            // Nesta versão estamos interessados somente
            // nos elementos UML.
            //
            // Ignora, por exemplo:
            //
            // ecore:EAnnotation
            // ecore:EStringToStringMapEntry

            if (!type.StartsWith("uml:"))
            {
                return;
            }


            string? id =
                GetAttribute(
                    xmlElement,
                    Xmi,
                    "id");


            string? name =
                GetAttribute(
                    xmlElement,
                    null,
                    "name");


            UmlElement element =
                new UmlElement(
                    id,
                    type,
                    name);


            element.Parent = parent;

            parent.Children.Add(element);


            foreach (
                XElement child
                in xmlElement.Elements())
            {
                LoadElement(
                    child,
                    element);
            }
        }


        private string? GetAttribute(
            XElement element,
            XNamespace? namespaceName,
            string attributeName)
        {
            XAttribute? attribute;


            if (namespaceName == null)
            {
                attribute =
                    element.Attribute(
                        attributeName);
            }
            else
            {
                attribute =
                    element.Attribute(
                        namespaceName + attributeName);
            }


            return attribute?.Value;
        }
    }
}