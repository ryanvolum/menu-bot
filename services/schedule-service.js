const schedule = require('../data/foodBankSchedule.json');
const { Attachment, CardFactory, MessageFactory } = require("botbuilder");

const getValidDays = () => {
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

const filterFoodBanks = (day) => {
    return schedule.filter(foodBank => foodBankIsOpenOnDay(foodBank, day));
}

const foodBankIsOpenOnDay = (foodBank, day) => {
    for (let i = 0; i < foodBank.donationHours.length; i++) {
        if (foodBank.donationHours[i].day === day)
            return true;
    }
}

const createFoodBankCardAttachment = (foodBank) => {
    let allHours = foodBank.donationHours.reduce((prev, curr) => prev += `\r${curr.day} ${curr.hours}`, "");

    let cardText = `**${foodBank.name}**\r${foodBank.donationNotes}\r**Hours**${allHours}`;

    return CardFactory.heroCard(cardText);
}

const createFoodBankCarousel = (foodBanks) => {
    const attachments = foodBanks.map((foodBank) => createFoodBankCardAttachment(foodBank));
    return (MessageFactory.carousel(attachments));
}

module.exports.getValidDays = getValidDays;
module.exports.filterFoodBanks = filterFoodBanks;
module.exports.createFoodBankCarousel = createFoodBankCarousel;