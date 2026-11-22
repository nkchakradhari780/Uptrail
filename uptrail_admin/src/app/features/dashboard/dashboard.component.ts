import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { PlatformAdminService } from '../../core/services/platform-admin.service';
import { ApiResponse, PlatformAdminResponse } from '../../core/models/auth.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private readonly authService: AuthService = inject(AuthService);
  private readonly platformAdminService: PlatformAdminService = inject(PlatformAdminService);

  readonly currentUser = this.authService.currentUser;
  readonly admins = signal<PlatformAdminResponse[]>([]);
  readonly isLoadingAdmins = signal(false);
  readonly isRefreshing = signal(false);
  readonly actionMessage = signal<string | null>(null);

  searchQuery = '';

  userInitials(): string {
    const name = this.currentUser()?.name || 'AD';
    return name.slice(0, 2).toUpperCase();
  }

  ngOnInit(): void {
    this.loadAdmins();
  }

  loadAdmins(): void {
    this.isLoadingAdmins.set(true);
    const query = this.searchQuery?.trim();
    const request$ = query
      ? this.platformAdminService.searchPlatformAdmins(query)
      : this.platformAdminService.getPlatformAdmins();

    request$.subscribe({
      next: (res: ApiResponse<PlatformAdminResponse[]>) => {
        this.admins.set(res.data || []);
        this.isLoadingAdmins.set(false);
      },
      error: (err: any) => {
        this.isLoadingAdmins.set(false);
        this.admins.set([]);
        const errorMsg =
          err.error?.message ||
          (err.status === 404
            ? 'No platform admin exist'
            : `Failed to load platform admins: ${err.message || 'API error'}`);
        this.actionMessage.set(errorMsg);
      }
    });
  }

  onSearch(): void {
    this.loadAdmins();
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.loadAdmins();
  }

  refreshTokens(): void {
    this.isRefreshing.set(true);
    this.authService.refreshSession().subscribe({
      next: () => {
        this.isRefreshing.set(false);
        this.actionMessage.set('Session successfully refreshed with new access and refresh tokens.');
      },
      error: () => {
        this.isRefreshing.set(false);
      }
    });
  }

  trackByAdminId(index: number, admin: PlatformAdminResponse): string {
    return admin.id;
  }

  logout(): void {
    this.authService.logout();
  }
}
