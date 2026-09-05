import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Models embedded directly here to fix the missing import error
export interface MoveLog {
  moveNumber: number;
  player: string;
  row: number;
  column: number;
}

export interface GameState {
  gameId: string;
  board: string[];
  currentPlayer: string;
  mode: number; // 0 = TwoPlayer, 1 = Computer
  status: number; // 0 = InProgress, 1 = Won, 2 = Draw
  winner: string;
  winningCells: number[];
  moveHistory: MoveLog[];
}

export interface Scoreboard {
  xWins: number;
  oWins: number;
  draws: number;
}

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private baseUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  createGame(mode: number): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/games?mode=${mode}`, {});
  }

  submitMove(gameId: string, player: string, index: number): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/games/${gameId}/moves`, { player, index });
  }

  undoMove(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/games/${gameId}/undo`, {});
  }

  resetGame(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(`${this.baseUrl}/games/${gameId}/reset`, {});
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(`${this.baseUrl}/scoreboard`);
  }

  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(`${this.baseUrl}/scoreboard/reset`, {});
  }
}
