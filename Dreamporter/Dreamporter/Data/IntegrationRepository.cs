using Dreamporter.Core;
using SwagOverFlow.Data.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dreamporter.Data
{
    #region IntegrationRepository
    public interface IIntegrationRepository : IRepository<Integration>
    {

    }

    public class IntegrationEFRepository : SwagEFRepository<DreamporterContext, Integration>, IIntegrationRepository
    {
        public IntegrationEFRepository(DreamporterContext context) : base(context)
        {

        }
    }
    #endregion IntegrationRepository

    #region BuildRepository
    public interface IBuildRepository : IRepository<Build>
    {

    }

    public class BuildEFRepository : SwagEFRepository<DreamporterContext, Build>, IBuildRepository
    {
        public BuildEFRepository(DreamporterContext context) : base(context)
        {

        }
    }
    #endregion BuildRepository
}
