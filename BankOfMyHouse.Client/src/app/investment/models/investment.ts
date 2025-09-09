import { Injectable } from "@angular/core";

export interface CompanyStockPrice {
    StockPrice: number,
    TimeOfPriceChange: Date,
    CompanyId: number
}

