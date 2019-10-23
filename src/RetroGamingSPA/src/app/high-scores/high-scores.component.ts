import { Component, OnInit } from '@angular/core';
import { HighScore } from '../shared/api/leaderboards-api/models';
import { LeaderboardService } from '../shared/api/leaderboards-api/services';
import { Observable } from 'rxjs';

@Component({
  selector: "app-high-scores",
  templateUrl: "./high-scores.component.html",
  styleUrls: ["./high-scores.component.scss"]
})
export class HighScoresComponent implements OnInit {
  public highScores$: Observable<HighScore[]>;

  constructor(private readonly leaderBoardsService: LeaderboardService) { }

  ngOnInit() {
    this.highScores$ = this.getHighScores();
  }

  onHighScoreSelected(highScore: HighScore) {
    // Log the selected highscore to the developer console.
    console.log(`High score selected`, highScore);
  }

  private getHighScores(): Observable<Array<HighScore>> {
    return this.leaderBoardsService
      .leaderboardGet({
        version: "1.0",
        format: 'json'
      });
  }
}
