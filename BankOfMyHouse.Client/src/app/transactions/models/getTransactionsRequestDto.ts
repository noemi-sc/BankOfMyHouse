export class GetTransactionsRequestDto {
    startDate: Date = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000);
    endDate: Date = new Date();
    iban: string = '';
}