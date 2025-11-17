import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCreateEvents } from './admin-create-events';

describe('AdminCreateEvents', () => {
  let component: AdminCreateEvents;
  let fixture: ComponentFixture<AdminCreateEvents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminCreateEvents]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminCreateEvents);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
