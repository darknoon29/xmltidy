using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Mono.Options;

namespace xmlidy
{
    class Program {

        static bool _verbose;
        static bool _backup;

        static void Main(string[] args) {
            List<string> fileList = ParseCommandLine(args);
            Debug("Verbose output: ON");

            try {
                CheckFiles(fileList);

                foreach (var file in fileList) {
                    if (_backup) {
                        Debug("Backup file: {0}", file + ".original");
                        File.Copy(file, file + ".original");
                    }

                    XTidy(file);
                }
            } catch (FileNotFoundException fex) {
                Console.WriteLine("{0}: {1}", fex.Message, fex.FileName);
            } catch (Exception e) {
                Console.WriteLine("Error: {0}", e.Message);
            }

            Debug("Done!");
        }

        static void XTidy(string filename) {
            Debug("Tidy up file: {0}", filename);
            var doc = XDocument.Load(filename);
            doc.Save(filename, SaveOptions.None);
        }

        static List<string> ParseCommandLine(string[] args) {
            if (args.Length == 0)
                NoInput(null);

            bool showHelp = false;
            bool showVersion = false;
            List<string> ls = new List<string>();

            var p = new OptionSet() {  
                { "?|h|help", "Show this help message and exit.", x => showHelp = x != null },
                { "V|version", "Show version information and exit.", x => showVersion = x != null },
                { "v|verbose", "Increase message verbosity.", x => _verbose = x != null },
                { "b|backup", "Make a copy of input before tidying it.", x => _backup = x != null },
            };

            try {
                ls = p.Parse(args);
            } catch (OptionException oex) {
                NoInput(oex);
            }

            if (showHelp) {
                ShowUsage(p);
                Environment.Exit(0);
            }

            if (showVersion) {
                ShowVersion();
                Environment.Exit(0);
            }

            return ls;
        }

        static void ShowVersion() {
            Console.WriteLine("XMLTidy - Tidy up XML files.");
            Console.WriteLine("Version: 0.1.0.0");
        }

        static void ShowUsage(OptionSet p) {
            Console.WriteLine("Usage: xmltidy.exe [OPTIONS]* [FILENAME]+");
            Console.WriteLine("Properly formats and indents a valid XML file. At least one");
            Console.WriteLine("valid xml file name is expected. No options are required.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        static void NoInput(OptionException oex) {
            if (oex == null)
                Console.WriteLine("Warn: No input!");
            else {
                Console.Write("xmltidy: ");
                Console.WriteLine(oex.Message);
            }

            Console.WriteLine("Try `xmltidy.exe --help` for more information");
            Environment.Exit(0);
        }

        static void CheckFiles(List<string> files) {
            foreach (var file in files) {
                Debug("Checking file: {0}", file);
                if (!File.Exists(file))
                    throw new FileNotFoundException("File not found", file);
                else
                    Debug("File is good: {0}", file);
            }
        }

        static void Debug(string format, params object[] args) {
            if (_verbose) {
                string s = string.Format(format, args);
                Debug(s);
            }
        }

        static void Debug(string message) {
            if (_verbose)
                Console.WriteLine(message);
        }
    }
}