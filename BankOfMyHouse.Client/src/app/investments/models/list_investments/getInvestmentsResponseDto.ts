export class InvestmentDto {
    id: number;
    sharesAmount: number;
    purchasePrice: number;
    createdAt: Date;
    companyId: number;
    bankAccountId: number;

    private constructor(
        sharesAmount: number,
        purchasePrice: number,
        companyId: number,
        bankAccountId: number,
        createdAt: Date,
        id: number
    ) {
        this.id = id;
        this.sharesAmount = sharesAmount;
        this.purchasePrice = purchasePrice;
        this.companyId = companyId;
        this.bankAccountId = bankAccountId;
        this.createdAt = createdAt;
    }
}

export class GetInvestmentsResponseDto {
    investments: InvestmentDto[];

    constructor(investments: InvestmentDto[] = []) {
        this.investments = investments;
    }
}
