import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private readonly authService: AuthService = inject(AuthService);
  private readonly router: Router = inject(Router);

  email = '';
  password = '';
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly showPassword = signal(false);
  submitted = false;

  togglePasswordVisibility(): void {
    this.showPassword.update(v => !v);
  }

  isEmailInvalid(): boolean {
    if (!this.submitted) return false;
    if (!this.email) return true;
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return !emailPattern.test(this.email);
  }

  isPasswordInvalid(): boolean {
    if (!this.submitted) return false;
    return !this.password;
  }

  onSubmit(): void {
    this.submitted = true;

    if (this.isEmailInvalid() || this.isPasswordInvalid()) {
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err: any) => {
        this.isLoading.set(false);
        const msg =
          err.error?.message ||
          (err.status === 401
            ? 'Invalid email or password.'
            : err.status === 400
            ? 'Invalid login request. Please verify the provided email and password.'
            : err.status === 404
            ? 'Platform admin account was not found.'
            : 'Failed to connect to authentication server. Please ensure the backend is running.');
        this.errorMessage.set(msg);
      }
    });
  }
}
