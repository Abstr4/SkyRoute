import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import {
  MatCard,
  MatCardContent,
  MatCardHeader,
  MatCardSubtitle,
  MatCardTitle,
} from '@angular/material/card';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { FlightSearchRequest } from '../models';
import { AIRPORTS, CABIN_CLASSES, CabinClass } from '../models/constants';
import { FlightSearchService } from '../services/flight-search';

@Component({
  selector: 'app-flight-search',
  providers: [provideNativeDateAdapter()],
  imports: [
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    FormsModule,
    MatDatepickerModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatCard,
    MatCardContent,
    MatCardTitle,
    MatCardSubtitle,
    MatCardHeader,
  ],
  templateUrl: './flight-search.html',
  styleUrl: './flight-search.css',
})
export class FlightSearchComponent {
  protected readonly AIRPORTS = AIRPORTS;
  protected readonly CABIN_CLASSES = CABIN_CLASSES;
  private flightSearchService = inject(FlightSearchService);
  private readonly router = inject(Router);
  protected readonly loading = signal(false);
  protected minDate = new Date();

  protected readonly searchForm = new FormGroup({
    originAirportCode: new FormControl<string>('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    destinationAirportCode: new FormControl<string>('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    departureDate: new FormControl<string>('', {
      nonNullable: true,
      validators: [Validators.required, this.todayOrFutureValidator()],
    }),
    passengers: new FormControl<number>(1, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(1), Validators.max(9)],
    }),
    cabinClass: new FormControl<CabinClass>('Economy', { nonNullable: true }),
  });

  onSearch(): void {
    if (this.searchForm.invalid || this.loading()) {
      return;
    }
    const raw = this.searchForm.getRawValue();

    const request: FlightSearchRequest = this.buildRequest(raw);

    this.loading.set(true);

    this.flightSearchService.searchFlights(request).subscribe({
      next: (data) => {
        this.router.navigate(['/flights'], {
          state: { results: data, passengers: raw.passengers },
        });
      },
      error: (error) => {
        console.error('Search failed:', error);
        this.loading.set(false);
      },
      complete: () => {
        this.loading.set(false);
      },
    });
  }

  private buildRequest(raw: {
    originAirportCode: string;
    destinationAirportCode: string;
    departureDate: string;
    passengers: number;
    cabinClass: 'Economy' | 'Business' | 'FirstClass';
  }): FlightSearchRequest {
    return {
      originAirportCode: raw.originAirportCode,
      destinationAirportCode: raw.destinationAirportCode,
      departureDate: this.toDateOnly(raw.departureDate),
      passengers: raw.passengers,
      cabinClass: raw.cabinClass,
      timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
    };
  }

  private toDateOnly(value: string | Date): string {
    const date = new Date(value);

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  private todayOrFutureValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }

      const selectedDate = new Date(control.value);
      selectedDate.setHours(0, 0, 0, 0);

      const today = new Date();
      today.setHours(0, 0, 0, 0);

      return selectedDate < today ? { pastDate: true } : null;
    };
  }
}
