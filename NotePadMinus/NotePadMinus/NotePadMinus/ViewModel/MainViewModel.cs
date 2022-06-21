using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using NotePadMinus.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotePadMinus.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        #region 属性字段
        #endregion

        /// <summary>
        /// 按钮点击命令
        /// </summary>
        public ICommand Menu_NewFileClick { get; set; }
        public ICommand Menu_OpenFileClick { get; set; }
        public ICommand Menu_CloseFileClick { get; set; }
        public ICommand Menu_SaveFileClick { get; set; }
        public ICommand Menu_SaveAllFileClick { get; set; }

        private ObservableCollection<DocItem> docList;

        public ObservableCollection<DocItem> DocList
        {
            get { return docList; }
            set { docList = value; OnPropertyChanged(); }
        }

        private DocItem selectedItem;

        public DocItem SelectedItem
        {
            get { return selectedItem; }
            set { selectedItem = value; OnPropertyChanged(); }
        }
        public MainViewModel()
        {
            DocList = new ObservableCollection<DocItem>();
            initCommand();

        }
        private void initCommand()
        {
            Menu_NewFileClick = new RelayCommand(DoOpenNewFile);
            Menu_OpenFileClick = new RelayCommand(DoOpenFile);
            Menu_CloseFileClick = new RelayCommand(DoCloseFile);
            Menu_SaveFileClick = new RelayCommand(DoSaveFile);
            Menu_SaveAllFileClick = new RelayCommand(DoSaveAllFile);
        }
        private void DoOpenNewFile()
        {
            DocItem item = new DocItem();
            DocList.Add(item);
            SelectedItem = item;

        }

        private void DoOpenFile()
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                DocItem item = new DocItem();
                item.LoadDoc(dlg.FileName);
                DocList.Add(item);
                SelectedItem = item;
            }
        }

        private void DoCloseFile()
        {
            if(SelectedItem.IsDirty)
            {
                MessageBoxResult ret;
                
                if (!string.IsNullOrEmpty(selectedItem.FilePath))
                {
                    ret = MessageBox.Show($"Save to file：\"{ selectedItem.FilePath}\"?", "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                }
                else
                {
                    ret = MessageBox.Show($"Save file：\"{selectedItem.Title}\"?", "Save", MessageBoxButton.YesNoCancel,MessageBoxImage.Question);
                }
                if(ret == MessageBoxResult.Cancel)
                {
                    return;
                }
                if(ret == MessageBoxResult.Yes)
                {
                    DoSaveFile();
                }
                
            }
            DocItem RemoveItem = SelectedItem;
            DocList.Remove(RemoveItem);
            SelectedItem = DocList.FirstOrDefault();
            return;
        }

        private void DoSaveFile()
        {
            SelectedItem.SaveDoc();
        }

        private void DoSaveAllFile()
        {
            foreach (var item in DocList)
            {
                item.SaveDoc();
            }
        }

    }
}
