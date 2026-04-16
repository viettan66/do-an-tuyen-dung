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

- This repository includes `render.yaml` which configures a Web Service for Render.com. You can deploy either as a managed .NET Web Service or as a Docker Web Service.

If you want Render to build the .NET project (managed .NET):

1. Set `env: dotnet` in `render.yaml` (or choose `.NET` as the Language in the Render UI).
2. Build Command:

```bash
dotnet publish -c Release -o ./publish
```

3. Start Command:

```bash
dotnet ./publish/JobBoard.dll
```

If you prefer Docker (recommended when the Render UI doesn't show `.NET`), use the included `Dockerfile` and set `env: docker` in `render.yaml` or choose `Docker` in the Render UI. Render will build the image from the Dockerfile.

Local Docker build & run example:

```bash
docker build -t jobboard .
docker run -e PORT=5000 -p 5000:5000 jobboard
```

Render Docker notes:

- Select **Docker** as the environment when creating a new Web Service on Render.
- Ensure `Dockerfile` is in the repository root (this project includes one).
- Render will expose a runtime `PORT` env var; the Dockerfile uses that to bind Kestrel.

Notes on scaling: for multiple instances you should configure a persistent Data Protection key store (Redis or external storage) so authentication cookies are valid across instances.

