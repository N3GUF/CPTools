using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Comdata.AppSupport.AppTools;


namespace Comdata.AppSupport.PPOLFileMover
{
    public enum FileOperation
    { 
        Copy,
        Move,
        Delete
    }
    public class FileToMove                                                                                                                                                                                                                             
    {
        public string Source { get; set; }
        public string Destination { get; set; }
        public FileOperation Operation { set; get; }

        public static FileToMove Init(string source, string destination, FileOperation operation)
        {
            var f = new FileToMove();
            f.Source = source;
            f.Destination = destination;
            f.Operation = operation;
            return f;
        }
    }

    public class FileMoverSettings :ISettings
    {
        public string LogPath { get; set; }
        public Severity LoggingSeverityLevel { get; set; }
        public List<FileToMove> FilesToMove { get; set; }

        //public FileMoverSettings()
        //{
        //    this.LogPath = "C:\\Logs\\PPOLFileMover";
        //    this.LoggingSeverityLevel = Severity.Debug;
        //    this.FilesToMove = new List<FileToMove>();
        //    this.FilesToMove.Add(FileToMove.Init(@"\\cdnbwdata1\PLSDBATCH\CorpBalances", @"\\cdnbwdata1\PLSDBATCH\CorpBalances",FileOperation.Move));
        //    this.Save("config.xml");
        //}

        public FileMoverSettings()
        {
        }

        public FileMoverSettings(string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FileMoverSettings));
                TextReader tr = new StreamReader(filename);
                var tmp = (FileMoverSettings)serializer.Deserialize(tr);
                tr.Close();

                foreach (var property in GetType().GetProperties())
                    if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).GetLength(0) == 0)
                        property.SetValue(this, property.GetValue(tmp, null), null);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading settings from {0}.", filename), ex);
            }

        }

        public FileMoverSettings Reload(string filename)
        {
            FileMoverSettings returnValue = new FileMoverSettings();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FileMoverSettings));
                TextReader tr = new StreamReader(filename);
                returnValue = (FileMoverSettings)serializer.Deserialize(tr);
                tr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading settings from {0}.", filename), ex);
            }

            return (returnValue);
        }

        public bool Save(string filename)
        {
            bool returnValue = false;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(FileMoverSettings));
                TextWriter tw = new StreamWriter(filename);
                serializer.Serialize(tw, this);
                tw.Close();
                returnValue = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving settings.", ex);
            }

            return (returnValue);
        }
    } 
}
 
