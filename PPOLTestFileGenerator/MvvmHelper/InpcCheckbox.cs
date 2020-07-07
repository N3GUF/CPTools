using System.Windows;
using System.Windows.Controls;

namespace Comdata.AppSupport.PPOLTestFileGenerator.MvvmHelper
{


    class InpcCheckbox : CheckBox
    {
        static FrameworkPropertyMetadata propertymetadata = new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault);
                                                                                             
        

        public static readonly DependencyProperty IsCheckOnProperty =
                                DependencyProperty.Register("IsCheckOn", typeof(bool), typeof(InpcCheckbox));
        public bool IsCheckOn
        {
            get
            {
                return (bool) this.GetValue(IsCheckOnProperty);
            }
            set
            {
                this.SetValue(IsCheckOnProperty, value);
            }
        }
    }
}
