using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeekHunters.Models
{
    public partial class Skill
    {
        public int SkillId { get; set; }

        [Required(ErrorMessage = "Skill name is required.")]
        public string Name { get; set; }

        public bool Filter { get; set; }

        public virtual ICollection<CandidateSkill> CandidateSkill { get; } 
            = new HashSet<CandidateSkill>();
    }
}
