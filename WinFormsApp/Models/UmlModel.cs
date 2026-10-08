using WinFormsApp.Models;

namespace WinFormsApp.Models
{
    public class UmlModel : UmlElement
    {
        public UmlModel(
            string id,
            string type,
            string name = null)
            : base(id, type, name)
        {
        }
    }
}