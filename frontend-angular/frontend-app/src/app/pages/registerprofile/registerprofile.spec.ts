import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Registerprofile } from './registerprofile';

describe('Registerprofile', () => {
  let component: Registerprofile;
  let fixture: ComponentFixture<Registerprofile>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Registerprofile]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Registerprofile);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
