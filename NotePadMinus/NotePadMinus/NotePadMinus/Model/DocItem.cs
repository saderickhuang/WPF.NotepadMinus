using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotePadMinus.Model
{
    public class DocItem : NotifyPropertyChanged
    {
        private TextEditor _textEditor;
        public TextEditor TextEditor
        {
            get { return _textEditor; }
            set 
            {
                if (_textEditor != value)
                {
                    _textEditor = value;
                    OnPropertyChanged();
                }
            }
        }
        private TextDocument _Document;
        public TextDocument Document
        {
            get { return _Document; }
            set
            {
                if (_Document != value)
                {
                    _Document = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool showLineNumbers;

        public bool ShowLineNumbers
        {
            get { return showLineNumbers; }
            set { showLineNumbers = value; OnPropertyChanged(); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { 
                title = value;
                DispTitle = title;
                if (string.IsNullOrEmpty(FilePath) || IsDirty)
                {
                    DispTitle+=(" *");
                }
                OnPropertyChanged(); 
            }
        }

        private string dispTitle;
        public string DispTitle
        {
            get { return dispTitle; }
            set { dispTitle = value; OnPropertyChanged(); }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged(); }
        }


        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                if (_FilePath != value)
                {
                    _FilePath = value;
                    Title = System.IO.Path.GetFileName(_FilePath);
                    OnPropertyChanged();
                }
            }
        }



        #region Highlighting Definition
        private ICommand _HighlightingChangeCommand;
        private IHighlightingDefinition _HighlightingDefinition;

        /// <summary>
        /// Gets a copy of all highlightings.
        /// </summary>
        public ReadOnlyCollection<IHighlightingDefinition> HighlightingDefinitions
        {
            get
            {
                var hlManager = HighlightingManager.Instance;

                if (hlManager != null)
                    return hlManager.HighlightingDefinitions;

                return null;
            }
        }

        /// <summary>
        /// AvalonEdit exposes a Highlighting property that controls whether keywords,
        /// comments and other interesting text parts are colored or highlighted in any
        /// other visual way. This property exposes the highlighting information for the
        /// text file managed in this viewmodel class.
        /// </summary>
        public IHighlightingDefinition HighlightingDefinition
        {
            get
            {
                return _HighlightingDefinition;
            }

            set
            {
                if (_HighlightingDefinition != value)
                {
                    _HighlightingDefinition = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a command that changes the currently selected syntax highlighting in the editor.
        /// </summary>
        public ICommand HighlightingChangeCommand
        {
            get
            {
                if (_HighlightingChangeCommand == null)
                {
                    _HighlightingChangeCommand = new RelayCommand<object>((p) =>
                    {
                        var parames = p as object[];

                        if (parames == null)
                            return;

                        if (parames.Length != 1)
                            return;

                        var param = parames[0] as IHighlightingDefinition;
                        if (param == null)
                            return;

                        HighlightingDefinition = param;
                    });
                }

                return _HighlightingChangeCommand;
            }
        }
        #endregion Highlighting Definition

        private bool _IsDirty;
        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;
                    OnPropertyChanged();
                }
            }
        }
        public DocItem()
        {
            Title = "Unsaved file";
            _textEditor = new TextEditor();
            _Document = TextEditor.Document;
            ShowLineNumbers = true;
        }

        public bool LoadDoc(string paramFilePath)
        {
            if (File.Exists(paramFilePath))
            {
                var hlManager = HighlightingManager.Instance;
                Document = new TextDocument();
                string extension = System.IO.Path.GetExtension(paramFilePath);
                HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);

                using (FileStream fs = new FileStream(paramFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = FileReader.OpenStream(fs, Encoding.UTF8))
                    {
                        Document = new TextDocument(reader.ReadToEnd());
                    }
                }
                FilePath = paramFilePath;

                return true;
            }
            return false;
        }


        public bool SaveDoc()
        {
            if(string.IsNullOrEmpty(FilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = Title;
                if (saveFileDialog.ShowDialog() == true)
                {
                    FilePath = saveFileDialog.FileName;
                    var hlManager = HighlightingManager.Instance;
                    string extension = System.IO.Path.GetExtension(FilePath);
                    HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);
                }
                else
                { 
                    return false;
                }
            }
            using (StreamWriter sw = new StreamWriter(FilePath))
            {
                sw.Write(Document.Text);
            }
            return true;
        }
    }

    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            RaisePropertyChanged(PropertyName);
        }
    }
}
