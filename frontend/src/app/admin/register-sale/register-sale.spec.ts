import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterSale } from './register-sale';

describe('RegisterSale', () => {
  let component: RegisterSale;
  let fixture: ComponentFixture<RegisterSale>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterSale]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterSale);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
