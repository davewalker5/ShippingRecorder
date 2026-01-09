using System.ComponentModel;
using ShippingRecorder.Mvc.Enumerations;
using ShippingRecorder.Mvc.Helpers;

namespace ShippingRecorder.Mvc.Models
{
    public class DataExchangeViewModel
    {
        [DisplayName("Data Type")]
        public DataExchangeType DataExchangeType { get; set; }

        public string DataExchangeTypeName { get { return DataExchangeType.ToName(); } }
    }
}