using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.PPOLTestFileGenerator.Model
{
    public static class Address
    {
        public static string Name;
        public static string Line1;
        public static string Line2;
        public static string City;
        public static string State;
        public static string PostalCd;
        public static string Country;

        public static void Parse(string address)
        {
            Name = "";
            Line1 = "";
            Line2 = "";
            City = "";
            State = "";
            PostalCd = "";
            Country = "";
            var line = address.Split(new char[] { '\n' });

            if (line.Count() < 1)
                return;

            Name = line[0].Trim();

            if (line.Count() < 2)
                return;

            Line1 = line[1].Trim();

            if (line.Count() < 3)
                return;

            if (line[2].Contains(","))
                parseCityStZipCountry(line[2], ref City, ref State, ref PostalCd, ref Country);
            else
                Line2 = line[2].Trim();

            if (line.Count() < 4)
                return;

            if (line[3].Contains(","))
                parseCityStZipCountry(line[3], ref City, ref State, ref PostalCd, ref Country);
        }

        private static void parseCityStZipCountry(string str, ref string city, ref string state, ref string postalCd, ref string country)
        {
            var part = str.Split(new char[] { ',' });

            if (part.Count() < 1)
                return;

            city = part[0].Trim();
            part = part[1].Split(new char[] { ' ' });

            if (part.Count() < 2)
                return;

            state = part[1].Trim();

            if (part.Count() < 3)
                return;

            postalCd = part[2].Trim();

            if (part.Count() < 4)
                return;

            country = part[3].Trim();
        }
    }
}
