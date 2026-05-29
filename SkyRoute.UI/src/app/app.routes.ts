import { Routes } from '@angular/router';
import { FlightSearchComponent } from './flight-search/flight-search';
import { Flights } from './flights/flights';

export const routes: Routes = [
  { path: '', component: FlightSearchComponent },
  { path: 'flights', component: Flights },
];
