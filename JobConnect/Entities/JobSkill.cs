using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class JobSkill
    {
        public int Id { get; set; }

        public int JobId { get; set; }
        public virtual Job Job { get; set; }

        public int SoftwareSkillId { get; set; }
        public virtual SoftwareSkill SoftwareSkill { get; set; }
    }
}
