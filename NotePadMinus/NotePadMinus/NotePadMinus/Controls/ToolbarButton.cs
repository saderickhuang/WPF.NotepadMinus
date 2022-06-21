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
   
    public class ToolbarButton : Button
    {

        #region Dependency Properties 

        public static readonly DependencyProperty MouseOverBackgroundProperty
            = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ToolbarButton));

        public static readonly DependencyProperty MouseDownBackgroundProperty
            = DependencyProperty.Register("MouseDownBackground", typeof(Brush), typeof(ToolbarButton));

        public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ToolbarButton), null);

        //图标高度  
        public static readonly DependencyProperty IconHeightProperty
            = DependencyProperty.Register("IconHeight", typeof(double), typeof(ToolbarButton), new PropertyMetadata(15.0, null));

        //图标宽度  
        public static readonly DependencyProperty IconWidthProperty
            = DependencyProperty.Register("IconWidth", typeof(double), typeof(ToolbarButton), new PropertyMetadata(15.0, null));

        #endregion

        #region Property Wrappers
        public Brush MouseOverBackground
        {
            get
            {
                return (Brush)GetValue(MouseOverBackgroundProperty);
            }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public Brush MouseDownBackground
        {
            get
            {
                return (Brush)GetValue(MouseDownBackgroundProperty);
            }
            set { SetValue(MouseDownBackgroundProperty, value); }
        }

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        #endregion
        static ToolbarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarButton), new FrameworkPropertyMetadata(typeof(ToolbarButton)));
        }


    }
}
