using System.Collections.Generic;
using System.IO;
using System.Linq;
using FoodBot.Models;
using Newtonsoft.Json;

namespace FoodBot.Services
{
    public class ScheduleHelpers
    {
        private static List<FoodBank> _scheduleData;

        private static List<FoodBank> ScheduleData
        {
            get
            {
                if (_scheduleData is null)
                {
                    _scheduleData = JsonConvert.DeserializeObject<List<FoodBank>>(File.ReadAllText(@".\Data\foodBankSchedule.json"));
                }
                return _scheduleData;
            }
        }

        public static List<string> GetValidDonationDays()
        {
            var days = new List<string>();

            foreach (var foodBank in ScheduleData)
            {
                foreach (var donationHour in foodBank.DonationHours)
                {
                    if (!days.Contains(donationHour.Day))
                    {
                        days.Add(donationHour.Day);
                    }
                }
            }

            return days;
        }

        public static IList<string> GetValidPickupDays()
        {
            var days = new List<string>();

            foreach (var foodBank in ScheduleData)
            {
                foreach (var pickupHour in foodBank.PickupHours)
                {
                    if (!days.Contains(pickupHour.Day))
                    {
                        days.Add(pickupHour.Day);
                    }
                }
            }

            return days;
        }

        public static List<FoodBank> FilterFoodBanksByDonation(string day)
        {
            return ScheduleData.Where(foodBank =>
                foodBank.DonationHours.FirstOrDefault(dh => dh.Day == day) != null).ToList();
        }

        public static List<FoodBank> FilterFoodBanksByPickup(string day)
        {
            return ScheduleData.Where(foodBank =>
                foodBank.PickupHours.FirstOrDefault(dh => dh.Day == day) != null).ToList();
        }

        public static List<string> GetFoodBanks()
        {
            return ScheduleData.Select((s) => s.Name).ToList();
        }

        public static void SendFoodbankMessage(string v1, string v2, string v3)
        {
            // Simulated...
        }
    }
}
