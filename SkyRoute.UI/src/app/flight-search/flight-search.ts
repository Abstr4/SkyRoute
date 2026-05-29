import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import {provideNativeDateAdapter} from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCard, MatCardContent, MatCardHeader, MatCardSubtitle, MatCardTitle } from '@angular/material/card';

@Component({
  selector: 'app-flight-search',
  providers: [provideNativeDateAdapter()],
  imports: [MatFormFieldModule, MatSelectModule, MatInputModule, FormsModule, MatDatepickerModule, ReactiveFormsModule, MatButtonModule, MatCard, MatCardContent, MatCardTitle, MatCardSubtitle, MatCardHeader],
  templateUrl: './flight-search.html',
  styleUrl: './flight-search.css',
})

export class FlightSearchComponent {

  readonly airports = [
    { code: 'EZE', name: 'Buenos Aires, Argentina' },
    { code: 'COR', name: 'Córdoba, Argentina' },
    { code: 'MDZ', name: 'Mendoza, Argentina' },
    { code: 'GRU', name: 'São Paulo, Brazil' },
    { code: 'GIG', name: 'Rio de Janeiro, Brazil' },
    { code: 'SCL', name: 'Santiago, Chile' },
  ];

  readonly passengerOptions = [1, 2, 3, 4, 5, 6, 7, 8, 9];

  readonly cabinClasses = ['Economy', 'Business', 'First Class'];

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

  protected minDate = new Date();

  onSearch(): void {
    console.log('Search submitted with values:', this.searchForm.value);
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
