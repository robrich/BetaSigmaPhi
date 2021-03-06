﻿using System;
using System.Linq;

namespace BetaSigmaPhi.Web.Models
{
    public class PollModel
    {
        public int PollId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int FrequencyId { get; set; }

        public string Frequency { get; set; }

        public int CategoryId { get; set; }

        public string Category { get; set; }

        public int VoteCountPerFrequency { get; set; }

    }

    public class VoteModel
    {
        public int VoteId { get; set; }

        public int PollId { get; set; }

        public DateTime VoteDate { get; set; }

        public int VoterUserId { get; set; }
        
        public string VoterUser { get; set; }

        public int ElectedUserId { get; set; }
        
        public string ElectedUser { get; set; }
    }

}

