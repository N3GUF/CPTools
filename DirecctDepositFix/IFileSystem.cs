using Comdata.AppSupport.AppTools;

namespace Comdata.AppSupport.DirectDepositFix
{
    public interface IFileSystem
    {
        ILog Log { get; set; }

        void Copy(string source, string dest);

        void Move(string source, string dest);

        void Delete(string source);

        string FindCurrentFile(string path, string pattern);
    }
}
