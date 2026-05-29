import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FlightSearchComponent } from './flight-search/flight-search';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FlightSearchComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('SkyRoute.UI');
}
