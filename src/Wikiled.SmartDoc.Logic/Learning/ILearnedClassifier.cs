using System.IO;
using System.Threading.Tasks;

namespace Wikiled.SmartDoc.Logic.Learning
{
    public interface ILearnedClassifier
    {
        Task<string> Classify(FileInfo file);
    }
}