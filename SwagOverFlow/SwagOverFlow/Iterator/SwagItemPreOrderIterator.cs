﻿using SwagOverFlow.ViewModels;
using System;
using System.Linq;

namespace SwagOverFlow.Iterator
{
    #region Interfaces
    public interface ISwagItemIterator<TChild>
    {
        TChild First();
        TChild Next();
        bool IsDone { get; }
        TChild CurrentItem { get; }
    }
    #endregion Interfaces

    public class SwagItemPreOrderIterator<TChild> : ISwagItemIterator<TChild> where TChild : class, ISwagChild<TChild>
    {
        private ISwagParent<TChild> _root;
        private TChild _current;
        private Boolean _isDone = false, _isSecondToLast = false;

        public SwagItemPreOrderIterator(ISwagParent<TChild> root)
        {
            _root = root;
        }

        public TChild First()
        {
            _isDone = false;
            _current = _root as TChild;
            System.Diagnostics.Debug.WriteLine($"[FIRST] {_current.Display}");
            return _current;
        }

        public TChild Next()
        {
            ISwagParent<TChild> composite = _current as ISwagParent<TChild>;

            //If parent node, return first child
            if (composite != null && composite.Children != null && composite.Children.Count > 0)
            {
                TChild firstChild = composite.Children.OrderBy(c => c.Sequence).First();
                _current = firstChild;
            }
            else if (_current.Parent != null) //If leaf node
            {
                TChild tempCurrent = _current, nextSibling = default(TChild), nextNextSibling = default(TChild);
                ISwagParent<TChild> tempParent = _current.Parent;

                do
                {
                    //Find next sibling
                    nextSibling = tempParent.Children.OrderBy(c => c.Sequence).Where(c => c.Sequence > tempCurrent.Sequence).FirstOrDefault();
                    tempCurrent = tempParent as TChild;           //currentNode is now the parent
                    tempParent = tempParent.Parent;                      //currenISwagParent<TChild> is now the grandParent
                } while (nextSibling == null && tempCurrent != null && tempParent != null);


                if (nextSibling != null)
                {
                    _current = nextSibling;
                    tempCurrent = _current;
                    tempParent = _current.Parent;

                    do
                    {
                        //Evaluate if next next Sibling is the last one
                        nextNextSibling = tempParent.Children.OrderBy(c => c.Sequence).Where(c => c.Sequence > tempCurrent.Sequence).FirstOrDefault();
                        tempCurrent = tempParent as TChild;           //currentNode is now the parent
                        tempParent = tempParent.Parent;     //currenISwagParent<TChild> is now the grandParent
                    } while (nextNextSibling == null && tempCurrent != null && tempParent != null);

                    if (nextNextSibling == null)
                    {
                        _isSecondToLast = true;
                    }
                }
                else if (_isSecondToLast == true)
                {
                    _isDone = true;
                }
            }
            //leaf node or (no parent and no children)
            else if (composite == null || (_current.Parent == null && composite.Children.Count == 0))
            {
                _isDone = true;
                System.Diagnostics.Debug.WriteLine($"[DONE]");
                return _current;
            }

            if (_isDone)
            {
                System.Diagnostics.Debug.WriteLine($"[DONE]");
                return default(TChild);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[NEXT] {_current.Display}");
            }
            return _current;
        }

        public TChild CurrentItem
        {
            get { return _current; }
        }

        public bool IsDone
        {
            get { return _isDone; }
        }
    }
}