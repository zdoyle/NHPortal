using GDCoreUtilities;
using GDCoreUtilities.IO;
using GDCoreUtilities.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace NHPortal.Classes
{
    /// <summary>Uses for converting NH reports stored in a CSV file to a DataTable.</summary>
    public class NHPortalCSVReader
    {
        private const char DELIMITER = ',';
        private readonly char[] m_splitOn;
        private readonly string m_file;
        private PeekableStreamReader m_reader;

        /// <summary>Instantiates a new instance of the CSVReader class.</summary>
        /// <param name="file">Full path of the file to read.</param>
        public NHPortalCSVReader(string file)
        {
            FileInfo fi;
            if (FileHelper.TryGetFileInfo(file, out fi))
            {
                if (StringUtilities.AreEqual(".csv", fi.Extension))
                {
                    m_splitOn = new char[] { DELIMITER };
                    m_file = file;
                }
                else
                {
                    throw new IOException("CSV file not provided to the NHPortalCSVReader.");
                }
            }
        }

        public void Process()
        {
            Table = new DataTable();
            ReadCSV();
        }

        private void ReadCSV()
        {
            try
            {
                using (m_reader = new PeekableStreamReader(m_file))
                {
                    ReadUntilDataStart();
                    ReadHeader();
                    ReadData();
                }
            }
            catch (IOException ex)
            {
                NHPortalUtilities.LogSessionException(ex, "IOException in CSVReader.ReadCSV");
            }
        }

        private void ReadUntilDataStart()
        {
            bool read = true;

            string line;
            while (read && TryReadLine(out line))
            {
                if (DataSeparatorLine(line))
                {
                    read = false;
                }
            }
        }

        private void ReadHeader()
        {
            // assume the header is comprised of two lines
            string lineOne = String.Empty;
            string lineTwo = String.Empty;
            if (TryReadLine(out lineOne) && TryReadLine(out lineTwo))
            {
                string[] headerOne = SplitLine(lineOne);
                string[] headerTwo = SplitLine(lineTwo);

                int max = (headerOne.Length >= headerTwo.Length ? headerOne.Length : headerTwo.Length);
                for (int i = 0; i < max; i++)
                {
                    string colName = (ReadArrayValue(headerOne, i) + " " + ReadArrayValue(headerTwo, i)).Trim();
                    if (!String.IsNullOrEmpty(colName))
                    {
                        Table.Columns.Add(colName);
                    }
                }
            }
            else
            {
                NHPortalUtilities.LogSessionMessage("CSVReader.ReadHeader: lineOne and lineTwo are both null or empty strings",
                    LogSeverity.Information);
                NHPortalUtilities.LogSessionMessage("lineOne: " + lineOne, LogSeverity.Information);
                NHPortalUtilities.LogSessionMessage("lineTwo: " + lineTwo, LogSeverity.Information);

                // TODO: should probably throw an exception here
            }
        }

        private void ReadData()
        {
            bool read = true;

            string line;
            while (read && TryReadLine(out line))
            {
                if (DataSeparatorLine(line))
                {
                    read = false;
                }
                else if (!String.IsNullOrWhiteSpace(line))
                {
                    Table.Rows.Add(SplitLine(line));
                }
            }
        }

        private string ReadArrayValue(string[] arr, int index)
        {
            string val = String.Empty;
            if (index < arr.Length)
            {
                val = arr[index];
            }
            return val;
        }


        private bool TryReadLine(out string line)
        {
            line = String.Empty;
            bool readLine = false;
            try
            {
                if (m_reader != null)
                {
                    line = m_reader.ReadLine();
                    readLine = (line != null);
                }
            }
            catch (IOException ex)
            {
                NHPortalUtilities.LogSessionException(ex, "IOException in NHPortalCSVReader.TryReadLine");
            }
            return readLine;
        }

        private string[] SplitLine(string line)
        {
            string[] parts = line.Split(m_splitOn);
            return parts.Skip(1).ToArray(); // ignore the first part of the line -- seems to be for formatting
        }

        private bool DataSeparatorLine(string line)
        {
            return StringUtilities.AreEqual("-", line);
        }


        /// <summary>Gets the DataTable representing the contents of the CSV file.</summary>
        public DataTable Table { get; private set; }
    }
}