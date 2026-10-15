import { Component, computed, input } from '@angular/core';

import { coverArt } from '../../utils/cover-art';

@Component({
  selector: 'app-cover',
  template: `<span>{{ initials() }}</span>`,
  host: {
    'aria-hidden': 'true',
    '[style.background]': 'background()',
  },
  styles: `
    :host {
      display: grid;
      place-items: center;
      overflow: hidden;
      container-type: size;
      color: rgba(255, 255, 255, 0.85);
      font-family: var(--mel-font-heading);
      font-weight: 800;
      user-select: none;
    }
    span {
      font-size: 36cqmin;
      line-height: 1;
      letter-spacing: 0.02em;
      text-shadow: 0 2px 12px rgba(0, 0, 0, 0.35);
    }
  `,
})
export class Cover {
  readonly name = input.required<string>();

  protected readonly background = computed(() => coverArt(this.name()));

  protected readonly initials = computed(() => {
    const words = this.name().trim().split(/\s+/).filter(Boolean);
    const letters = words.length > 1 ? words[0][0] + words[words.length - 1][0] : (words[0]?.[0] ?? '?');
    return letters.toLocaleUpperCase('tr-TR');
  });
}
