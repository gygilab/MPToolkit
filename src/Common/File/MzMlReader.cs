using MPToolkit.Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace MPToolkit.Common.File
{
    public class MzMlReader : IScanReader
    {
        private XmlReader Reader;

        private ScanFileHeader Header = new ScanFileHeader();

        private string FilePath;

        public Dictionary<string, string> scanAttrs = new Dictionary<string, string>()
        {
            { "ms level", "MsOrder" },
            { "total ion current", "TotalIonCurrent" },
            { "scan start time", "RetentionTime" }, // Time is in minutes.
            { "collision energy", "CollisionEnergy" },
            { "base peak m/z", "BasePeakMz" },
            { "base peak intensity", "BasePeakIntensity" },
            { "scan window lower limit", "StartMz" },
            { "scan window upper limit", "EndMz" },
            { "lowest observed m/z", "LowestMz" },
            { "highest observed m/z", "HighestMz" },
            { "filter string", "FilterLine" }
        };

        public Dictionary<string, string> precursorAttrs = new Dictionary<string, string>()
        {
            // Precusor information
            { "selected ion m/z", "Mz" },
            { "charge state", "Charge" }
        };

        /// <summary>
        /// Open new fileStream to mzML file.
        /// </summary>
        /// <param name="path"></param>
        public void Open(string path, ScanReaderOptions options)
        {
            if (!System.IO.File.Exists(path))
            {
                throw new IOException("File not found: " + path);
            }
            FilePath = path;
            Reader = XmlReader.Create(FilePath);
            // ReadHeader();
        }

        /// <summary>
        /// Returns header information from the mzXML file.
        /// </summary>
        /// <returns>An instance of the ScanFileHeader class</returns>
        public ScanFileHeader GetHeader()
        {
            return Header;
        }

        /// <summary>
        /// Dispose of the reader when reading multiple files.
        /// </summary>
        public void Close()
        {
            Reader.Dispose();
        }

        /// <summary>
        /// Open the given file and import scans into the reader.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            // Reset to beginning of document.
            Reader = XmlReader.Create(FilePath);
            Scan scan = null;
            Precursor precursor = null;
            var binaryData = new BinaryData();
            while (Reader.Read())
            {
                if (Reader.NodeType == XmlNodeType.Element)
                {
                    if (Reader.Name == "spectrum")
                    {
                        // Using spectrum as a start of scan.
                        // <spectrum index="11" defaultArrayLength="113" id="index=12">
                        // Using id attr for scan numbers
                        scan = new Scan();
                        ReadSpectrumAttrs(scan);
                    }
                    else if (Reader.Name == "precursor")
                    {
                        // Precursor contains the precursor scan number.
                        // <precursor spectrumRef="index=11">
                    }
                    else if (Reader.Name == "selectedIon")
                    {
                        precursor = new Precursor();
                    }
                    else if (Reader.Name == "cvParam")
                    {
                        var cvParam = ReadCVParam();
                        SetAttribute(cvParam, scan, precursor);
                    }
                    else if (Reader.Name == "binaryDataArray") {
                        ReadBinaryData(binaryData, scan.PeakCount);
                    }
                }
                else if (Reader.NodeType == XmlNodeType.EndElement)
                {
                    // Reached a closing tag.
                    if (Reader.Name == "spectrum")
                    {
                        scan.Centroids.Clear();
                        for (int i = 0; i < scan.PeakCount; ++i) {
                            scan.Centroids.Add(
                                new Centroid(
                                    binaryData.mzs[i],
                                    binaryData.intensities[i])
                            );
                        }
                        yield return scan;
                    }

                    if (Reader.Name == "selectedIon")
                    {
                        scan.Precursors.Add(precursor);
                    }
                    else if (Reader.Name == "spectrumList") {
                        break;
                    }
                }
               
            }
        }

        /// <summary>
        /// Reads attributes from the spectrum element
        /// and assigns the data to the scan.
        /// 
        /// ex: <spectrum index="0" defaultArrayLength="17242" id="index=1">
        /// </summary>
        private void ReadSpectrumAttrs(Scan scan)
        {
            while (Reader.MoveToNextAttribute())
            {
                if (Reader.Name == "id")
                {
                    scan.ScanNumber = ScanIDToScanNumber(Reader.Value);
                }
                else if (Reader.Name == "defaultArrayLength")
                {
                    scan.PeakCount = int.Parse(Reader.Value.Replace("defaultArrayLength=", ""));
                }
            }
        }

        /// <summary>
        /// Check and set attribute based on attributes dictionary
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        private void SetAttribute(CVParam cvParam, Scan scan, Precursor precursor)
        {
            string member = "";
            object o = null;
            if (scanAttrs.ContainsKey(cvParam.Name))
            {
                member = scanAttrs[cvParam.Name];
                o = scan;
            }
            else if (precursorAttrs.ContainsKey(cvParam.Name))
            {
                member = precursorAttrs[cvParam.Name];
                o = precursor;
            }

            if (member == "")
            {
                return;
            }

            var prop = o.GetType().GetProperty(member);
            var value = Convert.ChangeType(cvParam.Value, prop.PropertyType);
            prop.SetValue(o, value);
        }

        private void ReadHeader()
        {
            Header.FileName = Path.GetFileName(FilePath);
            while (Reader.Read())
            {
                switch (Reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (Reader.Name == "msRun")
                        {
                            while (Reader.MoveToNextAttribute())
                            {
                                switch (Reader.Name)
                                {
                                    case "scanCount":
                                        Header.ScanCount = int.Parse(Reader.Value);
                                        break;
                                    case "startTime":
                                        Header.StartTime = ParseRetentionTime(Reader.Value);
                                        break;
                                    case "endTime":
                                        Header.EndTime = ParseRetentionTime(Reader.Value);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else if (Reader.Name == "msManufacturer")
                        {
                            while (Reader.MoveToNextAttribute())
                            {
                                if (Reader.Name == "value")
                                {
                                    Header.InstrumentManufacturer = Reader.Value;
                                }
                            }
                        }
                        else if (Reader.Name == "msModel")
                        {
                            while (Reader.MoveToNextAttribute())
                            {
                                if (Reader.Name == "value")
                                {
                                    Header.InstrumentModel = Reader.Value;
                                }
                            }
                        }
                        else if (Reader.Name == "scan")
                        {
                            // Gone too far.
                            Reader.Dispose();
                            Reader = XmlReader.Create(FilePath);
                            return;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (Reader.Name == "msInstrument")
                        {
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ReadBinaryData(BinaryData binaryData, int peakCount) {
            string data = "";
            bool isMz = true;
            bool is64bit = true;
            while(Reader.Read()) {
                if (Reader.NodeType == XmlNodeType.Element) {
                    if (Reader.Name == "cvParam") {
                        var cvParam = ReadCVParam();
                        if (cvParam.Name == "intensity array") {
                            isMz = false;
                        }
                        else if (cvParam.Name == "32-bit float") {
                            is64bit = false;
                        }
                    }
                    else if(Reader.Name == "binary") {
                        data = Reader.ReadElementContentAsString();
                    }
                }
                else if (Reader.NodeType == XmlNodeType.EndElement) {
                    if (Reader.Name == "binaryDataArray") {
                        break;
                    }
                }
            }

            var values = ReadPeaks(data, peakCount, is64bit);
            if (isMz) {
                binaryData.mzs = values;
            }
            else {
                binaryData.intensities = values.ConvertAll(x => (float) x);
            }
        }

        private List<double> ReadPeaks(string base64Data, int peakCount, bool is64bit) {
            var output = new List<double>(new double[peakCount]);
            
            byte[] byteEncoded = Convert.FromBase64String(base64Data);
            for(int i = 0; i < peakCount; ++i) {
                if (is64bit) {
                    output[i] = BitConverter.ToDouble(byteEncoded, i * 8);
                }
                else {
                    output[i] = BitConverter.ToSingle(byteEncoded, i * 4);
                }
            }
            return output;
        }

        /// <summary>
        /// Read mzXML peaks property
        /// </summary>
        /// <param name="str"></param>
        /// <param name="peakCount"></param>
        /// <returns></returns>
        private List<Centroid> ReadPeaks(string str, int peakCount)
        {
            List<Centroid> peaks = new List<Centroid>();
            int size = peakCount * 2;
            if (String.Compare(str, "AAAAAAAAAAA=") == 0)
            {
                return peaks;
            }
            byte[] byteEncoded = Convert.FromBase64String(str);
            Array.Reverse(byteEncoded);
            float[] values = new float[size];
            for (int i = 0; i < size; i++)
            {
                values[i] = BitConverter.ToSingle(byteEncoded, i * 4);
            }
            Array.Reverse(values);
            for (int i = 0; i < peakCount; ++i)
            {
                Centroid tempCent = new Centroid(values[2 * i], values[(2 * i) + 1]);
                peaks.Add(tempCent);
            }
            return peaks;
        }

        private void Cleanup()
        {
            if (Reader != null)
            {
                ((IDisposable)Reader).Dispose();
            }
        }

        /// <summary>
        /// Converts retention time text into the number of minutes.
        /// 
        /// Input text is of the type xsd:duration
        /// </summary>
        /// <param name="text">Input, e.g. "PT2530.331S"</param>
        /// <returns>Number of Minutes</returns>
        private double ParseRetentionTime(string text)
        {
            try
            {
                if (text.StartsWith("PT"))
                {
                    var span = XmlConvert.ToTimeSpan(text);
                    return span.TotalMinutes;
                }

                return float.Parse(text);
            }
            catch (FormatException)
            {
                return 0;
            }
        }

        // Reads a CV param tag into the CVParam struct
        // <cvParam cvRef="PSI-MS" accession="MS:1000045"
        //   name="collision energy" value="24.46"
        //   unitCvRef="UO" unitAccession="UO:0000266" unitName="electronvolt"/>
        private CVParam ReadCVParam()
        {
            var data = new CVParam();
            while (Reader.MoveToNextAttribute())
            {
                switch (Reader.Name)
                {
                    case "cvRef":
                        data.CVRef = Reader.Value;
                        break;
                    case "accession":
                        data.Accession = Reader.Value;
                        break;
                    case "name":
                        data.Name = Reader.Value;
                        break;
                    case "value":
                        data.Value = Reader.Value;
                        break;
                    case "unitCvRef":
                        data.UnitCvRef = Reader.Value;
                        break;
                    case "unitAccession":
                        data.UnitAccession = Reader.Value;
                        break;
                    case "unitName":
                        data.UnitName = Reader.Value;
                        break;
                }
            }
            return data;
        }

        private static Regex ScanNumRegex = new Regex(@"(scan|index)=(\d+)");

        /// <summary>
        /// Read the scan number from the id string
        /// in the spectrum element.
        /// 
        /// Example1:
        /// <spectrum index="0"
        ///      id="controllerType=0 controllerNumber=1 scan=1"
        ///      defaultArrayLength="19914">
        /// 
        /// Example2:
        /// <spectrum index="0" defaultArrayLength="17242" id="index=1">
        /// 
        /// 
        /// </summary>
        /// <param name="idString"></param>
        /// <returns></returns>
        private int ScanIDToScanNumber(string idString) {

            int scanId = 0;
            foreach(Match match in ScanNumRegex.Matches(idString)) {
                scanId = int.Parse(match.Groups[2].Value);
            }
            if (scanId == 0) {
                throw new InvalidDataException("Scan number not found in spectrum tag");
            }
            return scanId;
        }
    }

    internal struct CVParam
    {
        public string CVRef;
        public string Accession;
        public string Name;
        public string Value;
        public string UnitCvRef;
        public string UnitAccession;
        public string UnitName;
    }

    internal class BinaryData {
        public List<double> mzs;
        public List<float> intensities;
    }

}
