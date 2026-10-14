import { Component, computed, input } from '@angular/core';

import { PackageName } from '../../../core/auth/auth.models';

const LABELS: Record<PackageName, string> = {
  Basic: 'BASIC',
  Gold: 'GOLD',
  Premium: 'PREMIUM',
  Elit: 'ELİT',
};

@Component({
  selector: 'app-package-badge',
  template: `{{ label() }}`,
  host: { '[class]': 'tierClass()' },
  styles: `
    :host {
      display: inline-flex;
      align-items: center;
      padding: 2px 7px;
      border: 1px solid;
      border-radius: 4px;
      font-size: 11px;
      font-weight: 600;
      line-height: 16px;
      letter-spacing: 0.04em;
      white-space: nowrap;
    }
    :host(.basic) {
      background: var(--mel-basic-bg);
      border-color: rgba(100, 116, 139, 0.4);
      color: var(--mel-basic-fg);
    }
    :host(.gold) {
      background: var(--mel-gold-bg);
      border-color: rgba(245, 158, 11, 0.45);
      color: var(--mel-gold-fg);
    }
    :host(.premium) {
      background: var(--mel-premium-bg);
      border-color: rgba(16, 185, 129, 0.45);
      color: var(--mel-premium-fg);
    }
    :host(.elit) {
      background: var(--mel-elit-bg);
      border-color: rgba(139, 92, 246, 0.5);
      color: var(--mel-elit-fg);
    }
  `,
})
export class PackageBadge {
  readonly tier = input.required<PackageName>();

  protected readonly label = computed(() => LABELS[this.tier()]);
  protected readonly tierClass = computed(() => this.tier().toLowerCase());
}
