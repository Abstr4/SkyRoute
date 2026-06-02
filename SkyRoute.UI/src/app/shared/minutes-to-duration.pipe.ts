import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'minutesToDuration',
})
export class MinutesToDurationPipe implements PipeTransform {
  public transform(minutes: number): string {
    if (minutes <= 0) return '0 min';

    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;

    const hourPart = hours > 0 ? `${hours} hour${hours > 1 ? 's' : ''}` : '';

    const minPart = mins > 0 ? `${mins} min` : '';

    return hourPart && minPart ? `${hourPart} ${minPart}` : hourPart || minPart;
  }
}
