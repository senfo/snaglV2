//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ionic.Zip;
using Microsoft.Win32;

namespace Berico.SnagL.ConfigurationEditor.ViewModel
{
    /// <summary>
    /// Contains the main view model for the application
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Used to store the unique identifier for the directory where files are extracted
        /// </summary>
        private Guid _directoryId = Guid.NewGuid();

        /// <summary>
        /// Stores the path where the XAP file is extracted to
        /// </summary>
        private string _destinationDirectory;

        /// <summary>
        /// Stores a reference to the context of the XML file
        /// </summary>
        private string _documentContent;

        /// <summary>
        /// Stores the name of the XAP file we're working with
        /// </summary>
        private string _fileName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the content of the XML configuration file
        /// </summary>
        public string DocumentContent
        {
            get
            {
                return _documentContent;
            }
            set
            {
                _documentContent = value;
                RaisePropertyChanged("DocumentContent");
            }
        }

        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                RaisePropertyChanged("SaveEnabled");
            }
        }

        /// <summary>
        /// Gets a value, which indicates whether or not the save button is enabled
        /// </summary>
        public bool SaveEnabled
        {
            get
            {
                return !String.IsNullOrWhiteSpace(FileName);
            }
        }

        /// <summary>
        /// Exposes a reference to an ICommand, which shows the OpenFileDialog
        /// </summary>
        public ICommand OpenFileDialogCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        OpenFileDialog dialog = new OpenFileDialog
                        {
                            Multiselect = false,
                            Filter = "Silverlight Application Package (.xap)|*.xap"
                        };

                        if (dialog.ShowDialog().Value)
                        {
                            FileName = dialog.FileName;
                            ExtractFile(FileName, _destinationDirectory);
                            DocumentContent = ReadConfigurationValue(_destinationDirectory);
                        }
                    });
            }
        }

        /// <summary>
        /// Gets a reference to an ICommand that saves the configuration file
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        SaveContent(_destinationDirectory, FileName, DocumentContent);
                    });
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _destinationDirectory = Path.Combine(Path.GetTempPath(), _directoryId.ToString());

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes the directory we created
        /// </summary>
        public override void Cleanup()
        {
            DirectoryInfo directory = new DirectoryInfo(_destinationDirectory);

            if (directory.Exists)
            {
                directory.Delete(true);
            }

            base.Cleanup();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Extracts the content of the XAP file to the specified location
        /// </summary>
        /// <param name="sourceXap">Source XAP file to extract</param>
        /// <param name="destination">Destination location</param>
        private static void ExtractFile(string sourceXap, string destination)
        {
            using (ZipFile zipFile = new ZipFile(sourceXap))
            {
                zipFile.ExtractAll(destination, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        /// <summary>
        /// Reads the configuration file from the supplied XAP file
        /// </summary>
        /// <param name="path">Root path for where the document is located</param>
        /// <returns>The contents of the XML file</returns>
        private static string ReadConfigurationValue(string path)
        {
            string content;

            using (TextReader stream = new StreamReader(Path.Combine(Path.GetTempPath(), path, "Resources", "Configuration", "DefaultConfiguration.xml")))
            {
                content = stream.ReadToEnd();
            }

            return content;
        }

        /// <summary>
        /// Saves the updated content to the destination XAP file
        /// </summary>
        /// <param name="extractionPath">Path where all the files were extracted to</param>
        /// <param name="destinationFileName">The name of the XAP file to save to</param>
        /// <param name="documentContent">The updated content</param>
        private static void SaveContent(string extractionPath, string destinationFileName, string documentContent)
        {
            string configFile = Path.Combine(extractionPath, "Resources", "Configuration", "DefaultConfiguration.xml");
            string tempZipFile = Path.Combine(extractionPath, "ConfigurationUtility.zip");
            DirectoryInfo directory = new DirectoryInfo(extractionPath);
            List<string> fileNames = new List<string>();

            // Save the updated XML to the configuration file
            using (TextWriter stream = new StreamWriter(configFile))
            {
                stream.WriteLine(documentContent);
            }

            // Make sure we don't have the temporary archive file from a previous run
            if (File.Exists(tempZipFile))
            {
                File.Delete(tempZipFile);
            }

            using (ZipFile zipFile = new ZipFile(tempZipFile))
            {
                foreach (FileInfo file in GetFileNames(directory))
                {
                    string directoryName = file.Directory.FullName.Replace(extractionPath, string.Empty);

                    zipFile.AddFile(file.FullName, directoryName);
                }

                zipFile.Save(tempZipFile);
            }

            File.Copy(tempZipFile, destinationFileName, true);
        }

        /// <summary>
        /// Gets a collection of all files in the specified directory and all subdirectories
        /// </summary>
        /// <param name="path">Root directory to return file names for</param>
        /// <returns>A collection of all files in the specified directory and all subdirectories</returns>
        private static IEnumerable<FileInfo> GetFileNames(DirectoryInfo path)
        {
            List<FileInfo> files = new List<FileInfo>();

            foreach (FileInfo file in path.GetFiles())
            {
                files.Add(file);
            }

            foreach (DirectoryInfo directory in path.GetDirectories())
            {
                files.AddRange(GetFileNames(directory));
            }

            return files;
        }

        #endregion
    }
}