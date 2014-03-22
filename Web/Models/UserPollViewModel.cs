using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetaSigmaPhi.Web.Models
{
    public class UserPollViewModel
    {
        public int PollId { get; set; }

        public int UserId { get; set; }

        public int ElectedUserId { get; set; }
        
        public DateTime CreateDate { get; set; }
    }
}