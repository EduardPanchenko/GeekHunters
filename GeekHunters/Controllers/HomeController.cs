using GeekHunters.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GeekHunters.Controllers
{

    public class HomeController : Controller
    {
        private readonly GeekHunterContext db;

        public HomeController(GeekHunterContext context) => db = context;

        public async Task<IActionResult> Index()
        {
            var cd = await db.Candidate
                .Include(e => e.CandidateSkill)
                .ThenInclude(e => e.Skill)
                .ToListAsync();

            var sk = await db.Skill
                .Where(x => x.Filter)
                .ToListAsync();

            ViewBag.Skils = string.Join(", ", sk.Select(x => x.Name));

            var si = sk.Select(k => k.SkillId).ToArray();
            return View(si.Contains(1) ? cd :
                cd.Where(x => !si.Except(x.CandidateSkill
                .Select(s => s.SkillId)).Any()).ToList());
        }

        public async Task<IActionResult> Create()
        {
            var sk = await db.Skill
                .Where(x => x.SkillId != 1 && x.Filter)
                .ToListAsync();
            ViewBag.Skils = string.Join(", ", sk.Select(x => x.Name));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Candidate candidate)
        {
            if (!ModelState.IsValid)
                return View();

            db.Candidate.Add(candidate);
            var sk = await db.Skill
              .Where(x => x.SkillId != 1 && x.Filter)
              .ToListAsync();

            db.AddRange(sk.Select(x => new CandidateSkill
            {
                Candidate = candidate,
                Skill = x
            }));

            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            Candidate cd;
            if (id == null || (cd = await db.Candidate.FindAsync(id)) == null)
                return NotFound();

            db.Remove(cd);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit() => View(await db.Skill.ToListAsync());

        public async Task<IActionResult> Reverse(int? id)
        {
            //Skill sk;
            //if (id == null || (sk = await db.Skill.FindAsync(id)) == null)
            //    return NotFound();

            //sk.Filter = !sk.Filter;
            //await db.SaveChangesAsync();

            if (id == null) return NotFound();

            var res = await db.Database.ExecuteSqlCommandAsync( //must be 1
                "update Skill set Filter = (Filter + 1) & 1 where SkillId = {0}", id);

            return RedirectToAction("Edit");
        }
    }
}
