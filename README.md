# Local Distributed Tic-Tac-Toe Engine (Angular / .NET)

This repository provides an enterprise-ready implementation of a full-stack, session-managed Tic-Tac-Toe application. It features a modern, clean Angular presentation interface communicating via synchronous REST protocols with an idempotent state calculation engine written in .NET 8.

## Technology Stack Architecture
*   **Frontend Ecosystem:** Angular v17+, TypeScript, Component-Driven Styling, Reactive Architecture.
*   **Backend Ecosystem:** .NET 8.0 Web API, Domain In-Memory Data Repositories, Strategic Object Heuristics.
*   **API Protocol Model:** Stateless REST JSON Payloads.

---

## Strategic Design & Architectural Decisions

### 1. Absolute State Source of Truth (Backend Sovereignty)
The frontend application maintains no independent knowledge of business rules, validation structures, layout arrays, or victory conditions. Every action (grid selection, undo operations, board clearing transformations) triggers a thin payload transaction to the API service. The server verifies criteria conditions and generates an updated canonical view representation model back down across the runtime layout tree.

### 2. Scoring Finality Ruleset Choice (Option A Selected)
As authorized by specification options, **Option A** has been applied to state processing boundaries. Upon resolution of a definitive terminal layout configuration (Winner verified or Draw recorded), the game history pipeline locks out downstream modifications. The corresponding score addition applies immediately to session storage records. Additional grid selection attempts or step-by-step history revisions are prevented until a clear session generation sequence is called by clicking "Reset Game".

### 3. AI Computer Heuristic Engine Tree (Step-by-Step Priority)
When evaluating the grid layout during single-player operations, the integrated system applies an exact priority model to choose its next move:
1.  **Immediate Victory Validation:** Scans line matrices for immediate "O" victory scenarios.
2.  **Defensive Trajectory Prevention:** Assesses adjacent cells for high-threat "X" groupings and intercepts them.
3.  **Center-Point Domain Anchoring:** Commands cell index [4] immediately if available.
4.  **Corner Traversal:** Claims empty corner indices sequentially `[0, 2, 6, 8]`.
5.  **Linear Extraction Fallback:** Occupies the next remaining open linear cell sequence.

---

## Operational Deployment Roadmap

### How to Run the Backend Locally
1. Ensure you have the **.NET 8.0 SDK** framework installed locally on your system.
2. Move into the backend working folder directory:
   ```bash
   cd Backend
   ```
3. Restore missing packages and run the local server hosting instance:
   ```bash
   dotnet restore
   dotnet run --urls=http://localhost:5000
   ```
4. Verification: The API will be active and listening at `http://localhost:5000/api`.

### How to Run the Frontend Locally
1. Ensure you have the **Node.js LTS** framework environment installed.
2. Navigate into the frontend repository section:
   ```bash
   cd Frontend
   ```
3. Install package dependencies using your package manager terminal client:
   ```bash
   npm install
   ```
4. Launch the local development server:
   ```bash
   npm start
   ```
5. Verification: Access your browser at `http://localhost:4200` to interact with the application.

---

## API Documentation Endpoint Summary

| HTTP Verb | Resource Path | Request Context Function Purpose |
| :--- | :--- | :--- |
| **POST** | `/api/games?mode={0\|1}` | Instantiates a fresh match state entity (0 = Two-player, 1 = AI). |
| **GET** | `/api/games/{id}` | Retrieves the complete session context snapshot payload using a unique ID tracker. |
| **POST** | `/api/games/{id}/moves` | Validates and commits coordinates into the layout matrix array. |
| **POST** | `/api/games/{id}/undo` | Rewinds the state timeline by 1 frame (Two-player) or 2 frames (Vs AI). |
| **POST** | `/api/games/{id}/reset` | Clears layout positions and logs while leaving scoreboard metrics untouched. |
| **GET** | `/api/scoreboard` | Extracts cumulative running context histories for current browser instance records. |
| **POST** | `/api/scoreboard/reset` | Resets wins, losses, and draw metrics safely. |
