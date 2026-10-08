using System;
using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp.Models
{
    public class UmlElement
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public UmlElement Parent { get; set; }

        public List<UmlElement> Children { get; } = new();

        // Estereótipos aplicados a este elemento.
        public HashSet<string> AppliedStereotypes { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        // Referências existentes no XMI, por exemplo:
        // base_Classifier = "_123..."
        // base_Operation  = "_456..."
        public Dictionary<string, string> References { get; } =
            new(StringComparer.OrdinalIgnoreCase);

        public UmlElement(
            string id,
            string type,
            string name = null)
        {
            Id = id;
            Type = type;
            Name = name;
        }

        public IEnumerable<UmlElement> AllOwnedElements()
        {
            foreach (var child in Children)
            {
                yield return child;

                foreach (var descendant in child.AllOwnedElements())
                    yield return descendant;
            }
        }

        public IEnumerable<UmlElement> OwnedElements()
        {
            return Children;
        }

        public IEnumerable<UmlElement> GetOperations()
        {
            return Children.Where(x =>
                string.Equals(
                    x.Type,
                    "uml:Operation",
                    StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<UmlElement> GetMethods()
        {
            // Em UML, métodos são comportamentos associados a operações.
            //
            // Nesta primeira implementação procuramos comportamentos
            // diretamente relacionados ao elemento.
            return Children.Where(x =>
                x.Type != null &&
                x.Type.Contains("Behavior", StringComparison.OrdinalIgnoreCase));
        }

        public UmlElement GetBehavior()
        {
            return Children.FirstOrDefault(x =>
                x.Type != null &&
                x.Type.Contains("Behavior", StringComparison.OrdinalIgnoreCase));
        }

        public bool HasStereotype(string stereotypeName)
        {
            return AppliedStereotypes.Contains(stereotypeName);
        }

        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Name))
                return $"{Type} - {Name}";

            return $"{Type} - {Id}";
        }
    }
}