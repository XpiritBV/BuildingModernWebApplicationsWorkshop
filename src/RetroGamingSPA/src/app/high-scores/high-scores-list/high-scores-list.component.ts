import { Component, Input, EventEmitter, Output, OnInit } from '@angular/core';
import { HighScore } from 'src/app/shared/api/leaderboards-api/models';

@Component({
  selector: "app-high-scores-list",
  templateUrl: "./high-scores-list.component.html",
  styleUrls: ["./high-scores-list.component.scss"]
})
export class HighScoresListComponent implements OnInit {
  // This property will contain data passed from another component
  @Input()
  highScores: HighScore[] = [];

  // This property makes you able to publish events to the component using this component
  @Output()
  onHighScoreSelected: EventEmitter<HighScore> = new EventEmitter();

  constructor() {}

  ngOnInit() {}

  onHighScoreClicked(highScore: HighScore): void {
    this.onHighScoreSelected.emit(highScore);
  }
}

