import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Coursesindividual } from './coursesindividual';

describe('Coursesindividual', () => {
  let component: Coursesindividual;
  let fixture: ComponentFixture<Coursesindividual>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Coursesindividual]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Coursesindividual);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
