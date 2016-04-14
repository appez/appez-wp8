using System.Windows;
using System.Windows.Controls;

namespace appez.utility.uicontrols.contentpicker
{
	public abstract class DataTemplateSelector : ContentControl
	{
        /// <summary>
        /// abstart method for template selection.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
		public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return null;
		}
        
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);

			ContentTemplate = SelectTemplate(newContent, this);
		}
	}
	
}
