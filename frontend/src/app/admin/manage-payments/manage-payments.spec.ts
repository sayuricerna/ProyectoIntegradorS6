import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagePayments } from './manage-payments';

describe('ManagePayments', () => {
  let component: ManagePayments;
  let fixture: ComponentFixture<ManagePayments>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManagePayments]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManagePayments);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
