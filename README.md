# Local Distributed Tic-Tac-Toe Engine (Angular / .NET)

## 1. Project Overview
This repository contains a full-stack, session-managed Tic-Tac-Toe application. The application provides a responsive browser-based game dashboard connected to a stateless REST API calculation engine running locally. The system operates on a rigorous server-authoritative model, ensuring that game rules, victory conditions, and move histories are processed securely on the backend.

## 2. Tech Stack
*   **Frontend Framework:** Angular (v17+ Standalone Components Architecture)
*   **Frontend Language:** TypeScript
*   **Backend Framework:** .NET 8.0 Web API
*   **API Protocol Style:** RESTful JSON Payloads
*   **Storage Layer:** In-Memory Domain State Repositories
*   **Testing Suite:** xUnit Test Framework Engine

## 3. Features Implemented
*   **Interactive 3x3 Grid:** Locked cells prevent over-writing or double-clicking.
*   **Two Player Mode:** Support for alternating, validated local player moves (X and O).
*   **Basic Computer Opponent (AI):** An intelligent, reactive bot playing as 'O'.
*   **Move Log Panel:** Updates instantly showing move sequences, players, and grid coordinates.
*   **Deterministic Undo:** Instantly rewinds 1 turn in Two-Player mode, or a matched pair (Player + AI) in Computer mode.
*   **Session Scoreboard:** Tracks X wins, O wins, and Draws served natively by the API layer.
*   **Reset Operations:** Independent buttons to clear the active game board or wipe global scoreboard tallies.

---

## 4. How to Run the Backend Locally
1. Ensure you have the **.NET 8.0 SDK** framework installed.
2. Open a terminal and navigate to the project backend root folder:
   ```bash
   cd Backend
   ```
3. Restore packages, build the assemblies, and launch the application:
   ```bash
   dotnet restore
   dotnet run --urls=http://localhost:5000
   ```
4. **Verification:** The API is now active and listening at `http://localhost:5000`.

## 5. How to Run the Frontend Locally
1. Ensure you have **Node.js (LTS)** installed.
2. Open a separate terminal window and navigate to the frontend directory:
   ```bash
   cd Frontend
   ```
3. Install the dependencies and compile the development server:
   ```bash
   npm install
   npm start
   ```
4. **Verification:** Open your web browser and navigate to `http://localhost:4200`.

---

## 6. API Endpoint Summary
All requests interact with the stateless engine via the following contract surface area:

| HTTP Method | Endpoint URI | Payload / Query | Purpose |
| :--- | :--- | :--- | :--- |
| **POST** | `/api/games?mode={0\|1}` | None | Creates a new match session (0=TwoPlayer, 1=Computer). |
| **GET** | `/api/games/{id}` | None | Retrieves the canonical snapshot of the current match state. |
| **POST** | `/api/games/{id}/moves` | `{ player: string, index: int }` | Submits a turn for validation and placement. |
| **POST** | `/api/games/{id}/undo` | None | Rolls back the board history sequence according to the mode. |
| **POST** | `/api/games/{id}/reset` | None | Flushes active boards and move lists but leaves scores intact. |
| **GET** | `/api/scoreboard` | None | Fetches session context win/loss/draw metrics. |
| **POST** | `/api/scoreboard/reset` | None | Wipes out cumulative metrics back to zero. |

---

## 7. How to Run Tests
Core matrix workflows and validation limits are verified via automated xUnit specs.
1. Navigate to the root `Backend` directory containing your solution file:
   ```bash
   cd Backend
   ```
2. Execute the test framework engine:
   ```bash
   dotnet test
   ```

---

## 8. AI Tools and Prompt Summary
*   **AI Assistant Used:** Gemini AI (Advanced Developer Assistant).
*   **Prompting Workflow Strategy:** 
    1. Translated raw markdown functional parameters into strongly typed C# domain services.
    2. Generated responsive frontend layout templates using Angular standalone structures.
    3. Resolved environment configuration loops (such as PowerShell Execution Policies, explicit type bindings, and Zone.js change detection hooks).
*   **Manual Changes Applied:** Refactored async subscription closures in `app.component.ts` to utilize explicit `ChangeDetectorRef` triggers, forcing UI drawing frames to run instantly upon client request resolutions.

## 9. Design Decisions
*   **State Sovereignty:** The frontend is treated as a presentation layer. It possesses zero validation intelligence. This design completely insulates game rules from client-side tempering.
*   **Scoreboard Finality (Option A Applied):** To ensure clean session finality, the "Undo Last Move" trigger locks automatically the moment a game reaches a Win or Draw state. The scoreboard records metrics immediately and remains final for that individual match.
*   **Single-Threaded Concurrent Storage:** Used a concrete `Dictionary<Guid, GameState>` combined with an application-lifetime singleton dependency injection registration to handle local sessions cleanly without configuration overhead.

## 10. Clarifications and Assumptions
*   **Turn Autonomy in Computer Mode:** It is assumed that when playing against the computer, the human player always opens first as `X`. The computer (`O`) responds automatically within the same API invocation transaction, processing a paired return turn state inside a single network lifecycle.
*   **Session Lifecycle Persistence:** Storage scope is active as long as the backend server process remains alive.

## 11. Known Limitations
*   **In-Memory Lifecycle:** Because session state is cached in server RAM, restarting the .NET Web API process clears all active games and wipes the scoreboard history back to zero.
*   **Local Cross-Origin Boundaries:** Requires explicit browser client permissions (`CORS`) configured specifically for port 4200 inside the Web API bootstrapping logic.

## 12. Future Improvements
*   **Persistent Storage Data Provider:** Swap out the in-memory dictionary data store for a lightweight Entity Framework Core SQLite container to keep scores intact across system restarts.
*   **Real-Time Subscriptions:** Implement ASP.NET Core SignalR WebSockets to change the polling layout into a duplex connection event loop, enabling multi-machine peer-to-peer gameplay.
*   **Advanced AI Tree Analysis:** Replace the linear priority rule sequence with a minimax lookahead algorithm tree to support variable computer difficulty settings.
