export interface GetBankAccountResponseDto {
    BankAccounts: BankAccount[]
}

export interface BankAccount {
    Id: number,
    IBAN: string,
    UserId: number,
    CreationDate: Date,
    Balance: number
}