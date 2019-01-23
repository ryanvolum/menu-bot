# food-bank-bot

This bot has been created using [Microsoft Bot Framework][1]. It demonstrates how to create a menu-guided conversation. 

See [wiki][5] for a code and pattern breakdown. 

## Prerequisites
- [Node.js][2]
Ensure [Node.js][2] version 8.5 or higher installed.  To determine if Node.js is installed run the following from a shell window.
```bash
node --version
```

# To run the bot
- Install modules and start the bot.
```bash
npm i && npm start
```
Alternatively you can also run the watch script which will reload the bot when source code changes are detected.
```bash
npm i && npm run watch
```

# Testing the bot using Bot Framework Emulator **v4**
[Microsoft Bot Framework Emulator][3] is a desktop application that allows bot developers to test and debug their bots on localhost or running remotely through a tunnel.

- Install the Bot Framework Emulator version 4.1.0 or greater from [here][4]

## Connect to the bot using Bot Framework Emulator **v4**
- Launch Bot Framework Emulator
- File -> Open Bot Configuration
- Navigate to `food-bot` folder
- Select `food-bot.bot` file

[1]: https://dev.botframework.com
[2]: https://nodejs.org
[3]: https://github.com/microsoft/botframework-emulator
[4]: https://github.com/Microsoft/BotFramework-Emulator/releases
[5]: https://github.com/ryanvolum/menu-bot/wiki
