using ICSharpCode.AvalonEdit;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NotePadMinus.Classes
{
    public sealed class GlobalSettings:ObservableObject
    {
        private static GlobalSettings instance = null;
        private GlobalSettings() 
        {
            Setting_FontSize = 20.0;
            Setting_FontFamily = new FontFamily("Consolas");
            Setting_ShowLineNumbers = true;
            Setting_Options.ShowTabs = true;
            Setting_Options.ShowSpaces = true;
            Setting_Options.ShowEndOfLine = true;
            Setting_Options.ShowColumnRuler = true;
            Setting_Options.ColumnRulerPosition = 15;
        }

        public static GlobalSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GlobalSettings();
                }
                return instance;
            }
        }

        public double       Setting_FontSize = 20.0;
        public FontFamily   Setting_FontFamily = new FontFamily("Consolas");
        public bool         Setting_ShowLineNumbers = true;
        public TextEditorOptions Setting_Options = new TextEditorOptions();

    }
}
