import { Component, input } from '@angular/core';
import { PackageBadge } from '../../../shared/ui/package-badge/package-badge';

@Component({
  selector: 'app-auth-shell',
  imports: [PackageBadge],
  templateUrl: './auth-shell.html',
  styleUrl: './auth-shell.scss',
})
export class AuthShell {
  readonly eyebrow = input.required<string>();
  readonly heading = input.required<string>();
  readonly text = input.required<string>();
}
