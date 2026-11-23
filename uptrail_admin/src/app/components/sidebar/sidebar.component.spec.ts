import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SidebarComponent } from './sidebar.component';
import { describe, beforeEach, it, expect } from 'vitest';

describe('SidebarComponent', () => {
  let component: SidebarComponent;
  let fixture: ComponentFixture<SidebarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SidebarComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(SidebarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create sidebar component', () => {
    expect(component).toBeTruthy();
  });

  it('should emit section change when selectSection is called for roles', () => {
    let emittedSection: string | undefined;
    component.sectionChange.subscribe(section => {
      emittedSection = section;
    });

    component.selectSection('roles');
    expect(emittedSection).toBe('roles');
  });

  it('should emit section change when selectSection is called for companies', () => {
    let emittedSection: string | undefined;
    component.sectionChange.subscribe(section => {
      emittedSection = section;
    });

    component.selectSection('companies');
    expect(emittedSection).toBe('companies');
  });
});
