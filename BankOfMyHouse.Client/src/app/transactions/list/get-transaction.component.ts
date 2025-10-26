import { Component, inject, OnInit, Signal, effect, ViewChild, AfterViewInit, ChangeDetectorRef, signal } from '@angular/core';
import { UserService } from '../../services/users/users.service';
import { TransactionService } from '../transaction.service';
import { GetUserDetailsResponseDto } from '../../auth/models/getUserDetails/getUserDetailsResponseDto';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatPaginatorModule, MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { GetTransactionsResponseDto } from '../models/getTransactionsResponseDto';
import { GetTransactionsRequestDto } from '../models/getTransactionsRequestDto';
import { TransactionDto } from '../models/transactionDto';

// Custom Italian Paginator
export class ItalianPaginatorIntl extends MatPaginatorIntl {
  override itemsPerPageLabel = 'Elementi per pag.';
  override nextPageLabel = 'Pagina successiva';
  override previousPageLabel = 'Pagina precedente';
  override firstPageLabel = 'Prima pagina';
  override lastPageLabel = 'Ultima pagina';

  override getRangeLabel = (page: number, pageSize: number, length: number): string => {
    if (length === 0 || pageSize === 0) {
      return `0 di ${length}`;
    }
    length = Math.max(length, 0);
    const startIndex = page * pageSize;
    const endIndex = startIndex < length ?
      Math.min(startIndex + pageSize, length) :
      startIndex + pageSize;
    return `${startIndex + 1} - ${endIndex} di ${length}`;
  };
}

@Component({
  selector: 'app-get-transaction',
  imports: [
    CurrencyPipe,
    DatePipe,
    FormsModule,
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  providers: [
    provideNativeDateAdapter(),
    { provide: MatPaginatorIntl, useClass: ItalianPaginatorIntl }
  ],
  templateUrl: './get-transaction.component.html',
  styleUrl: './get-transaction.component.css',
  standalone: true,
})
export class GetTransactionComponent implements OnInit, AfterViewInit {

  private usersService = inject(UserService);
  private transactionService = inject(TransactionService);
  private cdr = inject(ChangeDetectorRef);

  // ViewChild references for paginator and sort
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // State signals from service
  protected currentUser: Signal<GetUserDetailsResponseDto | null>;
  protected currentTransactions: Signal<GetTransactionsResponseDto | null>;

  // Date range filter signals with automatic triggering - prefilled with current month
  protected startDate = signal<Date | null>(new Date(new Date().getFullYear(), new Date().getMonth(), 1));
  protected endDate = signal<Date | null>(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0));

  // MatTableDataSource for table
  protected dataSource = new MatTableDataSource<TransactionDto>([]);

  // Columns to display in the table
  protected displayedColumns: string[] = [
    'transactionCreation',
    'description',
    'amount',
    'senderIban',
    'receiverIban'
  ];

  loading = this.usersService.loading;
  error = this.usersService.error;

  private paginatorInitialized = false;

  constructor() {
    this.currentUser = this.usersService.userDetails;
    this.currentTransactions = this.transactionService.getTransactions;

    // Effect to automatically fetch transactions when user data becomes available or dates change
    effect(() => {
      const user = this.currentUser();
      // Also track date changes to trigger refetch
      const start = this.startDate();
      const end = this.endDate();

      if (user?.bankAccounts) {
        // Only fetch if both dates are set or both are null
        if ((start && end) || (!start && !end)) {
          this.fetchTransactions();
        }
      }
    });

    // Effect to refresh transactions when a transaction is created
    effect(() => {
      const transactionCreated = this.transactionService.transactionCreated();
      if (transactionCreated) {
        this.fetchTransactions();
      }
    });

    // Effect to update dataSource when transactions change
    effect(() => {
      const transactions = this.currentTransactions();
      console.log('Transactions changed:', transactions);
      if (transactions?.transactions) {
                
        // Create sorted copy (ascending by creation date - oldest first)
        const sortedTransactions = [...transactions.transactions].sort((a, b) =>
          new Date(a.transactionCreation).getTime() - new Date(b.transactionCreation).getTime()
        );

        // Store current page index before updating data
        const currentPageIndex = this.paginator?.pageIndex ?? 0;

        this.dataSource.data = sortedTransactions;

        // Force table and paginator to refresh without resetting page
        if (this.paginatorInitialized && this.paginator) {
          // Temporarily disconnect and reconnect
          const tempPaginator = this.paginator;
          const tempSort = this.sort;
          this.dataSource.paginator = null;
          this.dataSource.sort = null;

          setTimeout(() => {
            this.dataSource.paginator = tempPaginator;
            this.dataSource.sort = tempSort;

            // Restore the page index if it's still valid
            if (tempPaginator && currentPageIndex < tempPaginator.getNumberOfPages()) {
              tempPaginator.pageIndex = currentPageIndex;
            }
          }, 0);
        }
      }
    });
  }

  private fetchTransactions(): void {
    const user = this.currentUser();
    if (user?.bankAccounts) {
      // Clear previous transactions before fetching new ones
      this.transactionService.clearTransactions();

      user.bankAccounts.forEach(element => {
        const request = new GetTransactionsRequestDto();
        request.iban = element.iban;
        request.startDate = this.startDate() ?? undefined;
        request.endDate = this.endDate() ?? undefined;
        this.transactionService.getTransactionsDetails(request);
      });
    }
  }

  protected clearDateFilter(): void {
    // Reset to current month range
    const now = new Date();
    this.startDate.set(new Date(now.getFullYear(), now.getMonth(), 1));
    this.endDate.set(new Date(now.getFullYear(), now.getMonth() + 1, 0));
  }

  ngOnInit() {
    this.usersService.getUserDetails();
  }

  ngAfterViewInit() {
    // Connect paginator and sort to data source
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.paginatorInitialized = true;

    console.log('Paginator connected:', this.paginator);
    console.log('Current data length:', this.dataSource.data.length);

    // Trigger initial update if data already exists
    if (this.dataSource.data.length > 0) {
      this.paginator._changePageSize(this.paginator.pageSize);
      this.cdr.detectChanges();
    }
  }
}
