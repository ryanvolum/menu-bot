using System.Collections.Generic;
using System.Linq;
using FoodBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace FoodBot.Services
{
    public class UXHelpers
    {
        public static IMessageActivity CreateFoodBankDonationCarousel(List<FoodBank> foodbanks)
        {
            var attachments = foodbanks.Select(fb => CreateFoodbankDonationCardAttachment(fb));
            return MessageFactory.Carousel(attachments);
        }

        public static IMessageActivity CreateFoodBankPickupCarousel(List<FoodBank> foodBanks)
        {
            var attachments = foodBanks.Select(fb => CreateFoodbankPickupCardAttachment(fb));
            return MessageFactory.Carousel(attachments);
        }

        private static Attachment CreateFoodbankDonationCardAttachment(FoodBank foodBank)
        {
            var allHours = foodBank.DonationHours.Select(dh => $"\r{dh.Day} {dh.Hours}")
                    .Aggregate((string prev, string curr) => prev + ", " + curr);
            var cardText = $"**{foodBank.Name}**\r{foodBank.DonationNotes}\r**Hours** {allHours}";
            return new HeroCard(cardText).ToAttachment();
        }

        private static Attachment CreateFoodbankPickupCardAttachment(FoodBank foodBank)
        {
            var allHours = foodBank.PickupHours.Select(dh => $"\r{dh.Day} {dh.Hours}")
                    .Aggregate((string prev, string curr) => prev + ", " + curr);
            var cardText = $"**{foodBank.Name}**\r{foodBank.PickupNotes}\r**Hours** {allHours}";
            return new HeroCard(cardText).ToAttachment();
        }
    }
}
