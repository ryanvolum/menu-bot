const schedule = require('../data/foodBankSchedule.json');
const { CardFactory, MessageFactory } = require("botbuilder");

// ================== DONATION HELPERS =======================
const getValidDonationDays = () => {
    let days = [];
    schedule.forEach(foodBank => {
        foodBank.donationHours.forEach(t => {
            if (!days.includes(t.day)) {
                days.push(t.day);
            }
        })
    })
    return days;
}

const filterFoodBanksByDonation = (day) => {
    return schedule.filter(foodBank => foodBankDonationIsAvailable(foodBank, day));
}

const foodBankDonationIsAvailable = (foodBank, day) => {
    for (let i = 0; i < foodBank.donationHours.length; i++) {
        if (foodBank.donationHours[i].day === day)
            return true;
    }
}

const createFoodBankDonationCardAttachment = (foodBank) => {
    let allHours = foodBank.donationHours.reduce((prev, curr) => prev += `\r${curr.day} ${curr.hours}`, "");

    let cardText = `**${foodBank.name}**\r${foodBank.donationNotes}\r**Hours**${allHours}`;

    return CardFactory.heroCard(cardText);
}

const createFoodBankDonationCarousel = (foodBanks) => {
    const attachments = foodBanks.map((foodBank) => createFoodBankDonationCardAttachment(foodBank));
    return (MessageFactory.carousel(attachments));
}

// ================== PICKUP HELPERS =======================

const getValidPickupDays = () => {
    let days = [];
    schedule.forEach(foodBank => {
        foodBank.pickupHours.forEach(t => {
            if (!days.includes(t.day)) {
                days.push(t.day);
            }
        })
    })
    return days;
}

const filterFoodBanksByPickup = (day) => {
    return schedule.filter(foodBank => foodBankPickupIsAvailable(foodBank, day));
}

const foodBankPickupIsAvailable = (foodBank, day) => {
    for (let i = 0; i < foodBank.pickupHours.length; i++) {
        if (foodBank.pickupHours[i].day === day)
            return true;
    }
}

const createFoodBankPickupCardAttachment = (foodBank) => {
    let allHours = foodBank.pickupHours.reduce((prev, curr) => prev += `\r${curr.day} ${curr.hours}`, "");

    let cardText = `**${foodBank.name}**\r${foodBank.pickupNotes}\r**Hours**${allHours}`;

    return CardFactory.heroCard(cardText);
}

const createFoodBankPickupCarousel = (foodBanks) => {
    const attachments = foodBanks.map((foodBank) => createFoodBankPickupCardAttachment(foodBank));
    return (MessageFactory.carousel(attachments));
}

// ================== CONTACT HELPERS =======================

const getFoodBanks = () => {
    return schedule.map((foodBank) => foodBank.name);
}

// Purely for demonstration - this function does nothing
const sendFoodBankMessage = (foodBankName, message, emailAddress) => {

}

module.exports.filterFoodBanksByDonation = filterFoodBanksByDonation;
module.exports.filterFoodBanksByPickup = filterFoodBanksByPickup;
module.exports.getValidDonationDays = getValidDonationDays;
module.exports.getValidPickupDays = getValidPickupDays;
module.exports.createFoodBankDonationCarousel = createFoodBankDonationCarousel;
module.exports.createFoodBankPickupCarousel = createFoodBankPickupCarousel;
module.exports.getFoodBanks = getFoodBanks;
module.exports.sendFoodBankMessage = sendFoodBankMessage;

