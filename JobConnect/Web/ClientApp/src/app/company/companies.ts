import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';

@Component({
  selector: 'app-companies',
  templateUrl: './companies.html'
})
export class Companies {
  groupsCompany = [];
  response = [];

  labels: any = {
    previousLabel: 'قبلی',
    nextLabel: 'بعدی'
  };
  page: number = 1;
  itemsPerPage: number = 10;
  total: number;


  constructor(private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {

    this.http.get(`${this.appConfig.apiEndpoint}/company/groupsCompany`).subscribe((response: any) => {
      this.groupsCompany = response;
    });
    this.get();
  }

  get() {
    const item = {
      pageNumber: this.page,
      pageSize: 10
    }
    this.http.post(`${this.appConfig.apiEndpoint}/company/Get`, item).subscribe((response: any) => {
      if (!response) {
        this.response = [];
        this.total = 0;
        return;
      }
      this.total = response[0].totalRowCount;
      this.response = response;
    });
  }
  onPageChange(number: number) {
    this.page = number;
    this.get();
  }
}
