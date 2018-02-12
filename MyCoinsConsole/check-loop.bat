@ echo off
for /l %%x in (1, 1, 100) do (
	echo Check# %%x
   	@ dotnet BittrexConsole.dll check-wallets --apiKey %1 --apiSecret %2
	@ waitfor nothingAtAll /T 60
)