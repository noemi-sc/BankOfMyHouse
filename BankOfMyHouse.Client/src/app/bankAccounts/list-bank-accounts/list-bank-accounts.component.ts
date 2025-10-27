import { Component, inject, ViewChild, AfterViewInit, effect, Signal, computed } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { UserService } from '../../services/users/users.service';
import { BankAccountDetailsDto, GetUserDetailsResponseDto } from '../../auth/models/getUserDetails/getUserDetailsResponseDto';

// Italian Paginator
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
  selector: 'app-list-bank-accounts',
  imports: [
    CurrencyPipe,
    DatePipe,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule
  ],
  providers: [
    { provide: MatPaginatorIntl, useClass: ItalianPaginatorIntl }
  ],
  templateUrl: './list-bank-accounts.component.html',
  styleUrl: './list-bank-accounts.component.css'
})
export class ListBankAccountsComponent implements AfterViewInit {

  private usersService = inject(UserService);

  // ViewChild for table
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // State from service
  protected currentUser: Signal<GetUserDetailsResponseDto | null>;

  // MatTableDataSource
  protected dataSource = new MatTableDataSource<BankAccountDetailsDto>([]);

  // Columns to display
  protected displayedColumns: string[] = ['iban', 'description', 'balance', 'creationDate'];

  // Computed signals for summary statistics
  protected totalAccounts = computed(() => {
    return this.currentUser()?.bankAccounts?.length ?? 0;
  });

  protected totalBalance = computed(() => {
    if (!this.currentUser()?.bankAccounts) return 0;
    return this.currentUser()!.bankAccounts.reduce((sum, account) => sum + account.balance, 0);
  });

  private paginatorInitialized = false;

  constructor() {
    this.currentUser = this.usersService.userDetails;

    // Effect to update table when user data changes
    effect(() => {
      const user = this.currentUser();
      if (user?.bankAccounts) {
        // Store current page index
        const currentPageIndex = this.paginator?.pageIndex ?? 0;

        this.dataSource.data = user.bankAccounts;

        // Force refresh without resetting page
        if (this.paginatorInitialized && this.paginator) {
          const tempPaginator = this.paginator;
          const tempSort = this.sort;
          this.dataSource.paginator = null;
          this.dataSource.sort = null;

          setTimeout(() => {
            this.dataSource.paginator = tempPaginator;
            this.dataSource.sort = tempSort;

            // Restore page index if valid
            if (tempPaginator && currentPageIndex < tempPaginator.getNumberOfPages()) {
              tempPaginator.pageIndex = currentPageIndex;
            }
          }, 0);
        }
      }
    });
  }

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.paginatorInitialized = true;
  }
}
