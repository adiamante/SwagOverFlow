using Microsoft.EntityFrameworkCore;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Interface;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SwagOverflowWPF.Repository
{
    public class SwagSettingGroupRepository : SwagEFRepository<SwagSettingGroupViewModel>, ISwagSettingGroupRepository
    {
        //Custom query method implementation here
        public SwagSettingGroupRepository(SwagContext context) : base(context)
        {
            
        }
    }
}
