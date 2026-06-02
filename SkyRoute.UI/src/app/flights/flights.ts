import { SelectionModel } from '@angular/cdk/collections';
import { CurrencyPipe, DatePipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  inject,
  OnInit,
  signal,
  ViewChild,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
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
export class Flights implements OnInit {
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly flightSearchService = inject(FlightSearchService);

  @ViewChild(MatSort) protected set sort(sort: MatSort) {
    this.flights.sort = sort;
  }

  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected passengers = 1;

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

  protected readonly flights = new MatTableDataSource<FlightOffer>([]);

  protected currentSearchParams: Record<string, string> = {};

  constructor() {
    this.flights.sortingDataAccessor = (item: FlightOffer, property: string) => {
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
    const p = this.route.snapshot.queryParamMap;
    const originAirportCode = p.get('originAirportCode');
    const destinationAirportCode = p.get('destinationAirportCode');
    const departureDate = p.get('departureDate');

    if (!originAirportCode || !destinationAirportCode || !departureDate) {
      this.router.navigate(['/']);
      return;
    }

    const passengers = parseInt(p.get('passengers') ?? '1', 10);
    const cabinClass = (p.get('cabinClass') ?? 'Economy') as FlightSearchRequest['cabinClass'];
    const timeZone = p.get('timeZone') ?? Intl.DateTimeFormat().resolvedOptions().timeZone;

    this.passengers = passengers;
    this.currentSearchParams = {
      originAirportCode,
      destinationAirportCode,
      departureDate,
      passengers: String(passengers),
      cabinClass: String(cabinClass),
      timeZone,
    };

    this.loading.set(true);
    this.flightSearchService
      .searchFlights({
        originAirportCode,
        destinationAirportCode,
        departureDate,
        passengers: passengers,
        cabinClass,
        timeZone,
      })
      .subscribe({
        next: (data) => {
          this.flights.data = data;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Failed to load flights. Please try again.');
          this.loading.set(false);
        },
      });
  }
}
