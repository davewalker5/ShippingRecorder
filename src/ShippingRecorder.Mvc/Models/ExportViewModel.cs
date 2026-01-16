using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShippingRecorder.Mvc.Enumerations;
using ShippingRecorder.Mvc.Helpers;

namespace ShippingRecorder.Mvc.Models
{
    public class ExportViewModel : DataExchangeViewModel
    {
        public List<SelectListItem> ExportTypes { get; private set; } = [];

        [DisplayName("File Name")]
        [Required(ErrorMessage = "You must provide an export file name")]
        public string FileName { get; set; }

        public string Message { get; set; } = "";

        public ExportViewModel()
        {
            foreach (var exportType in Enum.GetValues<DataExchangeType>())
            {
                var importTypeName = exportType.ToName();
                ExportTypes.Add(new SelectListItem() { Text = $"{importTypeName}", Value = exportType.ToString() });
            }
        }
    }
}