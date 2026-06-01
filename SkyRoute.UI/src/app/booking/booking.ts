import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormArray, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCard, MatCardContent } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { CreateBookingRequest, FlightOffer } from '../models';
import { BookingService } from '../services/booking';
import { MinutesToDurationPipe } from '../shared/minutes-to-duration.pipe';

@Component({
  selector: 'app-booking',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    MatFormFieldModule, MatInputModule, FormsModule, ReactiveFormsModule,
    MatButtonModule, MatProgressSpinnerModule, MatCard, MatCardContent,
    RouterLink,
    DatePipe, CurrencyPipe, MinutesToDurationPipe,
  ],
  templateUrl: './booking.html',
  styleUrl: './booking.css',
})
export class Booking {
  protected readonly router = inject(Router);
  private readonly bookingService = inject(BookingService);

  protected readonly flight: FlightOffer;
  protected readonly passengers: number;
  protected readonly searchParams: Record<string, string>;
  protected readonly isInternational: boolean;
  protected readonly documentTypeLabel: string;
  protected readonly documentTypeValue: string;

  protected readonly bookingForm: FormGroup;

  protected readonly loading = signal(false);
  protected readonly bookingReference = signal<string | null>(null);
  protected readonly errorMessage = signal<string | null>(null);

  constructor() {
    const nav = this.router.currentNavigation();
    const state = nav?.extras?.state as
      { flight: FlightOffer; passengers: number; searchParams: Record<string, string> } | undefined;

    if (!state?.flight) {
      this.flight = null!;
      this.passengers = 0;
      this.searchParams = {};
      this.isInternational = false;
      this.documentTypeLabel = '';
      this.documentTypeValue = '';
      this.bookingForm = null!;
      this.router.navigate(['/']);
      return;
    }

    this.flight = state.flight;
    this.passengers = state.passengers;
    this.searchParams = state.searchParams ?? {};

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

  protected get passengersArray(): FormArray {
    return this.bookingForm.controls['passengersArray'] as FormArray;
  }

  protected async confirmBooking(): Promise<void> {
    if (this.bookingForm.invalid || this.loading()) {
      return;
    }

    this.loading.set(true);
    this.bookingReference.set(null);
    this.errorMessage.set(null);

    const passengerForms = this.passengersArray.value as Array<{
      fullName: string;
      email: string;
      documentNumber: string;
    }>;

    const body: CreateBookingRequest = this.buildBookingRequest(passengerForms);

    this.bookingService.confirmBooking(body).subscribe({
      next: (data) => {
        this.bookingReference.set(data.bookingReferenceCode);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error ?? 'Booking failed. Please try again.');
        this.loading.set(false);
      },
    });
  }

  private buildBookingRequest(passengerForms: { fullName: string; email: string; documentNumber: string; }[]): CreateBookingRequest {
    return {
      provider: this.flight.provider,
      flightNumber: this.flight.flightNumber,
      passengers: passengerForms.map(p => ({
        fullName: p.fullName,
        email: p.email,
        documentType: this.documentTypeValue,
        documentNumber: p.documentNumber,
      })),
    };
  }
}
