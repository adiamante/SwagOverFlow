using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SwagOverFlow.WPF.UI
{
    public static class DependencyObjectHelper
    {
        #region Find Parent

        public static T FindParent<T>(this FrameworkElement child) where T : FrameworkElement
        {
            if (child.Parent != null && !(child.Parent is T))
            {
                return (child.Parent as FrameworkElement).FindParent<T>();
            }
            if (child.Parent != null && child.Parent is T)
            {
                return child.Parent as T;
            }
            else
            {
                return null;
            }
        }

        public static T FindParent<T>(this FrameworkElement child, String parentName) where T : FrameworkElement
        {
            if (child.Parent != null && (!(child.Parent is T) || (child.Parent as FrameworkElement).Name != parentName))
            {
                return (child.Parent as FrameworkElement).FindParent<T>(parentName);
            }
            if (child.Parent != null && child.Parent is T && (child.Parent as FrameworkElement).Name == parentName)
            {
                return child.Parent as T;
            }
            else
            {
                return null;
            }
        }

        public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = child.GetParentObject();

            if (child is ContextMenu)
            {
                ContextMenu cm = (ContextMenu)child;
                parentObject = cm.TemplatedParent;
            }

            if (parentObject == null)   //end of tree
            {
                return null;
            }

            T parent = parentObject as T; //match parent with type
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return TryFindParent<T>(parentObject);
            }
        }

        public static T TryFindParent<T>(this DependencyObject child, String parentName) where T : DependencyObject
        {
            DependencyObject parentObject = child.GetParentObject();

            if (parentObject == null)   //end of tree
            {
                return null;
            }

            T parent = parentObject as T; //match parent with type
            if (parent != null && (parent as FrameworkElement).Name == parentName)
            {
                return parent;
            }
            else
            {
                return TryFindParent<T>(parentObject, parentName);
            }
        }

        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null)
            {
                return null;
            }
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                {
                    return parent;
                }
                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //If It's not a ContentElement, rely on VisualTreeHelper or LogicalTreeHelper
            return VisualTreeHelper.GetParent(child) == null ? LogicalTreeHelper.GetParent(child) : VisualTreeHelper.GetParent(child);
        }

        #endregion Find Parent

        #region Find Children

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in child.FindVisualChildren<T>())
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                foreach (object rawChild in LogicalTreeHelper.GetChildren(parent))
                {
                    if (rawChild is DependencyObject)
                    {
                        DependencyObject child = (DependencyObject)rawChild;
                        if (child is T)
                        {
                            yield return (T)child;
                        }

                        foreach (T childOfChild in child.FindLogicalChildren<T>())
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> FindVisualChildrenOneLevel<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                }
            }
        }

        #endregion Find Children

        #region Find Child

        public static T FindVisualChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;

                if (childType == null)
                {
                    foundChild = (child as DependencyObject).FindVisualChild<T>();
                    if (foundChild != null) break;
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static T FindLogicalChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            foreach (Object child in LogicalTreeHelper.GetChildren(parent))
            {
                if (!(child is DependencyObject))
                {
                    continue;
                }

                T childType = child as T;

                if (childType == null)
                {
                    foundChild = (child as DependencyObject).FindLogicalChild<T>();
                    if (foundChild != null) break;
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static T FindVisualChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;

                if (childType == null)
                {
                    foundChild = (child as DependencyObject).FindVisualChild<T>(childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    FrameworkElement frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && (frameworkElement.Name == childName || frameworkElement.Tag.ToString() == childName))
                    {
                        foundChild = (T)child;
                        break;
                    }
                    else
                    {
                        foundChild = (child as DependencyObject).FindVisualChild<T>(childName);
                        if (foundChild != null) break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static T FindLogicalChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            foreach (Object child in LogicalTreeHelper.GetChildren(parent))
            {
                if (!(child is DependencyObject))
                {
                    continue;
                }

                T childType = child as T;

                if (childType == null)
                {
                    foundChild = (child as DependencyObject).FindLogicalChild<T>(childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    FrameworkElement frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && (frameworkElement.Name == childName || frameworkElement.Tag.ToString() == childName))
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        #endregion Find Child

        #region Refresh

        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        #endregion Refresh

        #region TreeView
        public static TreeViewItem ContainerFromItemRecursive(this ItemContainerGenerator root, object item)
        {
            var treeViewItem = root.ContainerFromItem(item) as TreeViewItem;
            if (treeViewItem != null)
                return treeViewItem;
            foreach (var subItem in root.Items)
            {
                treeViewItem = root.ContainerFromItem(subItem) as TreeViewItem;
                var search = treeViewItem?.ItemContainerGenerator.ContainerFromItemRecursive(item);
                if (search != null)
                    return search;
            }
            return null;
        }
        #endregion TreeView
    }
}
