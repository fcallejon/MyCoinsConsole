To run:
```
// portable
dotnet BittrexConsole.dll --apiKey [YOUR-KEY] --apiSecret [YOUR-SECRET] check-wallets 

// self-contained
BittrexConsole --apiKey [YOUR-KEY] --apiSecret [YOUR-SECRET] check-wallets 

BittrexConsole --apiKey [YOUR-KEY] --apiSecret [YOUR-SECRET] check-orders

BittrexConsole --apiKey [YOUR-KEY] --apiSecret [YOUR-SECRET] cancel-order [ORDER-UUID]

BittrexConsole --apiKey [YOUR-KEY] --apiSecret [YOUR-SECRET] buy BTC-XVG all last

BittrexConsole --apiKey [YOUR-KEY] --apiSecret [YOUR-SECRET] sell BTC-XVG all last
```