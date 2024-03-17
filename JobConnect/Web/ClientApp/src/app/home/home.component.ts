import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { Select2Data } from 'ng-select2-component';
import { APP_CONFIG, IAppConfig } from '../common/services/app.config';
import { MessageService } from '../common/services/message-service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  constructor(private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    private messageService: MessageService) {
  }

  response = [];
  popularTags = [];
  popularCompanies: any = [];

  cities: Select2Data = [];
  groups: Select2Data = [];
  cityId: number = 0;
  jobGroupId: number = 0;

  userCount = 0;
  jobCount = 0;
  viewsCount = 0;

  title:string;
  groupId:number[];
  provinceId:number[];

  ngOnInit(): void {
    this.get();
  }
  get() {
    this.http.get(`${this.appConfig.apiEndpoint}/Dashboard`).subscribe((response: any) => {
      if (response.latest) {
        response.latest.forEach((item) => {
          if (!item.companyLogo) {
            item.companyLogo = '../assets/images/jobs-company/pic1.jpg';
          }
        });
      }
      this.userCount = response.userCount;
      this.jobCount = response.jobCount;
      this.viewsCount = response.viewsCount;

      this.cities = response.groupStates;
      this.response = response.latest;
      this.popularTags = response.popularTags;
      this.popularCompanies = response.popularCompanies;
      this.groups = response.groups;
    });
  }

  pin(id) {
    const item = {
      Id: id
    };
    this.http.post(`${this.appConfig.apiEndpoint}/userPinJobs/Save`, item).subscribe((response: any) => {
      if (response.isValid) {
        return this.messageService.success(response.message);
      } else {
        return this.messageService.error(response.message);
      }
    });
  }
  onSubmitHandler(title,groupId,provinceId){
   title = this.title;
   groupId = this.jobGroupId;
   provinceId = this.provinceId;
   
   const item ={
    term: title,
    JobGroups : [groupId],
    cities : [provinceId]
   };
   
    this.http.post(`${this.appConfig.apiEndpoint}/job/Get`, item).subscribe((response: any) => {
      if (response.length >= 0) {
        var companyItem= response.map((item) => {
          return {
            id: item.id,
            status: item.status,
            statusName: item.statusName,
            expireDateTime: item.expireDateTime,
            companyName: item.companyName,
            companyRate: item.companyRate,
            attachmentLogoId: item.attachmentLogoId,
            companyLogo: item.companyLogo,
            jobTitle: item.jobTitle,
            jobGroupId: item.jobGroupId,
            jobGroupName: item.jobGroupName,
            workExperienceYearId: item.workExperienceYearId,
            workExperienceYearName: item.workExperienceYearName,
            cityId: item.cityId,
            cityName: item.cityName,
            zoneName: item.zoneName,
            salaryRequestedName: item.salaryRequestedName,
            salaryRequestedId: item.salaryRequestedId,
            contractTypeId: item.contractTypeId,
            contractTypeName: item.contractTypeName,
            dateTimeString: item.dateTimeString,
            totalRowCount: item.totalRowCount,
          }
        });

        this.response = companyItem;
      } else {
        return this.messageService.error(response.message);
      }
    });

  }



  //ngSearch(event) {
  //  if (event.search) {
  //    this.http.get(`${this.appConfig.apiEndpoint}/cities/get?term=${event.search}`).subscribe((response: any) => {
  //      this.cities1 = response;
  //    });
  //  }
  //}
}
