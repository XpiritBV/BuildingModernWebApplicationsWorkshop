import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HighScoresListComponent } from './high-scores-list.component';

describe('HighScoresListComponent', () => {
  let component: HighScoresListComponent;
  let fixture: ComponentFixture<HighScoresListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HighScoresListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HighScoresListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
