import { HttpClient } from '@angular/common/http'
import { Component, Inject } from '@angular/core'
import { Select2Data } from 'ng-select2-component'
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';
import { MessageService } from '../common/services/message-service';

@Component({
  selector: 'app-user-pin-jobs',
  templateUrl: './user-pin-jobs.html'
})

export class UserPinJobs {
  public labels: any = {
    previousLabel: 'قبلی',
    nextLabel: 'بعدی'
  }
  page: number = 1;
  itemsPerPage: number = 10;
  total: number;

  pageLengths: Select2Data = []
  pageLengthId = 10;

  sortingTypes: Select2Data = [];
  sortingTypeId = 1;

  response = [];

  constructor(private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    private messageService: MessageService) {
    this.http.get(`${this.appConfig.apiEndpoint}/common/setting`).subscribe((response: any) => {
      this.pageLengths = response.pageLengths;
      this.sortingTypes = response.sortingTypes;
    });
  }
  ngOnInit(): void {
    this.get();
  }

  get() {
    this.http.get(`${this.appConfig.apiEndpoint}/UserPinJobs/get?pageNumber=${this.page}&pageSize=${this.itemsPerPage}`).subscribe((response: any) => {
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

  unpin(id:number) {
    this.http.delete(`${this.appConfig.apiEndpoint}/UserPinJobs/Delete?id=${id}`).subscribe((response: any) => {
      if (response.isValid) {
        this.get();
        return this.messageService.success(response.message);
      }
      else {
        return this.messageService.error(response.message);
      }
    })
  }
};


