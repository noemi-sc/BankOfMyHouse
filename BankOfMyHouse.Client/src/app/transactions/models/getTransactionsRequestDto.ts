export class GetTransactionsRequestDto {
    startDate?: Date;
    endDate?: Date;
    iban: string = '';
}