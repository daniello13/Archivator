namespace Archiver.Processors
{
    interface IProcessFile
    {
        string ProcessExecute(string fileName);
        string BackProcessExecute(string fileName);
    }
}