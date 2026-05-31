import { SelectionModel } from '@angular/cdk/collections';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AfterViewInit, Component, inject, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Router } from '@angular/router';
import { FlightOffer } from '../models';
import { MinutesToDurationPipe } from '../shared/minutes-to-duration.pipe';

@Component({
  selector: 'app-flights',
  imports: [MatTableModule, MatSortModule, MatButtonModule, MatCheckboxModule, DatePipe, CurrencyPipe, MinutesToDurationPipe],
  templateUrl: './flights.html',
  styleUrl: './flights.css',
})
export class Flights implements AfterViewInit {
  private readonly router = inject(Router);

  @ViewChild(MatSort) sort!: MatSort;

  private passengers = 1;

  readonly selection = new SelectionModel<FlightOffer>(false, []);

  readonly displayedColumns = [
    'select',
    'provider',
    'flightNumber',
    'departureTime',
    'arrivalTime',
    'durationMinutes',
    'cabinClass',
    'price',
  ];

  readonly dataSource = new MatTableDataSource<FlightOffer>([]);

  constructor() {
    const nav = this.router.getCurrentNavigation();
    const state = nav?.extras?.state as any;
    this.dataSource.data = state?.['results'] ?? [];
    this.passengers = state?.['passengers'] ?? 1;
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
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

  goBack(): void {
    this.router.navigate(['/']);
  }

  continue(): void {
    this.router.navigate(['/booking'], {
      state: { flight: this.selection.selected[0], passengers: this.passengers },
    });
  }
}
