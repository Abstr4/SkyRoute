import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import {provideNativeDateAdapter} from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { Router } from '@angular/router';
import { FlightSearchRequest } from '../models';
import { AIRPORTS, CABIN_CLASSES } from '../models/constants';

@Component({
  selector: 'app-flight-search',
  providers: [provideNativeDateAdapter()],
  imports: [MatFormFieldModule, MatSelectModule, MatInputModule, FormsModule, MatDatepickerModule, ReactiveFormsModule, MatButtonModule, MatProgressSpinnerModule, MatCard, MatCardContent, MatCardTitle, MatCardSubtitle, MatCardHeader],
  templateUrl: './flight-search.html',
  styleUrl: './flight-search.css',
})

export class FlightSearchComponent {

  readonly AIRPORTS = AIRPORTS;
  readonly CABIN_CLASSES = CABIN_CLASSES;

  readonly searchForm = new FormGroup({
  originAirportCode: new FormControl('', { 
    nonNullable: true,
    validators: [Validators.required] 
  }),
  destinationAirportCode: new FormControl('', { 
    nonNullable: true,
    validators: [Validators.required] 
  }),
  departureDate: new FormControl('', {
    nonNullable: true,
    validators: [
      Validators.required,
      this.todayOrFutureValidator()
    ]
  }),
  passengers: new FormControl(1, {
    nonNullable: true,
    validators: [
      Validators.required,
      Validators.min(1),
      Validators.max(9)
    ]
  }),
  cabinClass: new FormControl('Economy', { nonNullable: true }),
  });

  private readonly router = inject(Router);

  readonly loading = signal(false);

  protected minDate = new Date();

  async onSearch(): Promise<void> {
    if (this.searchForm.invalid || this.loading()) {
      return;
    }

    const raw = this.searchForm.getRawValue();

    if (raw.originAirportCode === raw.destinationAirportCode) {
      return;
    }

    const cabinClass = CABIN_CLASSES.find(c => c === raw.cabinClass);
    if (!cabinClass) return;

    const date = new Date(raw.departureDate);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    const body = {
      originAirportCode: raw.originAirportCode,
      destinationAirportCode: raw.destinationAirportCode,
      departureDate: `${year}-${month}-${day}`,
      passengers: raw.passengers,
      cabinClass,
    };

    this.loading.set(true);

    try {
      const response = await fetch(
        'https://localhost:7229/api/Flights/search',
        {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(body),
        }
      );

      if (!response.ok) {
        const errorText = await response.text();
        console.error('Search failed:', response.status, errorText);
        return;
      }

      const data = await response.json();
      await this.router.navigate(['/flights'], { state: { results: data, passengers: raw.passengers } });
    } catch (error) {
      console.error('Network error:', error);
    } finally {
      this.loading.set(false);
    }
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

      return selectedDate < today
        ? { pastDate: true }
        : null;
    };
  }
}
