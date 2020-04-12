using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SwagOverflowWPF.Utilities;
using SwagOverflowWPF.ViewModels;

namespace SwagOverflowWPF.UI
{
    /// <summary>
    /// Provides a link between a value and a <see cref="DataTemplate"/>
    /// for the <see cref="DynamicTemplateSelector"/>
    /// </summary>
    /// <remarks>
    /// In this case, our value is a <see cref="System.Type"/> which we are attempting to match
    /// to a <see cref="DataTemplate"/>
    /// </remarks>
    public class Template : DependencyObject
    {
        /// <summary>
        /// Provides the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(Template));

        /// <summary>
        /// Provides the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public static readonly DependencyProperty TypePathProperty = DependencyProperty.Register("TypePath", typeof(String), typeof(Template));

        /// <summary>
        /// Provides the Option used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public static readonly DependencyProperty CompareValueProperty = DependencyProperty.Register("CompareValue", typeof(object), typeof(Template));

        /// <summary>
        /// Provides the <see cref="DataTemplate"/> used to render items matching the <see cref="Value"/>
        /// </summary>
        public static readonly DependencyProperty DataTemplateProperty =
           DependencyProperty.Register("DataTemplate", typeof(DataTemplate), typeof(Template));

        
        /// <summary>
        /// Gets or Sets the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public Type Type
        { get { return (Type)GetValue(TypeProperty); } set { SetValue(TypeProperty, value); } }

        /// <summary>
        /// Gets or Sets the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public String TypePath
        { get { return (String)GetValue(TypePathProperty); } set { SetValue(TypePathProperty, value); } }

        /// <summary>
        /// Gets or Sets the Option used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public object CompareValue
        { get { return (object)GetValue(CompareValueProperty); } set { SetValue(CompareValueProperty, value); } }

        /// <summary>
        /// Gets or Sets the <see cref="DataTemplate"/> used to render items matching the <see cref="Value"/>
        /// </summary>
        public DataTemplate DataTemplate
        { get { return (DataTemplate)GetValue(DataTemplateProperty); } set { SetValue(DataTemplateProperty, value); } }
    }

    /// <summary>
    /// Holds a collection of <see cref="Template"/> items
    /// for application as a control's DataTemplate.
    /// https://stackoverflow.com/questions/28247883/simple-existing-implementation-of-icollectiont
    /// </summary>
    public class TemplateCollection : List<Template>
    {
    }

    public class SwagTemplateSelector : DataTemplateSelector
    {
        public TemplateCollection StaticTemplates { get; set; }
        public String ComparePath { get; set; }

        public static readonly DependencyProperty CustomTemplatesProperty =
            DependencyProperty.RegisterAttached("CustomTemplates", typeof(IEnumerable), typeof(SwagTemplateSelector),
            new FrameworkPropertyMetadata(new TemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public static IEnumerable GetCustomTemplates(UIElement element)
        {
            return (IEnumerable)element.GetValue(CustomTemplatesProperty);
        }

        public static void SetCustomTemplates(UIElement element, TemplateCollection collection)
        {
            element.SetValue(CustomTemplatesProperty, collection);
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(container is UIElement))
                return base.SelectTemplate(item, container);


            TemplateCollection templates = new TemplateCollection();
            IEnumerable customSource = GetCustomTemplates(container as UIElement);
            ICollectionView customViewSource = CollectionViewSource.GetDefaultView(customSource);

            if (customViewSource != null)
            {
                foreach (Template customTemplate in customViewSource)
                {
                    templates.Add(customTemplate);
                }
            }
            
            if (StaticTemplates != null)
            {
                foreach (Template staticTemplate in StaticTemplates)
                {
                    templates.Add(staticTemplate);
                }
            }

            foreach (Template template in templates)
            {
                if (!String.IsNullOrEmpty(ComparePath) && template.CompareValue != null)
                {
                    PropertyInfo propInfo = ReflectionHelper.PropertyInfoCollection[item.GetType()][ComparePath];
                    if (propInfo.CanRead)
                    {
                        Object targetValue = propInfo.GetValue(item);
                        if (targetValue.Equals(template.CompareValue))
                        {
                            return template.DataTemplate;
                        }
                    }
                }

                if (template.Type != null && !String.IsNullOrEmpty(template.TypePath))
                {
                    PropertyInfo propInfo = ReflectionHelper.PropertyInfoCollection[item.GetType()][template.TypePath];
                    if (propInfo.CanRead)
                    {
                        Object targetType = propInfo.GetValue(item);
                        if (targetType.Equals(template.Type))
                        {
                            return template.DataTemplate;
                        }
                    }
                }

                if (template.Type != null && template.Type.IsInstanceOfType(item))
                {
                    return template.DataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
