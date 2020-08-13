using System;
using System.Collections;
using System.Xaml;

namespace SwagOverFlow.WPF.UI
{
    //https://stackoverflow.com/questions/634069/redefine-alias-a-resource-in-wpf
    /// <summary>
    /// Defines an Alias for an existing resource. Very similar to 
    /// <see cref="StaticResourceExtension"/>, but it works in
    ///  ResourceDictionaries
    /// </summary>
    public class StaticResourceAlias : System.Windows.Markup.MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IRootObjectProvider rootObjectProvider = (IRootObjectProvider)
                serviceProvider.GetService(typeof(IRootObjectProvider));
            if (rootObjectProvider == null) return null;
            IDictionary dictionary = rootObjectProvider.RootObject as IDictionary;
            if (dictionary == null) return null;
            return dictionary[ResourceKey];
        }


        public object ResourceKey { get; set; }
    }
}
