export class GetUserDetailsResponseDto {
    id: number = 0;
    username: string = '';
    email: string = '';
    firstName?: string;
    lastName?: string;
    createdAt: Date = new Date();
    lastLoginAt?: Date;
    isActive: boolean = false;
    roles: string[] = [];
    bankAccounts: BankAccountDetailsDto[] = [];
}

export class BankAccountDetailsDto {
    id: number = 0;
    iban: string = '';
    balance: number = 0;
    creationDate: Date = new Date();
}