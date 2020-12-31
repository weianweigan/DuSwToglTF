using DuSwToglTF.ExportContext;
using DuSwToglTF.Extension;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace DuSwToglTF.ViewModel
{
    public class ExportWindowViewModel : ViewModelBase
    {
        #region Field
        private IModelDoc2 _doc;
        private string _saveLocation;
        private RelayCommand _browserLocationCommand;
        private RelayCommand _saveCommand;
        private string _fileName;
        private bool _hasglb = true;
        private bool _hasglTF = false;
        private bool _hasObj = false;
        private ObservableCollection<SwNode> nodes;
        private bool _improvedQuality = true;
        private double progress;

        private BackgroundWorker _worker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
        };
        private bool _enableSave = true;
        private string msg;

        #endregion

        #region Properties
        public ExportWindowViewModel(IModelDoc2 doc)
        {
            this._doc = doc;

            var pathName = doc.GetPathName();

            if (File.Exists(pathName))
            {
                SaveLocation = Path.GetDirectoryName(pathName);
                FileName = Path.GetFileNameWithoutExtension(pathName);
            }

            Nodes = new ObservableCollection<SwNode>() { SwNode.Create(doc) };

            //worker
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.ProgressChanged += _worker_ProgressChanged;
        }

        public string SaveLocation
        {
            get => _saveLocation; set
            {
                Set(ref _saveLocation, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string FileName { get => _fileName; set => Set(ref _fileName, value); }

        public bool Hasglb { get => _hasglb; set => Set(ref _hasglb, value); }

        public bool HasglTF { get => _hasglTF; set => Set(ref _hasglTF, value); }

        public bool HasObj { get => _hasObj; set => Set(ref _hasObj, value); }

        public bool ImprovedQuality { get => _improvedQuality; set => Set(ref _improvedQuality, value); }

        public double Progress { get => progress; set => Set(ref progress, value); }

        public string Msg { get => msg; set => Set(ref msg ,value); }

        public bool EnableSave { get => _enableSave; set => Set(ref _enableSave, value); }

        public ObservableCollection<SwNode> Nodes { get => nodes; set => Set(ref nodes, value); }

        public RelayCommand BrowserLocationCommand { get => _browserLocationCommand ?? (_browserLocationCommand = new RelayCommand(BrowserClick)); set => Set(ref _browserLocationCommand, value); }

        public RelayCommand SaveCommand { get => _saveCommand ?? (_saveCommand = new RelayCommand(SaveClick, CanSaveClick)); set => _saveCommand = value; }
        #endregion

        #region Methods
        private bool CanSaveClick()
        {
            return Directory.Exists(SaveLocation) && !string.IsNullOrEmpty(FileName);
        }

        private class SaveArgument
        {
            public SaveArgument(string filePathName, ExportOptions options, IModelDoc2 doc)
            {
                this.FilePathName = filePathName;
                Options = options;
                Doc = doc;
            }

            public string FilePathName { get; set; }

            public ExportOptions Options { get; set; }

            public IModelDoc2 Doc { get; set; }
        }

        private void SaveClick()
        {
            EnableSave = false;
            _worker.RunWorkerAsync(
                new SaveArgument(Path.Combine(SaveLocation, FileName),
                new ExportOptions() { ImprovedQuality = ImprovedQuality },
                _doc
                ));
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
            Msg = e.UserState as string;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableSave = true;
            if (e.Error != null)
            {
                Msg = $"!!! Error:{e.Error.Message}";
            }
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var argu = (SaveArgument)e.Argument;

            var context = default(glTFExportContext);

            var docType = (swDocumentTypes_e)argu.Doc.GetType();

            if (docType == swDocumentTypes_e.swDocPART)
            {
                context = new PartDocExportContext(argu.FilePathName, argu.Options);
                _worker.ReportProgress(5, "Begin Convert PartDoc...");
            }
            else if (docType == swDocumentTypes_e.swDocASSEMBLY)
            {
                context = new AssemblyDocExportContext(argu.FilePathName, argu.Options);
                _worker.ReportProgress(5, "Begin Convert AssemblyDoc...");
            }

            ExporterUtility.ExportData(argu.Doc, context, (progress, msg) => _worker.ReportProgress(progress, msg), HasObj, HasglTF, Hasglb);
        }

        private void BrowserClick()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveLocation = folderBrowserDialog.SelectedPath;
            }
        }
        #endregion

    }
}
