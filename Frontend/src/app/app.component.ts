import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; // 👈 1. Added ChangeDetectorRef here
import { CommonModule } from '@angular/common';
import { GameService, GameState, Scoreboard } from './services/game.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  game!: GameState;
  scoreboard!: Scoreboard;
  selectedMode: number = 0; // 0 = TwoPlayer, 1 = Computer

  // 👈 2. Injected ChangeDetectorRef (cdr) right into the constructor
  constructor(private gameService: GameService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.startNewSession(this.selectedMode);
  }

  startNewSession(mode: number): void {
    this.selectedMode = mode;
    this.gameService.createGame(mode).subscribe({
      next: (state: GameState) => {
        this.game = state;
        this.refreshScore();
        this.cdr.detectChanges(); // 👈 3. Force UI refresh
      },
      error: () => alert("Could not connect to the backend server. Make sure your .NET API is running on port 5000!")
    });
  }

  cellClicked(index: number): void {
    if (this.game.board[index] || this.game.status !== 0) return;
    
    this.gameService.submitMove(this.game.gameId, this.game.currentPlayer, index).subscribe({
      next: (state: GameState) => {
        this.game = state;
        if (this.game.status !== 0) this.refreshScore();
        this.cdr.detectChanges(); // 👈 4. Force UI refresh instantly on click
      },
      error: (err: any) => alert(err.error?.message || "An invalid move operation occurred.")
    });
  }

  undo(): void {
    if (this.game.moveHistory.length === 0 || this.game.status !== 0) return;
    this.gameService.undoMove(this.game.gameId).subscribe((state: GameState) => {
      this.game = state;
      this.cdr.detectChanges(); // 👈 5. Force UI refresh on undo
    });
  }

  resetGame(): void {
    this.gameService.resetGame(this.game.gameId).subscribe((state: GameState) => {
      this.game = state;
      this.cdr.detectChanges(); // 👈 6. Force UI refresh on reset
    });
  }

  resetScoreboard(): void {
    this.gameService.resetScoreboard().subscribe((score: Scoreboard) => {
      this.scoreboard = score;
      this.cdr.detectChanges(); // 👈 7. Force UI refresh on score reset
    });
  }

  refreshScore(): void {
    this.gameService.getScoreboard().subscribe((score: Scoreboard) => {
      this.scoreboard = score;
      this.cdr.detectChanges(); // 👈 8. Force UI refresh on score load
    });
  }

  isWinningCell(index: number): boolean {
    return this.game?.winningCells?.includes(index) || false;
  }
}
