using System;

namespace projectApiAngular.Services
{
    public class GiftAlreadyAssignedException : Exception
    {
        public string WinnerName { get; }

        public GiftAlreadyAssignedException(string winnerName)
            : base($"Gift already assigned to {winnerName}")
        {
            WinnerName = winnerName;
        }
    }
}
