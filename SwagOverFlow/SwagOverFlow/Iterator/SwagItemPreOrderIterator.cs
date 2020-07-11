using SwagOverFlow.ViewModels;
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
            System.Diagnostics.Debug.WriteLine($"[FIRST] ({_current.Sequence}) {_current.Display}");
            return _current;
        }

        public TChild Next()
        {
            TChild next = TryGetNext();
            _current = next;
            if (_current == default(TChild))
            {
                _isDone = true;
                System.Diagnostics.Debug.WriteLine($"[DONE]");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[NEXT] ({_current.Sequence}) {_current.Display}");
            }

            return _current;
        }

        private TChild TryGetNext()
        {
            if (_current != default(TChild))
            {
                TChild localCurrent = _current;
                ISwagParent<TChild> composite = localCurrent as ISwagParent<TChild>;

                //If parent node, return first child
                if (composite != null && composite.Children != null && composite.Children.Count > 0)
                {
                    TChild firstChild = composite.Children.OrderBy(c => c.Sequence).First();
                    return firstChild;
                }
                else if (localCurrent.Parent != null) //If leaf node
                {
                    TChild tempCurrent = localCurrent, nextSibling = default(TChild);
                    ISwagParent<TChild> tempParent = localCurrent.Parent;

                    do
                    {
                        //Find next sibling
                        nextSibling = tempParent.Children.OrderBy(c => c.Sequence).Where(c => c.Sequence > tempCurrent.Sequence).FirstOrDefault();
                        tempCurrent = tempParent as TChild;           //currentNode is now the parent
                        tempParent = tempParent.Parent;                      //currenISwagParent<TChild> is now the grandParent
                    } while (nextSibling == null && tempCurrent != null && tempParent != null);

                    return nextSibling;
                }
            }
            
            return  default(TChild);
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