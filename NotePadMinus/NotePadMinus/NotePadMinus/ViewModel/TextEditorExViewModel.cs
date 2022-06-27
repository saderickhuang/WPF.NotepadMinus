using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using NotePadMinus.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace NotePadMinus.ViewModel
{
    

    public class TextEditorExViewModel:ObservableObject
    {
        #region fields
        private string _FilePath;

        private TextDocument _Document;
        private bool _IsDirty;
        private bool _IsReadOnly;
        private string _IsReadOnlyReason = string.Empty;

        private ICommand _HighlightingChangeCommand;
        private IHighlightingDefinition _HighlightingDefinition;
        private int _SynchronizedColumn;
        private int _SynchronizedLine;
        private Encoding _FileEncoding;
        private bool _IsContentLoaded;
        private TextEditorOptions _TextOptions;
        private string _Title;
        private string _DispTitle;
        private GlobalSettings _SettingInstance;
        #endregion fields

        #region properties
        public double SynchronizedFontSize
        {
            get { return _SettingInstance.Setting_FontSize; }
            //set
            //{
            //    if (_SettingInstance.Setting_FontSize != value)
            //    {
            //        _SettingInstance.Setting_FontSize = value;
            //        OnPropertyChanged();
            //    }
            //}
        }

        public FontFamily SynchronizedFontFamily
        {
            get { return _SettingInstance.Setting_FontFamily; }
            //set
            //{
            //    if (_SettingInstance.Setting_FontFamily != value)
            //    {
            //        _SettingInstance.Setting_FontFamily = value;
            //        OnPropertyChanged();
            //    }
            //}
        }

        public bool ShowLineNumbers
        {
            get { return _SettingInstance.Setting_ShowLineNumbers; }
        }

        public TextEditorOptions TextEditorOptionsSetting
        {
            get { return _SettingInstance.Setting_Options; }
        }
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

        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;
                    DispTitle = _IsDirty ? _Title + "*" : _Title;
                    OnPropertyChanged();
                }
            }
        }

        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                if (_FilePath != value)
                {
                    _FilePath = value;
                    Title = string.IsNullOrEmpty(value) ? "New File" : _FilePath;
                    OnPropertyChanged();
                }
            }
        }
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    DispTitle = IsDirty ? _Title + "*" : _Title;
                    OnPropertyChanged();
                }
            }
        }
        
        public string DispTitle
        {
            get { return _DispTitle; }
            set
            {
                if (_DispTitle != value)
                {
                    _DispTitle = value;

                    OnPropertyChanged();
                }
            }
        }
        public bool IsContentLoaded
        {
            get { return _IsContentLoaded; }
            set
            {
                if (_IsContentLoaded != value)
                {
                    _IsContentLoaded = value;

                    OnPropertyChanged();
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            protected set
            {
                if (_IsReadOnly != value)
                {
                    _IsReadOnly = value;
                    OnPropertyChanged();
                }
            }
        }

        public string IsReadOnlyReason
        {
            get
            {
                return _IsReadOnlyReason;
            }

            protected set
            {
                if (_IsReadOnlyReason != value)
                {
                    _IsReadOnlyReason = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Highlighting Definition
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

        #region Synchronized Caret Position
        /// <summary>
        /// Gets/sets the caret positions column from the last time when the
        /// caret position in the left view has been synchronzied with the right view (or vice versa).
        /// </summary>
        public int SynchronizedColumn
        {
            get
            {
                return _SynchronizedColumn;
            }

            set
            {
                if (_SynchronizedColumn != value)
                {
                    _SynchronizedColumn = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets/sets the caret positions line from the last time when the
        /// caret position in the left view has been synchronzied with the right view (or vice versa).
        /// </summary>
        public int SynchronizedLine
        {
            get
            {
                return _SynchronizedLine;
            }

            set
            {
                if (_SynchronizedLine != value)
                {
                    _SynchronizedLine = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion Synchronized Caret Position

        /// <summary>
        /// Get/set file encoding of current text file.
        /// </summary>
        public Encoding FileEncoding
        {
            get { return _FileEncoding; }

            protected set
            {
                if (!Equals(_FileEncoding, value))
                {
                    _FileEncoding = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FileEncodingDescription
        {
            get
            {
                return
                    string.Format("{0}, Header: {1} Body: {2}",
                    _FileEncoding.EncodingName, _FileEncoding.HeaderName, _FileEncoding.BodyName);
            }
        }

        /// <summary>
        /// Get/Set texteditor options frmo <see cref="AvalonEdit"/> editor as <see cref="TextEditorOptions"/> instance.
        /// </summary>
        public TextEditorOptions TextOptions
        {
            get { return _TextOptions; }
        }
        #endregion properties

        public bool LoadDocument(string paramFilePath)
        {
            if (File.Exists(paramFilePath))
            {
                var hlManager = HighlightingManager.Instance;

                Document = new TextDocument();
                string extension = System.IO.Path.GetExtension(paramFilePath);
                HighlightingDefinition = hlManager.GetDefinitionByExtension(extension);

                IsDirty = false;
                IsReadOnly = false;

                // Check file attributes and set to read-only if file attributes indicate that
                if ((System.IO.File.GetAttributes(paramFilePath) & FileAttributes.ReadOnly) != 0)
                {
                    IsReadOnly = true;
                    IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                                       "Change the file access permissions or save the file in a different location if you want to edit it.";
                }

                var fileEncoding = GetEncoding(paramFilePath);

                using (FileStream fs = new FileStream(paramFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = FileReader.OpenStream(fs, fileEncoding))
                    {
                        Document = new TextDocument(reader.ReadToEnd());

                        FileEncoding = reader.CurrentEncoding; // assign encoding after ReadToEnd() so that the StreamReader can autodetect the encoding
                    }
                }

                FilePath = paramFilePath;
                Title = FilePath;
                IsContentLoaded = true;

                return true;
            }

            return false;
        }
        public bool SaveDocument()
        {
            if (string.IsNullOrEmpty(FilePath))
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
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;

            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;

            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE

            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE

            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;

            return Encoding.Default;
        }
        public TextEditorExViewModel()
        {
            Document = new TextDocument();
            _FileEncoding = Encoding.Default;
            Title = "New File";

            _SettingInstance = GlobalSettings.Instance;
        }
    }
}
