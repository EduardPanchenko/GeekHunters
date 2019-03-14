using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GeekHunters.Models
{
    public class Candidate
    {
        public int CandidateId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"[A-Z][a-z]{1,20}", ErrorMessage = "First name is incorrect")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"[A-Z][a-z]{1,30}", ErrorMessage = "Last name is incorrect")]
        public string LastName { get; set; }

        public virtual ICollection<CandidateSkill> CandidateSkill { get; set; }
            = new HashSet<CandidateSkill>();

        public virtual string Skills => string.Join(", ",
            CandidateSkill.Select(x => x.Skill.Name));
    }
}
