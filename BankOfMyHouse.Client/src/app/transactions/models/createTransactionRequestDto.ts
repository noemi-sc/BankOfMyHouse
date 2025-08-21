export class CreateTransactionRequestDto {
    senderIban: IbanCodeDto = new IbanCodeDto('');
    receiverIban: IbanCodeDto = new IbanCodeDto('');
    amount: number = 0;
    currencyCode: string = '€';
    paymentCategory: PaymentCategoryCode = PaymentCategoryCode.Other;
    description: string = '';
}

export class IbanCodeDto {
    constructor(value: string) {
        this.value = value.trim();
    }

    value: string = '';
}

export enum PaymentCategoryCode {
    Other,
    Shopping,
    Food,
    Transport,
    Utilities,
    Entertainment,
    Healthcare,
    Education,
    Salary,
    Investment,
    Savings,
    Rent,
    Insurance
}