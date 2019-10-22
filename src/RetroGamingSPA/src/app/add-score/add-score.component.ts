import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { OnInit, Component } from '@angular/core';
import { ScoresService } from '../shared/api/leaderboards-api/services';

@Component({
  selector: "app-add-score",
  templateUrl: "./add-score.component.html",
  styleUrls: ["./add-score.component.scss"]
})
export class AddScoreComponent implements OnInit {
  addScoreForm: FormGroup;

  isScorePosted: boolean = false;

  constructor(private fb: FormBuilder, private readonly scoresService: ScoresService) {}
  
  ngOnInit() {
    this.addScoreForm = this.fb.group({
      nickname: ["", Validators.required],
      game: [""],
      points: [0]
    });
  }

  submitScore(): void {
    var formValues = this.addScoreForm.value;

    this.scoresService.scoresPostScore({
      game: formValues.game,
      nickname: formValues.nickname,
      body: formValues.points,
      version: "1.0"
    }).subscribe(x => {
      this.isScorePosted = true;
    });
  }
}