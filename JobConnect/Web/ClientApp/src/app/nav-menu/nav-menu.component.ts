import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, mergeMap, Subscription } from 'rxjs';
import { UserService } from '../common/services/user-service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  visible = true;
  displayName: string | undefined = '';
  logoName = 'logo-dark.png';
  isAuthUserLoggedIn = false;
  isExpanded = false;
  subscription: Subscription | null = null;
  objExp = '{left: 15%}';

  constructor(private activatedRoute: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private titleService: Title) {
    this.visible = true;
  }

  ngOnInit() {
    this.subscription = this.userService.authStatus$.subscribe(status => {
      this.isAuthUserLoggedIn = status;
      if (!this.isAuthUserLoggedIn) {
        this.objExp ='{left: 0%}'
      }
      if (status) {
        const authUser = this.userService.getAuthUser();
        this.displayName = authUser ? authUser.displayName : "";
      }
    });

    this.router.events.pipe(filter(event => event instanceof NavigationEnd), map(() => this.activatedRoute),
      map(route => {
        while (route.firstChild) {
          route = route.firstChild;
        }
        return route;
      }),)
      .pipe(
        filter(route => route.outlet === 'primary'),
        mergeMap(route => route.data),
    ).subscribe((data: any) => {
      if (data.title) {
        this.titleService.setTitle(data.title + ' - استخدام|کاریابی');
      }
        if (data.visible == false) {
          this.visible = false;
          return;
        }
        this.visible = true;
        return;
      });
  }


  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  logOut() {
    this.userService.logout(true);
  }
}
