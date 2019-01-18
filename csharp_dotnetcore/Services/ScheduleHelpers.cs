using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using food_bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace food_bot.Services
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

        public static List<FoodBank> FilterFoodBanksByDonation(string day)
        {
            return ScheduleData.Where(foodBank => 
                foodBank.DonationHours.FirstOrDefault(dh => dh.Day == day) != null).ToList();
        }

        public static IMessageActivity CreateFoodBankDonationCarousel(List<FoodBank> foodbanks)
        {
            var attachments = foodbanks.Select(fb => CreateFoodbankDonationCardAttachment(fb));
            return MessageFactory.Carousel(attachments);
        }

        public static Attachment CreateFoodbankDonationCardAttachment(FoodBank foodBank)
        {
            var allHours = foodBank.DonationHours.Select(dh => $"\r{dh.Day} {dh.Hours}")
                    .Aggregate((string prev, string curr) => prev + ", " + curr);
            var cardText = $"**{foodBank.Name}**\r{foodBank.DonationNotes}\r**Hours** {allHours}";
            return new HeroCard(cardText).ToAttachment();
        }
    }
}
