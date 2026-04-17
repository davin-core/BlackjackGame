using System;
using System.Collections.Generic;
using System.Text;

namespace BlackjackGame.Model
{
    public class HitResult
    {
        public bool IsBusted { get; set; }
        public int PlayerValue { get; set; }
        public int CreditsChanged { get; set; }
        public bool IsGameOver { get; set; }
        public bool AutoStand { get; set; }
    }
}
