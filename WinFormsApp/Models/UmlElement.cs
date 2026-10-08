namespace WinFormsApp.Models
{
    public class UmlElement
    {
        public string? Id { get; set; }

        public string? Type { get; set; }

        public string? Name { get; set; }

        public UmlElement? Parent { get; set; }


        public List<UmlElement> Children { get; }
            = new List<UmlElement>();


        public UmlElement(
            string? id,
            string? type,
            string? name = null)
        {
            Id = id;
            Type = type;
            Name = name;
        }


        public IEnumerable<UmlElement> AllOwnedElements()
        {
            foreach (UmlElement child in Children)
            {
                yield return child;


                foreach (
                    UmlElement descendant
                    in child.AllOwnedElements())
                {
                    yield return descendant;
                }
            }
        }


        public override string ToString()
        {
            return $"{Type} - {Name}";
        }
    }
}