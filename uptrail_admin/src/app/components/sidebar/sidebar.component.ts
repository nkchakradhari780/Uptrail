import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  @Input() activeSection: 'admins' | 'roles' | 'companies' = 'admins';
  @Input() adminCount = 0;
  @Input() roleCount = 0;
  @Input() companyCount = 0;
  @Input() isCollapsed = false;

  @Output() sectionChange = new EventEmitter<'admins' | 'roles' | 'companies'>();
  @Output() toggleCollapse = new EventEmitter<void>();
  @Output() logoutClick = new EventEmitter<void>();

  selectSection(section: 'admins' | 'roles' | 'companies'): void {
    this.sectionChange.emit(section);
  }

  onToggleCollapse(): void {
    this.toggleCollapse.emit();
  }

  onLogout(): void {
    this.logoutClick.emit();
  }
}
