# AI Development Workflow & Prompt Summary

This document provides a comprehensive log of the AI-assisted engineering workflow, prompt progression, and manual architectural modifications applied to satisfy the requirements of the Tic-Tac-Toe application assessment.

---

## 1. Prompt Sequencing & Core Evolution

### Phase 1: Architectural Scaffolding & State Sovereignty
*   **Engineering Objective:** Convert raw markdown requirements into a decoupled, server-authoritative distributed system layout.
*   **The Prompt Implemented:**
    > *"Review this Tic-Tac-Toe coding assignment document. I need to build a backend-authoritative REST API in .NET 8 and an Angular frontend. Generate a clean domain service for the game rules, state management, and the computer's defensive/offensive priority move tree. Choose Option A for scoreboard finality."*
*   **AI Deliverables:** Initial blueprint for `GameService.cs` incorporating explicit domain structures (`GameStatus`, `GameMode`), standard multidimensional grid abstractions flattened into a singular 9-element string array, and a case-insensitive RESTful routing surface.

### Phase 2: Resolving Environment & Framework Blockades
*   **Engineering Objective:** Initialize empty template footprints from scratch and unblock restricted environment configurations.
*   **The Prompts Implemented:**
    > *"I am getting an error: `npm.ps1 cannot be loaded because running scripts is disabled on this system.` How do I unblock my PowerShell terminal?"*
    >
    > *"The backend build failed with `error CS0246: The type or namespace name 'GETAttribute' could not be found`. Fix this casing error."*
*   **AI Deliverables:** Generation of `Set-ExecutionPolicy` localized scoping scopes to safely unlock Windows script-execution blocks. Refactored compiled route metadata abstractions from pure capitalized `[GET]` tokens to framework-compliant `[HttpGet]` attributes inside the ASP.NET pipeline.

### Phase 3: Presentation Syncing & Event Loop Adjustments
*   **Engineering Objective:** Eradicate a rendering lag where grid tiles required two distinct clicks to visually draw an updated state.
*   **The Prompt Implemented:**
    > *"The Angular grid UI is up and running now, but something is wrong. I have to click twice on a cell for the option to render visually on the screen. The backend logs show the data updates on click 1, but the screen stays empty until click 2."*
*   **AI Deliverables & Engineering Pivot:** The tool initially recommended toggling event coalescing flags. When that failed to sync asynchronous network threads with the visual rendering frames, a manual design override was implemented. We injected `ChangeDetectorRef` directly into the template tree controller to forcefully run microtask re-evaluations the millisecond HTTP callbacks returned JSON state payloads.

### Phase 4: Full-Spectrum Test Suite Expansion
*   **Engineering Objective:** Bring testing metrics to 100% completion across all 13 checklist points while debugging internal reference cross-talk.
*   **The Prompt Implemented:**
    > *"My xUnit project is failing with `error CS0234: The type or namespace name 'Models' does not exist in the namespace 'TicTacToe.Backend'`. Fix the assembly references and expand the tests to cover all minimum assignment criteria explicitly (valid moves, invalid moves, turn switches, column wins, diagonal wins, draws, resets, undo behaviors, and AI blocks)."*
*   **AI Deliverables:** Re-architected test file scripts to leverage fully-qualified type mappings (`TicTacToe.Backend.Models.GameService`), bypassing project namespace assembly mismatch bounds entirely.

---

## 2. Engineering Division of Labor

The table below outlines the professional boundary between automated generation loops and manual human architectural reviews applied to this workspace:

| System Layer | AI-Automated Generation | Manual Architectural Override / Refactoring |
| :--- | :--- | :--- |
| **State Ownership** | Initial REST endpoints schema modeling. | Enforced 100% pure backend rule verification; rejected frontend-side cell manipulation shortcuts. |
| **UI Change Detection** | Boilerplate Standalone component lifecycle hooks. | Injected explicit `ChangeDetectorRef` routines to eliminate asynchronous rendering frame lag. |
| **AI Competitor Logic** | Conditional array-checking heuristics. | Tailored transactional loop nesting to execute user inputs and computer blocks within a single API frame lifecycle. |
| **Test Coverage Suite** | Basic assertion stub templates. | Designed a comprehensive 13-point xUnit test matrix protecting column/diagonal matrices and post-game termination exceptions. |

---

## 3. Core Architectural Trade-offs & Assumptions

1.  **Option A Application Over Option B:** Choosing to completely lock out the "Undo Last Move" interaction after a match terminates simplifies the persistence model. The scoreboard updates exactly once per match lifecycle, keeping data records clean and avoiding complex reverse-calculation penalties.
2.  **Stateless API with Singleton Scope Caching:** Utilizing an in-memory dictionary registered via `.AddSingleton<IGameService>()` instead of deploying an immediate persistent database wrapper (like EF Core with SQLite) keeps the workspace completely local, portable, and easily shareable for the review panel without requiring local configuration steps.
3.  **Atomic Single-Transaction Computer Turns:** In Play Against Computer Mode, the computer's turn calculation runs inside the *same* HTTP POST transaction initiated by the human player. This completely removes the need for complex, error-prone client-side polling loops or persistent background workers.