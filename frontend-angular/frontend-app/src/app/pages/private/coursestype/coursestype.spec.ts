import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Coursestype } from './coursestype';

describe('Coursestype', () => {
  let component: Coursestype;
  let fixture: ComponentFixture<Coursestype>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Coursestype]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Coursestype);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
