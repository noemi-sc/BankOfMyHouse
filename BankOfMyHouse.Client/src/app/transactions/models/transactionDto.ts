import { PaymentCategoryCode } from "./createTransactionRequestDto";

export class TransactionDto {
    id: string = '';
    amount: number = 0;
    transactionCreation: Date = new Date();
    paymentCategory: PaymentCategoryCode = PaymentCategoryCode.Other; 
    currencyCode: string = '';
    currencySymbol: string = '€';
    description: string = '';
	senderIban: string = '';
    receiverIban: string = '';
}
