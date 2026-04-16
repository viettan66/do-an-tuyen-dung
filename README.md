JobBoard - Blazor Server (.NET 8) sample

Quick start (local):
1. Ensure .NET 8 SDK is installed.
2. Open a terminal in the project folder (where JobBoard.csproj lives).
3. Run:

```bash
dotnet restore
dotnet run
```

The app will be available at http://localhost:5000 (or port shown).

Credentials (mock):
- admin / password
- user / password

Notes:
- Mock JSON is in `Data/users.json` and `Data/jobs.json`.
- Authentication uses cookie auth (cookie name `JobBoardAuth`) with 2-day expiry.

Render.com deployment:
1. Create a new Web Service on Render ("Web Service").
2. Connect your Git repository (push this project to a repo).
3. Build Command: `dotnet publish -c Release -o ./publish`
4. Start Command: `dotnet JobBoard.dll`
5. Ensure you set environment to `Production` if desired. Render will detect port via `PORT` env variable; default should work for .NET apps.

If Render requires an explicit port binding, modify Program.cs to read `PORT` env variable and set Kestrel accordingly.
Render-specific notes (recommended):

- This repository includes `render.yaml` which configures a Web Service for Render.com (build/start commands and `ASPNETCORE_URLS` env var).
- The app will bind to the runtime `PORT` env var automatically if provided (Program.cs checks `PORT` and calls `UseUrls`).

Render settings to use if not using `render.yaml`:

Build Command:
```
dotnet publish -c Release -o ./publish
```

Start Command:
```
dotnet ./publish/JobBoard.dll
```

Environment variable (set on Render dashboard):
```
ASPNETCORE_URLS = http://0.0.0.0:$PORT
```

Notes on scaling: for multiple instances you should configure a persistent Data Protection key store (Redis or external storage) so authentication cookies are valid across instances.

