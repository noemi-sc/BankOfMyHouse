export class GetTransactionsRequestDto {
    startDate: Date = new Date();
    EndDate: Date = new Date();
    iban: string = '';
}