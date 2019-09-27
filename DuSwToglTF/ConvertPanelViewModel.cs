using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Command;
using SolidWorks.Interop.sldworks;

namespace DuSwToglTF
{
    public class ConvertPanelViewModel:GalaSoft.MvvmLight.ViewModelBase
    {
        #region 私有字段
        private ModelDoc2 swModel;
        private ISldWorks swApp;
        #endregion

        #region 属性
        private string _FileName;
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value;RaisePropertyChanged("FileName"); }
        }
        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value;RaisePropertyChanged("FilePath"); }
        }

        private double _ProgressBarValue;
        public double ProgressBarValue
        {       get { return _ProgressBarValue; }
                set{ _ProgressBarValue = value;
                RaisePropertyChanged("ProgressBarValue");
            }
        }
        private bool _IsInProgress = false;

        public bool IsInProgress { get { return _IsInProgress; }
            set { _IsInProgress = value;RaisePropertyChanged("IsInProgress"); }
        }
        private string _ProgressText;
        public string ProgressText { get { return _ProgressText; }
            set { _ProgressText = value;RaisePropertyChanged("ProgressText"); }
        }
        private bool _IsOpenFile = false;
        public bool IsOpenFile
        {
            get { return _IsOpenFile; }
            set { _IsOpenFile = value;
                RaisePropertyChanged("IsOpenFile");
            }
        }
        private bool _IsOpenFolder = true;
        public bool IsOpenFolder { get { return _IsOpenFolder; }
            set { _IsOpenFolder = value;
                RaisePropertyChanged("IsOpenFolder");
            }
        }
        #endregion

        #region 按钮委托
        private RelayCommand _SaveCommand;
        public RelayCommand SaveCommand {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(SaveClick);
                }
                return _SaveCommand;
            }
            set { _SaveCommand = value; }
        }

        private RelayCommand _ChoosePathCommand;
        public RelayCommand ChoosePathCommand
        {
            get
            {
                if (_ChoosePathCommand != null)
                {
                    _ChoosePathCommand = new RelayCommand(ChoosePathClick);
                }
                return _ChoosePathCommand;
            }
            set { _ChoosePathCommand = value; }
        }
        /// <summary>
        /// 选择路径
        /// </summary>
        private void ChoosePathClick()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (System.IO.Directory.Exists( folderBrowserDialog.SelectedPath))
                {
                    FilePath = folderBrowserDialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 保存按钮执行的动作
        /// </summary>
        private void SaveClick()
        {
            List<string> files = null;
            Controller.Convertor.ErrorType errors = Controller.Convertor.ErrorType.NoErros;
            IsInProgress = true;

            if (!System.IO.Directory.Exists(FilePath))
            {
                swApp.SendMsgToUser("当前路径不存在：" + FilePath);
                return;
            }
            try
            {
                //会堵塞UI；TODO:异步方式实现转换
                var model = Controller.Convertor.DuConvertor.ConvertToglTFModel(swModel, out errors);
                if (model != null)
                {
                    files = Controller.Convertor.DuConvertor.SaveAs(model, FilePath, FileName);
                }
                swApp.SendMsgToUser("保存完成");
                if (files != null && IsOpenFile && files.Count >= 3)
                {
                    System.Diagnostics.Process.Start( files[2]);
                }
                if (IsOpenFolder)
                {
                    System.Diagnostics.Process.Start("explorer.exe", FilePath);
                }
                IsInProgress = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
            // System.Diagnostics.Process.Start("ExpLore", "C:\\window");
        }

        private async void SaveClickAsyns()
        {
            Controller.Convertor.ErrorType errors = Controller.Convertor.ErrorType.NoErros;
            if (!System.IO.Directory.Exists(FilePath))
            {
                swApp.SendMsgToUser("当前路径不存在：" + FilePath);
                return;
            }
            var model = Controller.Convertor.DuConvertor.ConvertToglTFModel(swModel, out errors);
            if (model != null)
            {
                Controller.Convertor.DuConvertor.SaveAs(model, FilePath, FileName);
            }
            swApp.SendMsgToUser("保存完成");
        }

        #endregion
        public ConvertPanelViewModel(ISldWorks swApp1)
        {
            this.swApp = swApp1;
            Controller.Convertor.DuConvertor.ProgressStatus_event += DuConvertor_ProgressStatus_event;
        }
        ~ConvertPanelViewModel()
        {
            Controller.Convertor.DuConvertor.ProgressStatus_event -= DuConvertor_ProgressStatus_event;
        }
        private void DuConvertor_ProgressStatus_event(double Value, string Status, Controller.Convertor.ErrorType error)
        {
            ProgressBarValue = Value;
        }
        #region 公共方法
        public void SetModelDoc()
        {
            swModel = swApp.ActiveDoc;
            if (swModel != null)
            {
                string pathName = swModel.GetPathName();
                if (string.IsNullOrEmpty(pathName.Trim()))
                {
                    FileName = swModel.GetTitle();
                }
                else
                {
                    FileName = System.IO.Path.GetFileNameWithoutExtension(pathName);
                    FilePath = System.IO.Path.GetDirectoryName(pathName);

                }
            }
        }

        #endregion
    }
}
