import { SelectionModel } from '@angular/cdk/collections';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { FlightOffer, FlightSearchRequest } from '../models';
import { FlightSearchService } from '../services/flight-search';
import { MinutesToDurationPipe } from '../shared/minutes-to-duration.pipe';

@Component({
  selector: 'app-flights',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatTableModule,
    MatSortModule,
    MatButtonModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    RouterLink,
    DatePipe,
    CurrencyPipe,
    MinutesToDurationPipe,
  ],
  templateUrl: './flights.html',
  styleUrl: './flights.css',
})
export class Flights implements OnInit, OnDestroy {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly flightSearchService = inject(FlightSearchService);
  private readonly destroy$ = new Subject<void>();

  @ViewChild(MatSort) protected set sort(sort: MatSort) {
    this.dataSource.sort = sort;
  }

  protected readonly passengers = signal(1);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly selection = new SelectionModel<FlightOffer>(false, []);

  protected readonly displayedColumns = [
    'select',
    'provider',
    'flightNumber',
    'departureTime',
    'arrivalTime',
    'durationMinutes',
    'cabinClass',
    'price',
  ];

  protected readonly dataSource = new MatTableDataSource<FlightOffer>([]);

  protected currentSearchParams: Record<string, string> = {};

  constructor() {
    this.dataSource.sortingDataAccessor = (item: FlightOffer, property: string) => {
      switch (property) {
        case 'durationMinutes':
          return item.durationMinutes;
        case 'departureTime':
          return new Date(item.departureTime).getTime();
        case 'price':
          return item.totalPrice;
        default:
          return (item as unknown as Record<string, string>)[property] ?? '';
      }
    };
  }

  public ngOnInit(): void {
    this.route.queryParamMap.pipe(takeUntil(this.destroy$)).subscribe((params) => {
      const originAirportCode = params.get('originAirportCode');
      const destinationAirportCode = params.get('destinationAirportCode');
      const departureDate = params.get('departureDate');

      if (!originAirportCode || !destinationAirportCode || !departureDate) {
        this.router.navigate(['/']);
        return;
      }

      const passengers = parseInt(params.get('passengers') ?? '1', 10);
      const cabinClass = (params.get('cabinClass') ?? 'Economy') as FlightSearchRequest['cabinClass'];
      const timeZone = params.get('timeZone') ?? Intl.DateTimeFormat().resolvedOptions().timeZone;

      this.currentSearchParams = {
        originAirportCode,
        destinationAirportCode,
        departureDate,
        passengers: String(passengers),
        cabinClass: String(cabinClass),
        timeZone,
      };
      this.passengers.set(passengers);
      this.loading.set(true);
      this.error.set(null);
      this.selection.clear();

      this.flightSearchService
        .searchFlights({
          originAirportCode,
          destinationAirportCode,
          departureDate,
          passengers,
          cabinClass,
          timeZone,
        })
        .subscribe({
          next: (data) => {
            this.dataSource.data = data;
            this.loading.set(false);
          },
          error: () => {
            this.error.set('Failed to load flights. Please try again.');
            this.loading.set(false);
          },
        });
    });
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
