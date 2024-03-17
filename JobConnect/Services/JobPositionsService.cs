using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace Services
{
    public interface IJobPositionsService
    {
        Task<List<LabelValue>> Get();
    }
    public class JobPositionsService: IJobPositionsService
    {
        private readonly IUnitOfWork _uow;
        public JobPositionsService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<List<LabelValue>> Get()
        {
            return await _uow.Set<Position>().Select(w => new LabelValue
            {
                Label = w.Name,
                Value = w.Id
            }).ToListAsync();
        }
    }
}
