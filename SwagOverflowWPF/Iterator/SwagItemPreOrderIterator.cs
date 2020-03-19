using SwagOverflowWPF.Interface;
using System;
using System.Linq;

namespace SwagOverflowWPF.Iterator
{
    public class SwagItemPreOrderIterator<THeirarchy> : ISwagItemIterator<THeirarchy> where THeirarchy : ISwagHeirarchy<THeirarchy>

    {
        private THeirarchy _root, _current;
        private Boolean _isDone = false, _isSecondToLast = false;

        public SwagItemPreOrderIterator(THeirarchy root)
        {
            _root = root;
        }

        public THeirarchy First()
        {
            _isDone = false;
            _current = _root;
            System.Diagnostics.Debug.WriteLine($"[FIRST] {_current.Display}");
            return _current;
        }

        public THeirarchy Next()
        {
            //If parent node, return first child
            if (_current.Children != null && _current.Children.Count > 0)
            {
                THeirarchy firstChild = _current.Children.OrderBy(c => c.Sequence).First();
                _current = firstChild;
            }
            else if (_current.Parent != null) //If leaf node
            {
                THeirarchy tempCurrent = _current, tempParent = _current.Parent, nextSibling = default(THeirarchy), nextNextSibling = default(THeirarchy);

                do
                {
                    //Find next sibling
                    nextSibling = tempParent.Children.OrderBy(c => c.Sequence).Where(c => c.Sequence > tempCurrent.Sequence).FirstOrDefault();
                    tempCurrent = tempParent;           //currentNode is now the parent
                    tempParent = tempParent.Parent;     //currentParent is now the grandParent
                } while (nextSibling == null && tempCurrent != null && tempParent != null);


                if (nextSibling != null)
                {
                    _current = nextSibling;
                    tempCurrent = _current;
                    tempParent = _current.Parent;

                    if (tempCurrent.Children == null || tempCurrent.Children.Count == 0)
                    {
                        do
                        {
                            //Evaluate if next next Sibling is the last one
                            nextNextSibling = tempParent.Children.OrderBy(c => c.Sequence).Where(c => c.Sequence > tempCurrent.Sequence).FirstOrDefault();
                            tempCurrent = tempParent;           //currentNode is now the parent
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
            else if (_current.Children.Count == 0 && _current.Parent == null)
            {
                //No parent or children
                _isDone = true;
                System.Diagnostics.Debug.WriteLine($"[DONE]");
                return _current;
            }

            if (_isDone)
            {
                System.Diagnostics.Debug.WriteLine($"[DONE]");
                return default(THeirarchy);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[NEXT] {_current.Display}");
            }
            return _current;
        }

        public THeirarchy CurrentItem
        {
            get { return _current; }
        }

        public bool IsDone
        {
            get { return _isDone; }
        }
    }
}
