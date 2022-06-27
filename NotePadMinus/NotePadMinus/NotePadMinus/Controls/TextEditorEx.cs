using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NotePadMinus.Controls
{
    using ICSharpCode.AvalonEdit;
    using System;
    using System.Windows;
    public class TextEditorEx : TextEditor
    {
        #region fields
        #region CaretPosition
        private static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register("Column", typeof(int),
                typeof(TextEditorEx), new UIPropertyMetadata(1));

        private static readonly DependencyProperty LineProperty =
            DependencyProperty.Register("Line", typeof(int),
                typeof(TextEditorEx), new UIPropertyMetadata(1));
        #endregion CaretPosition

        #region FontSetting
        private static readonly DependencyProperty FontSizeDp = 
            DependencyProperty.Register("FontSize", typeof(double),
                typeof(TextEditorEx), new UIPropertyMetadata((double)1));

        private static readonly DependencyProperty FontFamilyDp =
            DependencyProperty.Register("FontFamily", typeof(FontFamily),
                typeof(TextEditorEx), new UIPropertyMetadata(new FontFamily("Consolas")));


        #endregion
        #endregion fields

        #region properties

        public int Column
        {
            get
            {
                return (int)GetValue(ColumnProperty);
            }

            set
            {
                SetValue(ColumnProperty, value);
            }
        }

        /// <summary>
        /// Get/set the current line of the editor caret.
        /// </summary>
        public int Line
        {
            get
            {
                return (int)GetValue(LineProperty);
            }

            set
            {
                SetValue(LineProperty, value);
               
            }
        }


        #endregion

        #region methods
        private void TextEdit_Loaded(object sender, RoutedEventArgs e)
        {
            this.TextArea.Caret.PositionChanged += Caret_PositionChanged;
        }

        private void TextEdit_Unloaded(object sender, RoutedEventArgs e)
        {
            this.TextArea.Caret.PositionChanged -= Caret_PositionChanged;
        }

        /// <summary>
        /// Update Column and Line position properties when caret position is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            // this.TextArea.TextView.InvalidateLayer(KnownLayer.Background); //Update current line highlighting

            if (this.TextArea != null)
            {
                this.Column = this.TextArea.Caret.Column;
                this.Line = this.TextArea.Caret.Line;
            }
            else
            {
                this.Column = 0;
                this.Line = 0;
                
            }
        }
        #endregion methods

        static TextEditorEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEditorEx), new FrameworkPropertyMetadata(typeof(TextEditorEx)));
        }

        public TextEditorEx()
        {
            this.Loaded += TextEdit_Loaded;
            this.Unloaded += TextEdit_Unloaded;
            

        }


    }
}
