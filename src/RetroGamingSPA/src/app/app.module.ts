import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppMaterialModule } from './app-material.module';
import { HighScoresComponent } from './high-scores/high-scores.component';
import { HighScoresListComponent } from './high-scores/high-scores-list/high-scores-list.component';
import { AddScoreComponent } from './add-score/add-score.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { ApiModule } from './shared/api/leaderboards-api/api.module';

@NgModule({
  declarations: [
    AppComponent,
    HighScoresComponent,
    HighScoresListComponent,
    AddScoreComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    AppMaterialModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    HttpClientModule,
    ApiModule.forRoot({ rootUrl: 'https://localhost:44366' }),
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
