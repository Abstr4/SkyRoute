import { Component, inject, ViewChild, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';

export interface Airport {
  code: string;
  name: string;
  city: string;
  country: string;
  countryCode: string;
  displayName: string;
  citySelectorName: string;
}

export interface FlightOffer {
  provider: string;
  flightNumber: string;
  originAirport: Airport;
  destinationAirport: Airport;
  departureTime: string;
  arrivalTime: string;
  durationMinutes: number;
  cabinClass: string;
  pricePerPassenger: number;
  totalPrice: number;
}

@Component({
  selector: 'app-flights',
  imports: [MatTableModule, MatSortModule, MatButtonModule, MatCheckboxModule, DatePipe, CurrencyPipe],
  templateUrl: './flights.html',
  styleUrl: './flights.css',
})
export class Flights implements AfterViewInit {
  private readonly router = inject(Router);

  @ViewChild(MatSort) sort!: MatSort;

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
    const results = (nav?.extras?.state as any)?.['results'] ?? [];
    this.dataSource.data = results;
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
    console.log('Selected flight:', this.selection.selected[0]);
  }
}
