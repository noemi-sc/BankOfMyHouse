import { TestBed } from '@angular/core/testing';

import { StockPriceSignalRService } from './stock-price-signalr.service';

describe('StockPriceSignalRService', () => {
  let service: StockPriceSignalRService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StockPriceSignalRService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
