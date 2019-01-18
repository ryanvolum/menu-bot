using System.Collections.Generic;

namespace food_bot.Model
{

    public class FoodBankSchedule
    {
        public List<FoodBank> FoodBanks { get; set; }
    }

    public class FoodBank
    {
        public string Name { get; set; }
        public string DonationNotes { get; set; }
        public string PickupNotes { get; set; }
        public List<DonationHour> DonationHours { get; set; }
        public List<PickupHour> PickupHours { get; set; }
    }

    public class DonationHour
    {
        public string Day { get; set; }
        public string Hours { get; set; }
    }

    public class PickupHour
    {
        public string Day { get; set; }
        public string Hours { get; set; }
    }

}
