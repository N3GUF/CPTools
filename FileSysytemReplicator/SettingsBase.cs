using Comdata.AppSupport.AppTools;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Comdata.AppSupport.FileSystemReplicater
{
    public class SettingsBase
    {
        public SettingsBase()
        {
        }

        public SettingsBase(string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                TextReader tr = new StreamReader(filename);
                var tmp = (Settings)serializer.Deserialize(tr);
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

        public Settings Reload(string filename)
        {
            Settings returnValue = new Settings();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                TextReader tr = new StreamReader(filename);
                returnValue = (Settings)serializer.Deserialize(tr);
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
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
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
