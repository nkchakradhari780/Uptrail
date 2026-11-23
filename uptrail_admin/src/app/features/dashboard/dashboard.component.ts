import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { PlatformAdminService } from '../../core/services/platform-admin.service';
import { RoleService } from '../../core/services/role.service';
import { CompanyService } from '../../core/services/company.service';
import { ApiResponse, PlatformAdminResponse } from '../../core/models/auth.models';
import { CreateRoleRequest, RoleResponse, UpdateRoleRequest } from '../../core/models/role.models';
import { CompanyResponse, CreateCompanyRequest, UpdateCompanyRequest } from '../../core/models/company.models';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private readonly authService: AuthService = inject(AuthService);
  private readonly platformAdminService: PlatformAdminService = inject(PlatformAdminService);
  private readonly roleService: RoleService = inject(RoleService);
  private readonly companyService: CompanyService = inject(CompanyService);

  // Global & Session Signals
  readonly currentUser = this.authService.currentUser;
  readonly activeSection = signal<'admins' | 'roles' | 'companies'>('admins');
  readonly isSidebarCollapsed = signal<boolean>(false);
  readonly isRefreshing = signal<boolean>(false);
  readonly actionMessage = signal<string | null>(null);
  readonly isErrorMessage = signal<boolean>(false);
  readonly modalErrorMessage = signal<string | null>(null);

  // Platform Admins State
  readonly admins = signal<PlatformAdminResponse[]>([]);
  readonly isLoadingAdmins = signal<boolean>(false);
  adminSearchQuery = '';

  // Roles State
  readonly roles = signal<RoleResponse[]>([]);
  readonly isLoadingRoles = signal<boolean>(false);
  readonly isSubmittingRole = signal<boolean>(false);
  roleSearchQuery = '';
  roleCompanyFilter = '';

  // Role Modals State
  readonly isCreateRoleModalOpen = signal<boolean>(false);
  readonly isEditRoleModalOpen = signal<boolean>(false);
  readonly isDeleteRoleModalOpen = signal<boolean>(false);
  readonly selectedRole = signal<RoleResponse | null>(null);

  // Companies State
  readonly companies = signal<CompanyResponse[]>([]);
  readonly isLoadingCompanies = signal<boolean>(false);
  readonly isSubmittingCompany = signal<boolean>(false);
  companySearchQuery = '';

  // Company Modals State
  readonly isCreateCompanyModalOpen = signal<boolean>(false);
  readonly isEditCompanyModalOpen = signal<boolean>(false);
  readonly isDeleteCompanyModalOpen = signal<boolean>(false);
  readonly selectedCompany = signal<CompanyResponse | null>(null);

  // Create & Edit Role Forms
  createRoleForm: CreateRoleRequest = {
    companyId: '',
    name: '',
    isAdmin: false
  };

  editRoleForm: UpdateRoleRequest = {
    name: '',
    isAdmin: false,
    isActive: true
  };

  // Create & Edit Company Forms
  createCompanyForm: CreateCompanyRequest = {
    name: '',
    email: '',
    phoneNumber: '',
    address: '',
    website: ''
  };

  editCompanyForm: UpdateCompanyRequest = {
    name: '',
    email: '',
    phoneNumber: '',
    address: '',
    website: '',
    isActive: true
  };

  userInitials(): string {
    const name = this.currentUser()?.name || 'AD';
    return name.slice(0, 2).toUpperCase();
  }

  ngOnInit(): void {
    this.loadAdmins();
    this.loadCompanies();
    this.loadRoles();
  }

  // Section Navigation
  onSectionChanged(section: 'admins' | 'roles' | 'companies'): void {
    this.activeSection.set(section);
    if (section === 'admins' && this.admins().length === 0) {
      this.loadAdmins();
    } else if (section === 'roles' && this.roles().length === 0) {
      this.loadRoles();
    } else if (section === 'companies' && this.companies().length === 0) {
      this.loadCompanies();
    }
  }

  toggleSidebar(): void {
    this.isSidebarCollapsed.update(v => !v);
  }

  // ==========================================
  // Platform Admin Management Methods
  // ==========================================
  loadAdmins(): void {
    this.isLoadingAdmins.set(true);
    const query = this.adminSearchQuery?.trim();
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
        const fallback = err.status === 404 ? 'No platform admin exist' : 'Failed to load platform admins.';
        const errorMsg = this.extractErrorMessage(err, fallback);
        this.notify(errorMsg, true);
      }
    });
  }

  onSearchAdmins(): void {
    this.loadAdmins();
  }

  clearAdminSearch(): void {
    this.adminSearchQuery = '';
    this.loadAdmins();
  }

  // ==========================================
  // Role Management Methods
  // ==========================================
  loadRoles(): void {
    this.isLoadingRoles.set(true);
    const query = this.roleSearchQuery?.trim();
    const companyFilter = this.roleCompanyFilter?.trim() || undefined;

    const request$ = query
      ? this.roleService.searchRoles(query, companyFilter)
      : this.roleService.getRoles(companyFilter);

    request$.subscribe({
      next: (res: ApiResponse<RoleResponse[]>) => {
        this.roles.set(res.data || []);
        this.isLoadingRoles.set(false);
      },
      error: (err: any) => {
        this.isLoadingRoles.set(false);
        this.roles.set([]);
        const fallback = err.status === 404 ? 'No roles exist' : 'Failed to load roles.';
        const errorMsg = this.extractErrorMessage(err, fallback);
        this.notify(errorMsg, true);
      }
    });
  }

  onSearchRoles(): void {
    this.loadRoles();
  }

  clearRoleSearch(): void {
    this.roleSearchQuery = '';
    this.loadRoles();
  }

  onCompanyFilterChange(): void {
    this.loadRoles();
  }

  clearCompanyFilter(): void {
    this.roleCompanyFilter = '';
    this.loadRoles();
  }

  // Modal Handlers for Create Role
  openCreateRoleModal(): void {
    this.modalErrorMessage.set(null);
    if (this.companies().length === 0) {
      this.loadCompanies();
    }

    const defaultCompanyId = this.roleCompanyFilter?.trim() ||
      (this.companies().length > 0 ? this.companies()[0].id : '');

    this.createRoleForm = {
      companyId: defaultCompanyId,
      name: '',
      isAdmin: false
    };
    this.isCreateRoleModalOpen.set(true);
  }

  closeCreateRoleModal(): void {
    this.isCreateRoleModalOpen.set(false);
    this.modalErrorMessage.set(null);
  }

  submitCreateRole(): void {
    this.modalErrorMessage.set(null);
    if (!this.createRoleForm.companyId?.trim() || !this.createRoleForm.name?.trim()) {
      const msg = 'Please select a Company and provide a Role Name.';
      this.modalErrorMessage.set(msg);
      this.notify(msg, true);
      return;
    }

    this.isSubmittingRole.set(true);
    this.roleService.createRole({
      companyId: this.createRoleForm.companyId.trim(),
      name: this.createRoleForm.name.trim(),
      isAdmin: this.createRoleForm.isAdmin,
      createdBy: this.currentUser()?.id
    }).subscribe({
      next: (res: ApiResponse<string>) => {
        this.isSubmittingRole.set(false);
        this.closeCreateRoleModal();
        this.notify(res.message || 'Role created successfully.');
        this.loadRoles();
      },
      error: (err: any) => {
        this.isSubmittingRole.set(false);
        const errorMsg = this.extractErrorMessage(err, 'Failed to create role. Please verify uniqueness.');
        this.modalErrorMessage.set(errorMsg);
        this.notify(errorMsg, true);
      }
    });
  }

  // Modal Handlers for Edit Role
  openEditRoleModal(role: RoleResponse): void {
    this.modalErrorMessage.set(null);
    this.selectedRole.set(role);
    this.editRoleForm = {
      name: role.name,
      isAdmin: role.isAdmin,
      isActive: role.isActive
    };
    this.isEditRoleModalOpen.set(true);
  }

  closeEditRoleModal(): void {
    this.isEditRoleModalOpen.set(false);
    this.selectedRole.set(null);
    this.modalErrorMessage.set(null);
  }

  submitUpdateRole(): void {
    this.modalErrorMessage.set(null);
    const role = this.selectedRole();
    if (!role) return;

    if (!this.editRoleForm.name?.trim()) {
      const msg = 'Role name cannot be empty.';
      this.modalErrorMessage.set(msg);
      this.notify(msg, true);
      return;
    }

    this.isSubmittingRole.set(true);
    this.roleService.updateRole(role.id, {
      name: this.editRoleForm.name.trim(),
      isAdmin: this.editRoleForm.isAdmin,
      isActive: this.editRoleForm.isActive,
      updatedBy: this.currentUser()?.id
    }).subscribe({
      next: (res: ApiResponse<RoleResponse>) => {
        this.isSubmittingRole.set(false);
        this.closeEditRoleModal();
        this.notify(res.message || 'Role updated successfully.');
        this.loadRoles();
      },
      error: (err: any) => {
        this.isSubmittingRole.set(false);
        const errorMsg = this.extractErrorMessage(err, 'Failed to update role. Please verify uniqueness.');
        this.modalErrorMessage.set(errorMsg);
        this.notify(errorMsg, true);
      }
    });
  }

  // Modal Handlers for Delete Role
  openDeleteRoleModal(role: RoleResponse): void {
    this.modalErrorMessage.set(null);
    this.selectedRole.set(role);
    this.isDeleteRoleModalOpen.set(true);
  }

  closeDeleteRoleModal(): void {
    this.isDeleteRoleModalOpen.set(false);
    this.selectedRole.set(null);
    this.modalErrorMessage.set(null);
  }

  confirmDeleteRole(): void {
    this.modalErrorMessage.set(null);
    const role = this.selectedRole();
    if (!role) return;

    this.isSubmittingRole.set(true);
    this.roleService.deleteRole(role.id).subscribe({
      next: (res: ApiResponse<any>) => {
        this.isSubmittingRole.set(false);
        this.closeDeleteRoleModal();
        this.notify(res.message || 'Role deleted successfully.');
        this.loadRoles();
      },
      error: (err: any) => {
        this.isSubmittingRole.set(false);
        const errorMsg = this.extractErrorMessage(err, 'Failed to delete role.');
        this.modalErrorMessage.set(errorMsg);
        this.notify(errorMsg, true);
      }
    });
  }

  // ==========================================
  // Company Management Methods
  // ==========================================
  loadCompanies(): void {
    this.isLoadingCompanies.set(true);
    const query = this.companySearchQuery?.trim();

    const request$ = query
      ? this.companyService.searchCompanies(query)
      : this.companyService.getCompanies();

    request$.subscribe({
      next: (res: ApiResponse<CompanyResponse[]>) => {
        this.companies.set(res.data || []);
        this.isLoadingCompanies.set(false);
      },
      error: (err: any) => {
        this.isLoadingCompanies.set(false);
        this.companies.set([]);
        const fallback = err.status === 404 ? 'No companies exist' : 'Failed to load companies.';
        const errorMsg = this.extractErrorMessage(err, fallback);
        this.notify(errorMsg, true);
      }
    });
  }

  onSearchCompanies(): void {
    this.loadCompanies();
  }

  clearCompanySearch(): void {
    this.companySearchQuery = '';
    this.loadCompanies();
  }

  // Modal Handlers for Create Company
  openCreateCompanyModal(): void {
    this.modalErrorMessage.set(null);
    this.createCompanyForm = {
      name: '',
      email: '',
      phoneNumber: '',
      address: '',
      website: ''
    };
    this.isCreateCompanyModalOpen.set(true);
  }

  closeCreateCompanyModal(): void {
    this.isCreateCompanyModalOpen.set(false);
    this.modalErrorMessage.set(null);
  }

  submitCreateCompany(): void {
    this.modalErrorMessage.set(null);
    if (
      !this.createCompanyForm.name?.trim() ||
      !this.createCompanyForm.email?.trim() ||
      !this.createCompanyForm.phoneNumber?.trim() ||
      !this.createCompanyForm.address?.trim()
    ) {
      const msg = 'Please provide Company Name, Email, Phone Number, and Address.';
      this.modalErrorMessage.set(msg);
      this.notify(msg, true);
      return;
    }

    this.isSubmittingCompany.set(true);
    this.companyService.createCompany({
      name: this.createCompanyForm.name.trim(),
      email: this.createCompanyForm.email.trim(),
      phoneNumber: this.createCompanyForm.phoneNumber.trim(),
      address: this.createCompanyForm.address.trim(),
      website: this.createCompanyForm.website?.trim() || undefined,
      createdBy: this.currentUser()?.id
    }).subscribe({
      next: (res: ApiResponse<string>) => {
        this.isSubmittingCompany.set(false);
        this.closeCreateCompanyModal();
        this.notify(res.message || 'Company created successfully.');
        this.loadCompanies();
      },
      error: (err: any) => {
        this.isSubmittingCompany.set(false);
        const errorMsg = this.extractErrorMessage(err, 'Failed to create company. Please verify email uniqueness.');
        this.modalErrorMessage.set(errorMsg);
        this.notify(errorMsg, true);
      }
    });
  }

  // Modal Handlers for Edit Company
  openEditCompanyModal(company: CompanyResponse): void {
    this.modalErrorMessage.set(null);
    this.selectedCompany.set(company);
    this.editCompanyForm = {
      name: company.name,
      email: company.email,
      phoneNumber: company.phoneNumber,
      address: company.address,
      website: company.website || '',
      isActive: company.isActive
    };
    this.isEditCompanyModalOpen.set(true);
  }

  closeEditCompanyModal(): void {
    this.isEditCompanyModalOpen.set(false);
    this.selectedCompany.set(null);
    this.modalErrorMessage.set(null);
  }

  submitUpdateCompany(): void {
    this.modalErrorMessage.set(null);
    const company = this.selectedCompany();
    if (!company) return;

    if (
      !this.editCompanyForm.name?.trim() ||
      !this.editCompanyForm.email?.trim() ||
      !this.editCompanyForm.phoneNumber?.trim() ||
      !this.editCompanyForm.address?.trim()
    ) {
      const msg = 'Please provide Company Name, Email, Phone Number, and Address.';
      this.modalErrorMessage.set(msg);
      this.notify(msg, true);
      return;
    }

    this.isSubmittingCompany.set(true);
    this.companyService.updateCompany(company.id, {
      name: this.editCompanyForm.name.trim(),
      email: this.editCompanyForm.email.trim(),
      phoneNumber: this.editCompanyForm.phoneNumber.trim(),
      address: this.editCompanyForm.address.trim(),
      website: this.editCompanyForm.website?.trim() || undefined,
      isActive: this.editCompanyForm.isActive,
      updatedBy: this.currentUser()?.id
    }).subscribe({
      next: (res: ApiResponse<CompanyResponse>) => {
        this.isSubmittingCompany.set(false);
        this.closeEditCompanyModal();
        this.notify(res.message || 'Company updated successfully.');
        this.loadCompanies();
      },
      error: (err: any) => {
        this.isSubmittingCompany.set(false);
        const errorMsg = this.extractErrorMessage(err, 'Failed to update company. Please verify email uniqueness.');
        this.modalErrorMessage.set(errorMsg);
        this.notify(errorMsg, true);
      }
    });
  }

  // Modal Handlers for Delete Company
  openDeleteCompanyModal(company: CompanyResponse): void {
    this.modalErrorMessage.set(null);
    this.selectedCompany.set(company);
    this.isDeleteCompanyModalOpen.set(true);
  }

  closeDeleteCompanyModal(): void {
    this.isDeleteCompanyModalOpen.set(false);
    this.selectedCompany.set(null);
    this.modalErrorMessage.set(null);
  }

  confirmDeleteCompany(): void {
    this.modalErrorMessage.set(null);
    const company = this.selectedCompany();
    if (!company) return;

    this.isSubmittingCompany.set(true);
    this.companyService.deleteCompany(company.id).subscribe({
      next: (res: ApiResponse<any>) => {
        this.isSubmittingCompany.set(false);
        this.closeDeleteCompanyModal();
        this.notify(res.message || 'Company deleted successfully.');
        this.loadCompanies();
      },
      error: (err: any) => {
        this.isSubmittingCompany.set(false);
        const errorMsg = this.extractErrorMessage(err, 'Failed to delete company. It may have active users or roles.');
        this.modalErrorMessage.set(errorMsg);
        this.notify(errorMsg, true);
      }
    });
  }

  // Navigate directly to roles of a selected company
  viewCompanyRoles(company: CompanyResponse): void {
    this.roleCompanyFilter = company.id;
    this.activeSection.set('roles');
    this.loadRoles();
  }

  // Token Refresh & Utilities
  refreshTokens(): void {
    this.isRefreshing.set(true);
    this.authService.refreshSession().subscribe({
      next: () => {
        this.isRefreshing.set(false);
        this.notify('Session successfully refreshed with new access and refresh tokens.');
      },
      error: (err: any) => {
        this.isRefreshing.set(false);
        const errorMsg = this.extractErrorMessage(err, 'Failed to refresh authentication session.');
        this.notify(errorMsg, true);
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }

  trackByAdminId(index: number, admin: PlatformAdminResponse): string {
    return admin.id;
  }

  trackByRoleId(index: number, role: RoleResponse): string {
    return role.id;
  }

  trackByCompanyId(index: number, company: CompanyResponse): string {
    return company.id;
  }

  extractErrorMessage(err: any, fallback: string): string {
    if (!err) return fallback;
    if (typeof err === 'string' && err.trim()) return err;
    if (typeof err.error === 'string' && err.error.trim()) {
      try {
        const parsed = JSON.parse(err.error);
        if (parsed.message) return parsed.message;
        if (parsed.Message) return parsed.Message;
      } catch {
        return err.error;
      }
    }
    if (err.error?.message) return err.error.message;
    if (err.error?.Message) return err.error.Message;
    if (err.error?.title) return err.error.title;
    if (err.error?.errors && typeof err.error.errors === 'object') {
      const errorList = Object.values(err.error.errors).flat().filter(Boolean);
      if (errorList.length > 0) return String(errorList[0]);
    }
    if (err.message) return err.message;
    if (err.statusText && err.statusText !== 'OK') return `${err.status}: ${err.statusText}`;
    return fallback;
  }

  private notify(message: string, isError = false): void {
    this.actionMessage.set(message);
    this.isErrorMessage.set(isError);
  }
}
