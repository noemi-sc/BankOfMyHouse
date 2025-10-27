export class InvestmentDto {
    id: number;
    sharesAmount: number;
    createdAt: Date;
    companyId: number;
    bankAccountId: number;

    private constructor(
        sharesAmount: number,
        companyId: number,
        bankAccountId: number,
        createdAt: Date,
        id: number
    ) {
        this.id = id;
        this.sharesAmount = sharesAmount;
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
