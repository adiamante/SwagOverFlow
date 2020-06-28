using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Core
{
    public class CoreBuild : BaseBuild
    {
        BooleanContainerExpression _condition = new BooleanContainerExpression();

        #region Condition
        public BooleanContainerExpression Condition
        {
            get { return _condition; }
            set { SetValue(ref _condition, value); }
        }
        #endregion Condition
        #region Type
        public override Type Type { get { return typeof(CoreBuild); } }
        #endregion Type
    }
}
