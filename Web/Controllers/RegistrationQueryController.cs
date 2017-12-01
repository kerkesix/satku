
namespace Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KsxEventTracker.Domain.Repositories;
    using QueryModels;
    using System.Threading.Tasks;
    using System.Configuration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize()]
    public class RegistrationQueryController : Controller
    {
        private readonly RegistrationRepository repository ;

        public RegistrationQueryController(RegistrationRepository repository)
        {
            this.repository = repository;
        }

        [Route("api/{happening}/[controller]")]
        public async Task<IActionResult> Get(string happening)
        {
            var data = (await repository.All(happening)).Select(m => 
                new RegistrationWithLinkInfo
                {
                    Id = m.RowKey,
                    Timestamp =  m.Timestamp,
                    BeenThere = m.BeenThere,
                    ConfirmedAt = m.ConfirmedAt,
                    Email = m.Email,
                    Firstname = m.Firstname,
                    Info = m.Info,
                    IsMember = m.IsMember,
                    Lastname = m.Lastname,
                    DisplayName = m.Nickname,
                    Phone = m.Mobile
                }).ToList();

            // Add ids to registrations to avoid creating them at client
            var randomGenerator = new Random();
            foreach (var r in data)
            {
                r.NewPersonId = KsxEventTracker.Domain.Aggregates.Person.Person.CreateRandomEnoughBarcode(randomGenerator);
            }

            // Add linking data
            var alreadyLinked = QueryModelRepository.LinkPeople.Where(m => m.Happenings.ContainsKey(happening));
            foreach (var linked in alreadyLinked)
            {
                // Should never occur, but skipping nulls does no harm in this case
                var p =
                    data.FirstOrDefault(
                        m => m.Id.Equals(linked.Happenings[happening], StringComparison.OrdinalIgnoreCase));

                if (p != null)
                {
                    p.LinkedToPerson = linked.PersonId;
                }
            }

            return Json(data.OrderBy(m => m.Lastname));
        }
    }

    public class RegistrationWithLinkInfo
    {
        public string Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public DateTime? ConfirmedAt { get; set; }  

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string DisplayName { get; set; }

        public bool BeenThere { get; set; }

        public bool IsMember { get; set; }

        public string Info { get; set; }

        public string LinkedToPerson { get; set; }

        public string NewPersonId { get; set; }
    }
}
