using BetaSigmaPhi.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetaSigmaPhi.Web.Models
{
    public class PollsWithEligibleUsersViewModel
    {
        public List<PollWithEligibleUsers> avaiablePollsWithEligibleUsers { get; set; } 
    }

    public class PollWithEligibleUsers
    {
        public Poll userPoll { get; set; }

        public List<User> EligibleUsers { get; set; }
    }
}