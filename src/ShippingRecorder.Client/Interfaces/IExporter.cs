using System.Threading.Tasks;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IExporter
    {
        Task ExportAsync(string fileName);
    }
}