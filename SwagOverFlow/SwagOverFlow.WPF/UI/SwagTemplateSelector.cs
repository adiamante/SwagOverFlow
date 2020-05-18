using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SwagOverFlow.Utils;
using SwagOverFlow.WPF.Controls;
using SwagOverFlow.WPF.ViewModels;

namespace SwagOverFlow.WPF.UI
{
    //https://www.codeproject.com/Tips/873562/Markup-Extension-for-Generic-Classes-2
    /// <summary>
    /// Provides a link between a value and a <see cref="DataTemplate"/>
    /// for the <see cref="DynamicTemplateSelector"/>
    /// </summary>
    /// <remarks>
    /// In this case, our value is a <see cref="System.Type"/> which we are attempting to match
    /// to a <see cref="DataTemplate"/>
    /// </remarks>
    public class SwagTemplate : DependencyObject
    {
        /// <summary>
        /// Provides the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(Type), typeof(SwagTemplate));

        /// <summary>
        /// Provides the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public static readonly DependencyProperty TypePathProperty = DependencyProperty.Register("TypePath", typeof(String), typeof(SwagTemplate));

        /// <summary>
        /// Provides the Option used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public static readonly DependencyProperty CompareValueProperty = DependencyProperty.Register("CompareValue", typeof(object), typeof(SwagTemplate));

        /// <summary>
        /// Provides the <see cref="DataTemplate"/> used to render items matching the <see cref="Value"/>
        /// </summary>
        public static readonly DependencyProperty DataTemplateProperty =
           DependencyProperty.Register("DataTemplate", typeof(DataTemplate), typeof(SwagTemplate));
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
    /// Holds a collection of <see cref="SwagTemplate"/> items
    /// for application as a control's DataTemplate.
    /// https://stackoverflow.com/questions/28247883/simple-existing-implementation-of-icollectiont
    /// </summary>
    public class SwagTemplateCollection : List<SwagTemplate>
    {
    }

    public class SwagTemplateSelector : DataTemplateSelector
    {
        public SwagTemplate DefaultTemplate { get; set; }
        public SwagTemplateCollection StaticTemplates { get; set; }
        public String ComparePath { get; set; }

        #region CustomTemplates
        public static readonly DependencyProperty CustomTemplatesProperty =
                    DependencyProperty.RegisterAttached("CustomTemplates", typeof(IEnumerable), typeof(SwagTemplateSelector),
                    new FrameworkPropertyMetadata(new SwagTemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        public static IEnumerable GetCustomTemplates(UIElement element)
        {
            return (IEnumerable)element.GetValue(CustomTemplatesProperty);
        }

        public static void SetCustomTemplates(UIElement element, SwagTemplateCollection collection)
        {
            element.SetValue(CustomTemplatesProperty, collection);
        }
        #endregion CustomTemplates

        #region CustomDefaultTemplate
        public static readonly DependencyProperty CustomDefaultTemplateProperty =
                    DependencyProperty.RegisterAttached("CustomDefaultTemplate", typeof(SwagTemplate), typeof(SwagTemplateSelector));

        public static SwagTemplate GetCustomDefaultTemplate(UIElement element)
        {
            return (SwagTemplate)element.GetValue(CustomDefaultTemplateProperty);
        }

        public static void SetCustomDefaultTemplate(UIElement element, SwagTemplate template)
        {
            element.SetValue(CustomDefaultTemplateProperty, template);
        }
        #endregion CustomDefaultTemplate

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(container is UIElement))
                return base.SelectTemplate(item, container);


            SwagTemplateCollection templates = new SwagTemplateCollection();
            IEnumerable customSource = GetCustomTemplates(container as UIElement);
            ICollectionView customViewSource = CollectionViewSource.GetDefaultView(customSource);

            if (customViewSource != null)
            {
                foreach (SwagTemplate customTemplate in customViewSource)
                {
                    templates.Add(customTemplate);
                }
            }
            
            if (StaticTemplates != null)
            {
                foreach (SwagTemplate staticTemplate in StaticTemplates)
                {
                    templates.Add(staticTemplate);
                }
            }

            foreach (SwagTemplate template in templates)
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
                        if (targetType != null && targetType.Equals(template.Type))
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

            SwagTemplate customDefaultTemplate = GetCustomDefaultTemplate(container as UIElement);

            if (customDefaultTemplate == null && 
                ((container is TreeViewItem) ||
                (container is FrameworkElement && ((FrameworkElement)container).TemplatedParent is TreeViewItem)))
            {
                TreeView treeParent = container.TryFindParent<TreeView>();
                customDefaultTemplate = GetCustomDefaultTemplate(treeParent);
            }

            if (customDefaultTemplate != null)
            {
                return customDefaultTemplate.DataTemplate;
            }

            if (DefaultTemplate != null)
            {
                return DefaultTemplate.DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
