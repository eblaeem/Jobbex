using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class JobBenefit
    {
        public int Id { get; set; }

        public int JobId { get; set; }
        public virtual Job Job { get; set; }

        public int BenefitId { get; set; }
        public Benefit Benefit { get; set; }
    }
}
