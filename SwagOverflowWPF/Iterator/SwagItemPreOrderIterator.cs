using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;
using System;
using System.Linq;

namespace SwagOverflowWPF.Iterator
{
    #region Interfaces
    public interface ISwagItemIterator<TParent, TChild>
    {
        TChild First();
        TChild Next();
        bool IsDone { get; }
        TChild CurrentItem { get; }
    }
    #endregion Interfaces

    public class SwagItemPreOrderIterator<TParent, TChild> : ISwagItemIterator<TParent, TChild>
        where TParent : class, ISwagParent<TParent, TChild>
        where TChild : class, ISwagChild<TParent, TChild>
    {
        private TParent _root;
        private TChild _current;
        private Boolean _isDone = false, _isSecondToLast = false;

        public SwagItemPreOrderIterator(TParent root)
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
            TParent composite = _current as TParent;

            //If parent node, return first child
            if (composite != null && composite.Children != null && composite.Children.Count > 0)
            {
                TChild firstChild = composite.Children.OrderBy(c => c.Sequence).First();
                _current = firstChild;
            }
            else if (_current.Parent != null) //If leaf node
            {
                TChild tempCurrent = _current, nextSibling = default(TChild), nextNextSibling = default(TChild);
                TParent tempParent = _current.Parent;

                do
                {
                    //Find next sibling
                    nextSibling = tempParent.Children.OrderBy(c => c.Sequence).Where(c => c.Sequence > tempCurrent.Sequence).FirstOrDefault();
                    tempCurrent = tempParent as TChild;           //currentNode is now the parent
                    tempParent = tempParent.Parent;                      //currentParent is now the grandParent
                } while (nextSibling == null && tempCurrent != null && tempParent != null);


                if (nextSibling != null)
                {
                    _current = nextSibling;
                    tempCurrent = _current;
                    tempParent = _current.Parent;

                    TParent tempComposite = tempCurrent as TParent;

                    if (tempComposite == null) //leaf node
                    {
                        do
                        {
                            //Evaluate if next next Sibling is the last one
                            nextNextSibling = tempParent.Children.OrderBy(c => c.Sequence).Where(c => c.Sequence > tempCurrent.Sequence).FirstOrDefault();
                            tempCurrent = tempParent as TChild;           //currentNode is now the parent
                            tempParent = tempParent.Parent;     //currentParent is now the grandParent
                        } while (nextNextSibling == null && tempCurrent != null && tempParent != null);

                        if (nextNextSibling == null)
                        {
                            _isSecondToLast = true;
                        }
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