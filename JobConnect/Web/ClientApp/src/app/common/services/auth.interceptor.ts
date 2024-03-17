import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { AuthTokenType } from "./../models/auth-token-type";
import { MessageService } from "./message-service";
import { TokenStoreService } from "./token-store.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private tokenStoreService: TokenStoreService,
    private router: Router,
    private messageService: MessageService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken);
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${accessToken}`,
      },
    });

    return next.handle(request).pipe(
      tap({
        next: (event) => {
          if (event instanceof HttpResponse) {
            if (event.status == 401) {
              this.router.navigate(["/login"]);
            } else if (event.status === 403) {
              this.messageService.error('سطح دسترسی لازم برای انجام عملیات را ندارید.');
              this.router.navigate(["/access-denied"]);
            }
          }
          return event;
        },
        error: (error) => {
          if (error.status == 400) {
            const obj = error.error.errors;
            const entries = Object.entries(obj);
            var entriesArr = entries.map((item: any) => {
              return {
                name: item,
              };
            });
            entriesArr.forEach((element: any) => {
              this.messageService.error(element.name);
            });
          } else if (error.status === 401) {
            this.router.navigate(["/login"]);
          } else if (error.status === 403) {
            this.messageService.error('سطح دسترسی لازم برای انجام عملیات را ندارید.');
            this.router.navigate(["/access-denied"]);
          } else {
            this.messageService.error(`${error.message}`);
          }
        }
      })
    );
  }
}
