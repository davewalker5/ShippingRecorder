using System.Threading.Tasks;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IImporter
    {
        Task ImportFromFileContentAsync(string content);
        Task ImportFromFileAsync(string filePath);
    }
}