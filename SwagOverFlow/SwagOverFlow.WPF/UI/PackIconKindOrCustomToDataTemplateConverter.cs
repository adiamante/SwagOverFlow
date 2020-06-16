using MahApps.Metro.IconPacks;
using MahApps.Metro.IconPacks.Converter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace SwagOverFlow.WPF.UI
{
    public class PackIconKindOrCustomToDataTemplateConverter : MarkupConverter
    {
        /// <summary>
        /// Gets or sets the brush to draw the icon.
        /// </summary>
        public Brush Brush { get; set; } = Brushes.Black;

        /// <summary>
        /// Gets or sets the flip orientation for the icon.
        /// </summary>
        public PackIconFlipOrientation Flip { get; set; } = PackIconFlipOrientation.Normal;

        /// <summary>
        /// Gets or sets the rotation (angle) for the icon.
        /// </summary>
        public double RotationAngle { get; set; } = 0d;

        /// <summary>
        /// Gets or sets the brush to draw the icon.
        /// </summary>
        public String DynamicResourceBrush { get; set; } = "MahApps.Brushes.AccentBase";

        public Boolean UseForegroundBrush { get; set; } = false;

        /// <summary>
        /// Gets the <see cref="T:System.Windows.Media.TransformGroup" /> for the <see cref="T:System.Windows.Media.DrawingGroup" />.
        /// </summary>
        /// <param name="iconKind">The icon kind to draw.</param>
        protected Transform GetTransformGroup(object iconKind)
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(GetScaleTransform(iconKind)); // scale
            transformGroup.Children.Add(new ScaleTransform(
                this.Flip == PackIconFlipOrientation.Horizontal || this.Flip == PackIconFlipOrientation.Both ? -1 : 1,
                this.Flip == PackIconFlipOrientation.Vertical || this.Flip == PackIconFlipOrientation.Both ? -1 : 1
            )); // flip
            transformGroup.Children.Add(new RotateTransform(this.RotationAngle)); // rotate

            return transformGroup;
        }

        /// <summary>
        /// Gets the ImageSource for the given kind.
        /// </summary>
        //protected ImageSource CreateImageSource(object iconKind, Brush foregroundBrush)
        protected DataTemplate CreateImageSource(object iconKind, Brush foregroundBrush)
        {
            string path = this.GetPathData(iconKind);

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            //var drawingImage = new DrawingImage(GetDrawingGroup(iconKind, foregroundBrush, path));
            //drawingImage.Freeze();
            //return drawingImage;

            Decimal baseWidth = 20m, baseHeight = 20m, centerX = 0.0m, centerY = 0.0m, scaleX = 1.0m, scaleY = 1.0m;
            switch (iconKind)
            {
                case PackIconBoxIconsKind boxKind:
                    baseWidth = 600m;
                    baseHeight = 600m;
                    scaleX = 0.7m;
                    scaleY = -0.7m;
                    break;
                default:
                case PackIconEntypoKind entypoKind:
                    baseWidth = 20m;
                    baseHeight = 20m;
                    break;
                case PackIconFontAwesomeKind fontAwesomeKind:
                    baseWidth = 450m;
                    baseHeight = 450m;
                    break;
                case PackIconIoniconsKind ioniconsKind:
                    baseWidth = 350m;
                    baseHeight = 350m;
                    scaleX = 0.7m;
                    scaleY = 0.7m;
                    break;
                case PackIconMaterialKind materialKind:
                    baseWidth = 25m;
                    baseHeight = 25m;
                    break;
                case PackIconMaterialDesignKind materialDesignKind:
                    baseWidth = 500m;
                    baseHeight = 500m;
                    scaleY = -1.0m;
                    break;
                case PackIconModernKind modernKind:
                    baseWidth = 65m;
                    baseHeight = 65m;
                    scaleX = 1.3m;
                    scaleY = 1.3m;
                    break;
                case PackIconPicolIconsKind picolKind:
                    baseWidth = 1000m;
                    baseHeight = 1000m;
                    break;
                case PackIconRPGAwesomeKind rpgAwesomeKind:
                    baseWidth = 600m;
                    baseHeight = 600m;
                    scaleX = 0.7m;
                    scaleY = -0.7m;
                    break;
                case PackIconWeatherIconsKind weatherKind:
                    baseWidth = 30m;
                    baseHeight = 30m;
                    scaleX = 1.3m;
                    scaleY = 1.3m;
                    break;
            }

            if (centerX == 0)
            {
                centerX = baseWidth / 2.0m;
            }

            if (centerY == 0)
            {
                centerY = baseHeight / 2.0m;
            }

            String fill = UseForegroundBrush ?
                $"{{Binding RelativeSource={{RelativeSource AncestorType={{x:Type FrameworkElement}}}}, Path=(TextElement.Foreground)}}" :
                $"{{DynamicResource {DynamicResourceBrush}}}";

            StringReader stringReader = new StringReader("Nothing");    //should fail if this hits custom
            switch (iconKind)
            {
                case PackIconCustomKind customKind:
                    if (!UseForegroundBrush)
                    {
                        path = path.Replace($"{{Binding RelativeSource={{RelativeSource AncestorType={{x:Type FrameworkElement}}}}, Path=(TextElement.Foreground)}}", fill);
                    }
                    stringReader = new StringReader(path);
                    break;
                default:
                    stringReader = new StringReader(
                        $@"<DataTemplate 
                            xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""> 
                                <Viewbox xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" Stretch=""Uniform"">
                                    <Canvas Width=""{baseWidth}"" Height=""{baseHeight}"">
                                        <Path Fill=""{fill}"">
                                            <Path.RenderTransform>
                                                <ScaleTransform CenterX=""{centerX}"" CenterY=""{centerY}"" ScaleX=""{scaleX}"" ScaleY=""{scaleY}"" />
                                            </Path.RenderTransform>
                                            <Path.Data>
                                                <PathGeometry Figures=""{path}"" />
                                            </Path.Data>
                                        </Path>
                                    </Canvas>
                                </Viewbox>
                            </DataTemplate>");
                    break;
            }
            
            XmlReader xmlReader = XmlReader.Create(stringReader);
            DataTemplate dataTemplate = XamlReader.Load(xmlReader) as DataTemplate;

            return dataTemplate;
        }

        /// <inheritdoc />
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Enum))
            {
                return DependencyProperty.UnsetValue;
            }

            var imageSource = CreateImageSource(value, parameter as Brush ?? this.Brush ?? Brushes.Black);
            return imageSource ?? DependencyProperty.UnsetValue;
        }

        /// <inheritdoc />
        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        /// <inheritdoc />
        protected string GetPathData(object iconKind)
        {
            string data = null;
            switch (iconKind)
            {
                case PackIconBoxIconsKind boxIconsKind:
                    PackIconBoxIconsDataFactory.DataIndex.Value?.TryGetValue(boxIconsKind, out data);
                    return data;
                case PackIconEntypoKind entypoKind:
                    PackIconEntypoDataFactory.DataIndex.Value?.TryGetValue(entypoKind, out data);
                    return data;
                case PackIconEvaIconsKind evaIconsKind:
                    PackIconEvaIconsDataFactory.DataIndex.Value?.TryGetValue(evaIconsKind, out data);
                    return data;
                case PackIconFeatherIconsKind featherIconsKind:
                    PackIconFeatherIconsDataFactory.DataIndex.Value?.TryGetValue(featherIconsKind, out data);
                    return data;
                case PackIconFontAwesomeKind fontAwesomeKind:
                    PackIconFontAwesomeDataFactory.DataIndex.Value?.TryGetValue(fontAwesomeKind, out data);
                    return data;
                case PackIconIoniconsKind ioniconsKind:
                    PackIconIoniconsDataFactory.DataIndex.Value?.TryGetValue(ioniconsKind, out data);
                    return data;
                case PackIconJamIconsKind jamIconsKind:
                    PackIconJamIconsDataFactory.DataIndex.Value?.TryGetValue(jamIconsKind, out data);
                    return data;
                case PackIconMaterialDesignKind materialDesignKind:
                    PackIconMaterialDesignDataFactory.DataIndex.Value?.TryGetValue(materialDesignKind, out data);
                    return data;
                case PackIconMaterialKind materialKind:
                    PackIconMaterialDataFactory.DataIndex.Value?.TryGetValue(materialKind, out data);
                    return data;
                case PackIconMaterialLightKind materialLightKind:
                    PackIconMaterialLightDataFactory.DataIndex.Value?.TryGetValue(materialLightKind, out data);
                    return data;
                case PackIconMicronsKind micronsKind:
                    PackIconMicronsDataFactory.DataIndex.Value?.TryGetValue(micronsKind, out data);
                    return data;
                case PackIconModernKind modernKind:
                    PackIconModernDataFactory.DataIndex.Value?.TryGetValue(modernKind, out data);
                    return data;
                case PackIconOcticonsKind octiconsKind:
                    PackIconOcticonsDataFactory.DataIndex.Value?.TryGetValue(octiconsKind, out data);
                    return data;
                case PackIconPicolIconsKind picolIconsKind:
                    PackIconPicolIconsDataFactory.DataIndex.Value?.TryGetValue(picolIconsKind, out data);
                    return data;
                case PackIconRPGAwesomeKind rpgAwesomeKind:
                    PackIconRPGAwesomeDataFactory.DataIndex.Value?.TryGetValue(rpgAwesomeKind, out data);
                    return data;
                case PackIconSimpleIconsKind simpleIconsKind:
                    PackIconSimpleIconsDataFactory.DataIndex.Value?.TryGetValue(simpleIconsKind, out data);
                    return data;
                case PackIconTypiconsKind typiconsKind:
                    PackIconTypiconsDataFactory.DataIndex.Value?.TryGetValue(typiconsKind, out data);
                    return data;
                case PackIconUniconsKind uniconsKind:
                    PackIconUniconsDataFactory.DataIndex.Value?.TryGetValue(uniconsKind, out data);
                    return data;
                case PackIconWeatherIconsKind weatherIconsKind:
                    PackIconWeatherIconsDataFactory.DataIndex.Value?.TryGetValue(weatherIconsKind, out data);
                    return data;
                case PackIconZondiconsKind zondiconsKind:
                    PackIconZondiconsDataFactory.DataIndex.Value?.TryGetValue(zondiconsKind, out data);
                    return data;
                case PackIconCustomKind customKind:
                    PackIconCustomFactory.DataIndex.Value?.TryGetValue(customKind, out data);
                    return data;
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        protected ScaleTransform GetScaleTransform(object iconKind)
        {
            return
                ((iconKind is PackIconBoxIconsKind)
                 || (iconKind is PackIconEvaIconsKind)
                 || (iconKind is PackIconJamIconsKind)
                 || (iconKind is PackIconMaterialDesignKind)
                 || (iconKind is PackIconRPGAwesomeKind)
                 || (iconKind is PackIconTypiconsKind))
                    ? new ScaleTransform(1, -1)
                    : new ScaleTransform(1, 1);
        }

        /// <inheritdoc />
        protected DrawingGroup GetDrawingGroup(object iconKind, Brush foregroundBrush, string path)
        {
            var geometryDrawing = new GeometryDrawing
            {
                Geometry = Geometry.Parse(path)
            };

            if (iconKind is PackIconFeatherIconsKind)
            {
                var pen = new Pen(foregroundBrush, 2d)
                {
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round,
                    LineJoin = PenLineJoin.Round,
                };
                geometryDrawing.Pen = pen;
            }
            else
            {
                geometryDrawing.Brush = foregroundBrush;
            }

            var drawingGroup = new DrawingGroup
            {
                Children = { geometryDrawing },
                Transform = this.GetTransformGroup(iconKind)
            };

            return drawingGroup;
        }
    }
}
