using SwagOverflowWPF.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

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
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Type), typeof(Template));

        /// <summary>
        /// Provides the Option used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public static readonly DependencyProperty OptionProperty = DependencyProperty.Register("Option", typeof(object), typeof(Template));

        /// <summary>
        /// Provides the <see cref="DataTemplate"/> used to render items matching the <see cref="Value"/>
        /// </summary>
        public static readonly DependencyProperty DataTemplateProperty =
           DependencyProperty.Register("DataTemplate", typeof(DataTemplate), typeof(Template));

        /// <summary>
        /// Gets or Sets the value used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public Type Value
        { get { return (Type)GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        /// <summary>
        /// Gets or Sets the Option used to match this <see cref="DataTemplate"/> to an item
        /// </summary>
        public object Option
        { get { return (object)GetValue(OptionProperty); } set { SetValue(OptionProperty, Option); } }

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


    //https://www.codeproject.com/Articles/418250/WPF-Based-Dynamic-DataTemplateSelector
    public class WindowSettingTemplateSelector : DataTemplateSelector
    {
        public TemplateCollection Templates { get; set; }
        ///// <summary>
        ///// Generic attached property specifying <see cref="Template"/>s
        ///// used by the <see cref="DynamicTemplateSelector"/>
        ///// </summary>
        ///// <remarks>
        ///// This attached property will allow you to set the templates you wish to be available whenever
        ///// a control's TemplateSelector is set to an instance of <see cref="DynamicTemplateSelector"/>
        ///// </remarks>
        //public static readonly DependencyProperty TemplatesProperty =
        //    DependencyProperty.RegisterAttached("Templates", typeof(TemplateCollection), typeof(WindowSettingTemplateSelector),
        //    new FrameworkPropertyMetadata(new TemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        ///// <summary>
        ///// Gets the value of the <paramref name="element"/>'s attached <see cref="TemplatesProperty"/>
        ///// </summary>
        ///// <param name="element">The <see cref="UIElement"/> who's attached template's property you wish to retrieve</param>
        ///// <returns>The templates used by the givem <paramref name="element"/>
        ///// when using the <see cref="DynamicTemplateSelector"/></returns>
        //public static TemplateCollection GetTemplates(UIElement element)
        //{
        //    return (TemplateCollection)element.GetValue(TemplatesProperty);
        //}

        ///// <summary>
        ///// Sets the value of the <paramref name="element"/>'s attached <see cref="TemplatesProperty"/>
        ///// </summary>
        ///// <param name="element">The element to set the property on</param>
        ///// <param name="collection">The collection of <see cref="Template"/>s to apply to this element</param>
        //public static void SetTemplates(UIElement element, TemplateCollection collection)
        //{
        //    element.SetValue(TemplatesProperty, collection);
        //}

        /// <summary>
        /// Generic attached property specifying <see cref="Template"/>s
        /// used by the <see cref="DynamicTemplateSelector"/>
        /// </summary>
        /// <remarks>
        /// This attached property will allow you to set the templates you wish to be available whenever
        /// a control's TemplateSelector is set to an instance of <see cref="DynamicTemplateSelector"/>
        /// </remarks>
        public static readonly DependencyProperty CustomTemplatesProperty =
            DependencyProperty.RegisterAttached("CustomTemplates", typeof(TemplateCollection), typeof(WindowSettingTemplateSelector),
            new FrameworkPropertyMetadata(new TemplateCollection(), FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the value of the <paramref name="element"/>'s attached <see cref="CustomTemplatesProperty"/>
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> who's attached template's property you wish to retrieve</param>
        /// <returns>The templates used by the givem <paramref name="element"/>
        /// when using the <see cref="DynamicTemplateSelector"/></returns>
        public static TemplateCollection GetCustomTemplates(UIElement element)
        {
            return (TemplateCollection)element.GetValue(CustomTemplatesProperty);
        }

        /// <summary>
        /// Sets the value of the <paramref name="element"/>'s attached <see cref="CustomTemplatesProperty"/>
        /// </summary>
        /// <param name="element">The element to set the property on</param>
        /// <param name="collection">The collection of <see cref="Template"/>s to apply to this element</param>
        public static void SetCustomTemplates(UIElement element, TemplateCollection collection)
        {
            element.SetValue(CustomTemplatesProperty, collection);
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
            //TemplateCollection templates = GetCustomTemplates(container as UIElement);
            TemplateCollection templates = Templates;
            if (templates == null || templates.Count == 0)
                base.SelectTemplate(item, container);

            if (item == null)
            {
                return null;
            }

            SwagSetting setting = (SwagSetting)item;

            //Then we go through them checking if any of them match our criteria
            foreach (Template template in templates)
            {
                //In this case, we are checking whether the type of the item
                //is the same as the type supported by our DataTemplate
                if (template.Value != null && template.Value.IsInstanceOfType(item))
                    //And if it is, then we return that DataTemplate
                    return template.DataTemplate;

                

                switch (setting.SettingType)
                {
                    case SettingType.DropDown:
                        if (Enum.Equals(template.Option, SettingType.DropDown))
                        {
                            return template.DataTemplate;
                        }
                        break;
                    case SettingType.Normal:
                    default:
                        if (template.Value == setting.ValueType)
                        {
                            return template.DataTemplate;
                        }
                        break;
                    case SettingType.SettingGroup:
                        if (Enum.Equals(template.Option, SettingType.SettingGroup) && setting is SwagSettingGroup)
                        {
                            return template.DataTemplate;
                        }
                        break;
                }
            }
               
            //If all else fails, then we go back to using the default DataTemplate
            return base.SelectTemplate(item, container);
        }
    }
}
