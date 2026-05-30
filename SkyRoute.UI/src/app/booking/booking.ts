import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { FormArray, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCard, MatCardContent } from '@angular/material/card';
import { FlightOffer } from '../flights/flights';

@Component({
  selector: 'app-booking',
  imports: [
    MatFormFieldModule, MatInputModule, FormsModule, ReactiveFormsModule,
    MatButtonModule, MatProgressSpinnerModule, MatCard, MatCardContent,
    DatePipe, CurrencyPipe,
  ],
  templateUrl: './booking.html',
  styleUrl: './booking.css',
})
export class Booking {
  readonly router = inject(Router);

  readonly flight: FlightOffer;
  readonly passengers: number;
  readonly isInternational: boolean;
  readonly documentTypeLabel: string;
  readonly documentTypeValue: string;

  readonly bookingForm: FormGroup;

  readonly loading = signal(false);
  bookingReference: string | null = null;
  errorMessage: string | null = null;

  constructor() {
    const nav = this.router.getCurrentNavigation();
    const state = nav?.extras?.state as { flight: FlightOffer; passengers: number } | undefined;

    if (!state?.flight) {
      this.flight = null!;
      this.passengers = 0;
      this.isInternational = false;
      this.documentTypeLabel = '';
      this.documentTypeValue = '';
      this.bookingForm = null!;
      this.router.navigate(['/']);
      return;
    }

    this.flight = state.flight;
    this.passengers = state.passengers;

    this.isInternational =
      this.flight.originAirport.countryCode !== this.flight.destinationAirport.countryCode;
    this.documentTypeLabel = this.isInternational ? 'Passport Number' : 'National ID';
    this.documentTypeValue = this.isInternational ? 'Passport' : 'NationalId';

    const groups = Array.from({ length: this.passengers }, () =>
      new FormGroup({
        fullName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
        email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
        documentNumber: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
      })
    );

    this.bookingForm = new FormGroup({
      passengersArray: new FormArray(groups),
    });
  }

  get passengersArray(): FormArray {
    return this.bookingForm.controls['passengersArray'] as FormArray;
  }

  async confirmBooking(): Promise<void> {
    if (this.bookingForm.invalid || this.loading()) {
      return;
    }

    this.loading.set(true);
    this.bookingReference = null;
    this.errorMessage = null;

    const passengerForms = this.passengersArray.value as Array<{
      fullName: string;
      email: string;
      documentNumber: string;
    }>;

    const body = {
      provider: this.flight.provider,
      flightNumber: this.flight.flightNumber,
      passengers: passengerForms.map(p => ({
        fullName: p.fullName,
        email: p.email,
        documentType: this.documentTypeValue,
        documentNumber: p.documentNumber,
      })),
    };

    try {
      const response = await fetch('https://localhost:7229/api/Booking', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
      });

      if (!response.ok) {
        const errorBody = await response.json();
        this.errorMessage = errorBody.error ?? 'Booking failed. Please try again.';
        return;
      }

      const data = await response.json();
      this.bookingReference = data.bookingReferenceCode;
    } catch {
      this.errorMessage = 'Network error. Please try again.';
    } finally {
      this.loading.set(false);
    }
  }
}
