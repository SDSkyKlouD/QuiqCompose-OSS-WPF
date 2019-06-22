using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Extensions {
    public sealed class ComboBoxItemTemplateSelectorExtension : MarkupExtension {
        public DataTemplate SelectedItemTemplate { get; set; }
        public DataTemplateSelector SelectedItemTemplateSelector { get; set; }
        public DataTemplate DropdownItemTemplate { get; set; }
        public DataTemplateSelector DropdownItemTemplateSelector { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
            => new ComboBoxItemTemplateSelector() {
                SelectedItemTemplate = SelectedItemTemplate,
                SelectedItemTemplateSelector = SelectedItemTemplateSelector,
                DropdownItemTemplate = DropdownItemTemplate,
                DropdownItemTemplateSelector = DropdownItemTemplateSelector
            };
    }

    public sealed class ComboBoxItemTemplateSelector : DataTemplateSelector {
        public DataTemplate SelectedItemTemplate { get; set; }
        public DataTemplateSelector SelectedItemTemplateSelector { get; set; }
        public DataTemplate DropdownItemTemplate { get; set; }
        public DataTemplateSelector DropdownItemTemplateSelector { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            var parent = container;

            while(parent != null && !(parent is ComboBoxItem) && !(parent is ComboBox)) {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var isInDropdown = (parent is ComboBoxItem);

            return isInDropdown
                ? DropdownItemTemplate ?? DropdownItemTemplateSelector?.SelectTemplate(item, container)
                : SelectedItemTemplate ?? SelectedItemTemplateSelector?.SelectTemplate(item, container);
        }
    }
}
