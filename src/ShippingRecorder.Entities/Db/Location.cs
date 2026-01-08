using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Location : ShippingRecorderEntityBase, INamedEntity
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}