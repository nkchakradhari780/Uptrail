import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { describe, beforeEach, it, expect } from 'vitest';
import { CompanyResponse } from '../../core/models/company.models';

describe('DashboardComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])]
    }).compileComponents();
  });

  it('should create the dashboard component', () => {
    const fixture = TestBed.createComponent(DashboardComponent);
    const component = fixture.componentInstance;
    expect(component).toBeTruthy();
  });

  it('should switch sections when onSectionChanged is called', () => {
    const fixture = TestBed.createComponent(DashboardComponent);
    const component = fixture.componentInstance;
    component.onSectionChanged('roles');
    expect(component.activeSection()).toBe('roles');

    component.onSectionChanged('companies');
    expect(component.activeSection()).toBe('companies');
  });

  it('should open and close create role modal', () => {
    const fixture = TestBed.createComponent(DashboardComponent);
    const component = fixture.componentInstance;
    component.openCreateRoleModal();
    expect(component.isCreateRoleModalOpen()).toBe(true);
    component.closeCreateRoleModal();
    expect(component.isCreateRoleModalOpen()).toBe(false);
  });

  it('should open and close create company modal', () => {
    const fixture = TestBed.createComponent(DashboardComponent);
    const component = fixture.componentInstance;
    component.openCreateCompanyModal();
    expect(component.isCreateCompanyModalOpen()).toBe(true);
    component.closeCreateCompanyModal();
    expect(component.isCreateCompanyModalOpen()).toBe(false);
  });

  it('should open and close edit company modal', () => {
    const fixture = TestBed.createComponent(DashboardComponent);
    const component = fixture.componentInstance;
    const mockCompany: CompanyResponse = {
      id: 'c1a11111-1111-1111-1111-111111111111',
      name: 'Test Company',
      email: 'test@example.com',
      phoneNumber: '1234567890',
      address: '123 Main St',
      website: 'https://test.com',
      isActive: true,
      createdAt: new Date().toISOString(),
      activeUsersCount: 0,
      activeRolesCount: 0
    };

    component.openEditCompanyModal(mockCompany);
    expect(component.isEditCompanyModalOpen()).toBe(true);
    expect(component.selectedCompany()).toEqual(mockCompany);

    component.closeEditCompanyModal();
    expect(component.isEditCompanyModalOpen()).toBe(false);
    expect(component.selectedCompany()).toBeNull();
  });

  it('should open and close delete company modal', () => {
    const fixture = TestBed.createComponent(DashboardComponent);
    const component = fixture.componentInstance;
    const mockCompany: CompanyResponse = {
      id: 'c1a11111-1111-1111-1111-111111111111',
      name: 'Test Company',
      email: 'test@example.com',
      phoneNumber: '1234567890',
      address: '123 Main St',
      isActive: true,
      createdAt: new Date().toISOString(),
      activeUsersCount: 0,
      activeRolesCount: 0
    };

    component.openDeleteCompanyModal(mockCompany);
    expect(component.isDeleteCompanyModalOpen()).toBe(true);
    expect(component.selectedCompany()).toEqual(mockCompany);

    component.closeDeleteCompanyModal();
    expect(component.isDeleteCompanyModalOpen()).toBe(false);
    expect(component.selectedCompany()).toBeNull();
  });
});
