import { Injectable } from "@angular/core";
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  constructor(private toastr: ToastrService) { }

  success(message: string) {
    if (!message) {
      message = 'عملیات با موفقیت انجام گردید';
    }
    this.toastr.success(message, '');
  };
  error(body = '') {
    if (!body) {
      body = 'خطایی در هنگام اجرای برنامه اتفاق افتاده است و توسط تیم فنی در حال پیگیری می باشد';
    }

    let result: any[] = [];
    ;
    try {
      const parse = JSON.parse(body);
      parse.forEach((element: any) => {
        result.push(element.Label);
      });
    } catch (e) {
      if (Array.isArray(body)) {
        result = body;
      }
      else {
        result.push(body);
      }
    }
    if (result) {
      if (result.length > 1) {
        const string = result.join(' </br> ');
        this.toastr.error(string, '', {
          timeOut: 3000,
          positionClass: 'toast-bottom-right',
          enableHtml: true
        });
        return;
      }
      this.toastr.error(result[0], '', {
        timeOut: 3000,
        positionClass: 'toast-bottom-right',
        enableHtml: true
      });
    }
  }
}
