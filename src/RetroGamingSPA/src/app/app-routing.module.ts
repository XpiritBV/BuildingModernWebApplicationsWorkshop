import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HighScoresComponent } from './high-scores/high-scores.component';
import { AddScoreComponent } from './add-score/add-score.component';

const routes: Routes = [
  {
    path: "",
    component: HighScoresComponent
  },
  // Add 'add score' route
  {
    path: "add-score",
    component: AddScoreComponent
  }  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
