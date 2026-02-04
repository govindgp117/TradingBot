# TradingBot (sample)

This is a minimal .NET 8 console trading bot scaffold.

Structure:
- `Services` - exchange adapter and hosted background service
- `Strategies` - sample moving average crossover strategy
- `Models` - small domain models

How to run:

```powershell
# from d:\Projects\TradingBot\src\TradingBot
dotnet run --project .
```

Notes:
- This project uses a mock exchange adapter. For real trading replace `MockExchangeAdapter` with a live adapter and add authentication/security.
- This code is for education and prototyping only. Do not run with real funds without proper testing and safety checks.
