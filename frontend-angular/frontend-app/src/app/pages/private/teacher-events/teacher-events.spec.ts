import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherEvents } from './teacher-events';

describe('TeacherEvents', () => {
  let component: TeacherEvents;
  let fixture: ComponentFixture<TeacherEvents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TeacherEvents]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TeacherEvents);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
