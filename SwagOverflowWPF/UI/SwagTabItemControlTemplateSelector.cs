using SwagOverflowWPF.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SwagOverflowWPF.UI
{
    public class SwagTabItemControlTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CompositeTemplate { get; set; }

        /// <summary>
        /// Generic attached property specifying <see cref="Template"/>s
        /// used by the <see cref="DynamicTemplateSelector"/>
        /// </summary>
        /// <remarks>
        /// This attached property will allow you to set the templates you wish to be available whenever
        /// a control's TemplateSelector is set to an instance of <see cref="DynamicTemplateSelector"/>
        /// </remarks>
        public static readonly DependencyProperty TemplatesProperty =
            DependencyProperty.RegisterAttached("Templates", typeof(IEnumerable), typeof(SwagTabItemControlTemplateSelector),
            new FrameworkPropertyMetadata(new TemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the value of the <paramref name="element"/>'s attached <see cref="TemplatesProperty"/>
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> who's attached template's property you wish to retrieve</param>
        /// <returns>The templates used by the givem <paramref name="element"/>
        /// when using the <see cref="DynamicTemplateSelector"/></returns>
        public static IEnumerable GetTemplates(UIElement element)
        {
            return (IEnumerable)element.GetValue(TemplatesProperty);
        }

        /// <summary>
        /// Sets the value of the <paramref name="element"/>'s attached <see cref="TemplatesProperty"/>
        /// </summary>
        /// <param name="element">The element to set the property on</param>
        /// <param name="collection">The collection of <see cref="Template"/>s to apply to this element</param>
        public static void SetTemplates(UIElement element, TemplateCollection collection)
        {
            element.SetValue(TemplatesProperty, collection);
        }

        /// <summary>
        /// Overriden base method to allow the selection of the correct DataTemplate
        /// </summary>
        /// <param name="item">The item for which the template should be retrieved</param>
        /// <param name="container">The object containing the current item</param>
        /// <returns>The <see cref="DataTemplate"/> to use when rendering the <paramref name="item"/></returns>
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            //This should ensure that the item we are getting is in fact capable of holding our property
            //before we attempt to retrieve it.
            if (!(container is UIElement))
                return base.SelectTemplate(item, container);

            //First, we gather all the templates associated with the current control through our dependency property
            IEnumerable source = GetTemplates(container as UIElement);
            if (source == null)
                base.SelectTemplate(item, container);

            TemplateCollection templates = new TemplateCollection();
            ICollectionView collectionViewSource = CollectionViewSource.GetDefaultView(source);
            foreach (Template template in collectionViewSource)
            {
                templates.Add(template);
            }

            if (item is SwagTabItem)
            {
                SwagTabItem swagTabItem = item as SwagTabItem;

                if (swagTabItem is SwagTabCollection)
                {
                    return CompositeTemplate;
                }

                //Then we go through them checking if any of them match our criteria
                foreach (Template template in templates)
                {
                    if (template.Option.ToString() == swagTabItem.Path)
                    {
                        return template.DataTemplate;
                    }
                }
            }

            //If all else fails, then we go back to using the default DataTemplate
            return base.SelectTemplate(item, container);
        }
    }
}
