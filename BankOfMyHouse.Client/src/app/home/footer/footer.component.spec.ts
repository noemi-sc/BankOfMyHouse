import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BankFooterComponent } from './footer.component';

describe('FooterComponent', () => {
  let component: BankFooterComponent;
  let fixture: ComponentFixture<BankFooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BankFooterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BankFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
