import { Component, inject, input, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { PasswordModule } from 'primeng/password';
import { AuthShell } from '../auth-shell/auth-shell';
import { AuthService } from '../../../core/auth/auth.service';
import { ProblemDetails, toProblem } from '../../../core/http/problem-details';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, RouterLink, ButtonModule, InputTextModule, MessageModule, PasswordModule, AuthShell],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss',
})
export class LoginPage {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(NonNullableFormBuilder);

  readonly returnUrl = input<string>();

  protected readonly loading = signal(false);
  protected readonly problem = signal<ProblemDetails | null>(null);

  protected readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.problem.set(null);

    this.auth
      .login(this.form.getRawValue())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: () => this.router.navigateByUrl(this.safeReturnUrl()),
        error: (err) => this.problem.set(toProblem(err)),
      });
  }

  private safeReturnUrl(): string {
    const url = this.returnUrl();
    return url?.startsWith('/') && !url.startsWith('//') ? url : '/';
  }
}
