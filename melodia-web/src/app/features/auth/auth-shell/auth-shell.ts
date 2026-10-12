import { Component, input } from '@angular/core';

@Component({
  selector: 'app-auth-shell',
  templateUrl: './auth-shell.html',
  styleUrl: './auth-shell.scss',
})
export class AuthShell {
  readonly eyebrow = input.required<string>();
  readonly heading = input.required<string>();
  readonly text = input.required<string>();
}
