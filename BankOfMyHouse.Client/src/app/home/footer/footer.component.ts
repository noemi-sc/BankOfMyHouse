// bank-footer.component.ts
import { Component, ChangeDetectionStrategy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgOptimizedImage } from '@angular/common';

interface FooterLink {
  label: string;
  path: string;
  external?: boolean;
}

interface FooterSection {
  title: string;
  links: FooterLink[];
}

interface SocialLink {
  platform: string;
  url: string;
  icon: string;
  ariaLabel: string;
}

@Component({
  selector: 'app-bank-footer',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterModule, NgOptimizedImage],
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css'],
  standalone: true,

  host: {
    '(window:scroll)': 'onScroll()'
  }
})
export class BankFooterComponent {
  // State signals
  showBackToTop = signal(false);
  
  currentYear = computed(() => new Date().getFullYear());

  footerSections = signal<FooterSection[]>([
    {
      title: 'Banking',
      links: [
        { label: 'Personal Banking', path: '/personal' },
        { label: 'Business Banking', path: '/business' },
        { label: 'Online Banking', path: '/online-banking' },
        { label: 'Mobile App', path: '/mobile-app' },
        { label: 'Credit Cards', path: '/credit-cards' },
        { label: 'Loans', path: '/loans' },
        { label: 'Mortgages', path: '/mortgages' }
      ]
    },
    {
      title: 'Services',
      links: [
        { label: 'Investment Services', path: '/investments' },
        { label: 'Insurance', path: '/insurance' },
        { label: 'Wealth Management', path: '/wealth-management' },
        { label: 'Financial Planning', path: '/financial-planning' },
        { label: 'Business Services', path: '/business-services' },
        { label: 'International Banking', path: '/international' }
      ]
    },
    {
      title: 'Support',
      links: [
        { label: 'Help Center', path: '/help' },
        { label: 'Contact Us', path: '/contact' },
        { label: 'Find a Branch', path: '/locations' },
        { label: 'ATM Locator', path: '/atm-locator' },
        { label: 'Security Center', path: '/security' },
        { label: 'Report Fraud', path: '/report-fraud' },
        { label: 'Feedback', path: '/feedback' }
      ]
    }
  ]);

  socialLinks = signal<SocialLink[]>([
    {
      platform: 'facebook',
      url: 'https://facebook.com/securebank',
      icon: 'icon-facebook',
      ariaLabel: 'Follow us on Facebook'
    },
    {
      platform: 'twitter',
      url: 'https://twitter.com/securebank',
      icon: 'icon-twitter',
      ariaLabel: 'Follow us on Twitter'
    },
    {
      platform: 'linkedin',
      url: 'https://linkedin.com/company/securebank',
      icon: 'icon-linkedin',
      ariaLabel: 'Follow us on LinkedIn'
    },
    {
      platform: 'youtube',
      url: 'https://youtube.com/securebank',
      icon: 'icon-youtube',
      ariaLabel: 'Subscribe to our YouTube channel'
    }
  ]);

  // Event handlers
  onScroll() {
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
    this.showBackToTop.set(scrollTop > 300);
  }

  scrollToTop() {
    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }
}