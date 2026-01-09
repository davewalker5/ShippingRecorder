using System.Threading.Tasks;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IImporterExporter
    {
        Task ImportFromFileContentAsync(string content);
        Task ImportFromFileAsync(string filePath);
    }
}