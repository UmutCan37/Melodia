import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'duration' })
export class DurationPipe implements PipeTransform {
  transform(totalSeconds: number | null | undefined): string {
    if (totalSeconds == null || totalSeconds < 0) {
      return '0:00';
    }

    const seconds = Math.floor(totalSeconds);
    const minutes = Math.floor(seconds / 60);
    const rest = seconds % 60;

    return `${minutes}:${rest.toString().padStart(2, '0')}`;
  }
}
