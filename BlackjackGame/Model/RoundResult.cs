using System;
using System.Collections.Generic;
using System.Text;

namespace BlackjackGame.Model
{
    public class RoundResult
    {
        public int PlayerValue { get; set; }
        public int DealerValue { get; set; }
        public string Outcome { get; set; }
        public int CreditsChanged { get; set; }
        public bool IsGameOver { get; set; }
    }
}
